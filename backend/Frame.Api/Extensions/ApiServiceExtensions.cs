using System.Text;
using System.Text.Json;
using System.Threading.RateLimiting;
using Frame.Api.Filters;
using Frame.Api.Middleware;
using Frame.Api.Security;
using Frame.Api.Services;
using Frame.Application.Common.Abstractions;
using Frame.Application.Common.Errors;
using Frame.Domain.Enums;
using Frame.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

namespace Frame.Api.Extensions;

/// <summary>
/// Everything the HTTP layer needs, split into small focused methods:
/// controllers + validation, request language, JWT authentication + policies,
/// CORS, login rate limiting and Swagger. Program.cs calls AddFrameApi once.
/// </summary>
internal static class ApiServiceExtensions
{
    // Development defaults (Live Server = 5500). Docker overrides them via configuration.
    private static readonly string[] DefaultWebOrigins =
        ["http://localhost:4200", "http://localhost:5500", "http://127.0.0.1:5500"];

    private static readonly string[] DefaultAdminOrigins =
        ["http://localhost:4201", "http://localhost:5500", "http://127.0.0.1:5500"];

    public static IServiceCollection AddFrameApi(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddControllers(options => options.Filters.Add<ValidationFilter>())
            .ConfigureApiBehaviorOptions(options => options.InvalidModelStateResponseFactory = BuildInvalidRequestResponse);

        // Request language (Accept-Language → "ar" / "en") for the Application layer.
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentLanguage, HttpCurrentLanguage>();

        services.AddFrameAuthentication(configuration);
        services.AddFrameCors(configuration);
        services.AddFrameRateLimiting();
        services.AddFrameSwagger();

        return services;
    }

    // ===== JWT authentication + the two isolated policies =====
    private static void AddFrameAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwt = JwtOptions.FromConfiguration(configuration);

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudiences = [JwtOptions.CustomerAudience, JwtOptions.AdminAudience],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };

                // 401 / 403 in the same JSON shape as every other error.
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        await ExceptionMiddleware.WriteErrorAsync(
                            context.HttpContext, StatusCodes.Status401Unauthorized, ErrorCodes.Unauthorized);
                    },
                    OnForbidden = context => ExceptionMiddleware.WriteErrorAsync(
                        context.HttpContext, StatusCodes.Status403Forbidden, ErrorCodes.Forbidden)
                };
            });

        services.AddAuthorizationBuilder()
            .AddPolicy(AuthPolicies.Customer, policy => policy
                .RequireAuthenticatedUser()
                .RequireRole(nameof(UserRole.Customer))
                .RequireClaim("aud", JwtOptions.CustomerAudience))
            .AddPolicy(AuthPolicies.Admin, policy => policy
                .RequireAuthenticatedUser()
                .RequireRole(nameof(UserRole.Admin))
                .RequireClaim("aud", JwtOptions.AdminAudience));
    }

    // ===== CORS: customer site by default, admin app only on admin controllers =====
    private static void AddFrameCors(this IServiceCollection services, IConfiguration configuration)
    {
        var webOrigins = configuration.GetSection("Cors:WebOrigins").Get<string[]>() ?? DefaultWebOrigins;
        var adminOrigins = configuration.GetSection("Cors:AdminOrigins").Get<string[]>() ?? DefaultAdminOrigins;

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy => policy.WithOrigins(webOrigins).AllowAnyHeader().AllowAnyMethod());
            options.AddPolicy(CorsPolicies.Admin, policy => policy.WithOrigins(adminOrigins).AllowAnyHeader().AllowAnyMethod());
        });
    }

    // ===== Login brute-force protection: 5 attempts per minute per IP =====
    private static void AddFrameRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.AddPolicy(AuthPolicies.LoginRateLimit, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    }));

            options.OnRejected = (context, _) => new ValueTask(ExceptionMiddleware.WriteErrorAsync(
                context.HttpContext, StatusCodes.Status429TooManyRequests, ErrorCodes.TooManyRequests));
        });
    }

    // ===== Swagger UI with an "Authorize" button for the JWT =====
    private static void AddFrameSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Frame API", Version = "v1" });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Paste the token from a login response (without the word Bearer)."
            });

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            });
        });
    }

    // ===== Broken or incomplete JSON → the contract error shape, not ASP.NET's default =====
    private static IActionResult BuildInvalidRequestResponse(ActionContext context)
    {
        var fieldErrors = context.ModelState
            .Where(entry => entry.Value is { Errors.Count: > 0 } && !entry.Key.StartsWith('$'))
            .ToDictionary(
                entry => JsonNamingPolicy.CamelCase.ConvertName(entry.Key),
                _ => new[] { FieldErrorCodes.Required });

        return new ObjectResult(new
        {
            status = StatusCodes.Status400BadRequest,
            code = ErrorCodes.ValidationError,
            message = "Bad Request",
            errors = fieldErrors.Count > 0 ? fieldErrors : null,
            traceId = context.HttpContext.TraceIdentifier
        })
        {
            StatusCode = StatusCodes.Status400BadRequest
        };
    }
}