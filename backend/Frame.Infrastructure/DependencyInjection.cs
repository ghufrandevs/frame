using Frame.Application.Common.Abstractions;
using Frame.Application.Common.Abstractions.Persistence;
using Frame.Infrastructure.Persistence;
using Frame.Infrastructure.Persistence.Repositories;
using Frame.Infrastructure.Persistence.Seed;
using Frame.Infrastructure.Security;
using Frame.Infrastructure.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Frame.Infrastructure;

/// <summary>
/// The single entry point the API calls to register everything
/// this layer provides: database, repositories, time, security
/// (and later: email, payment).
/// Program.cs stays one line: builder.Services.AddInfrastructure(...)
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ===== Database =====
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException(
                "Connection string 'Default' is missing. " +
                "Set it with user-secrets (local) or the ConnectionStrings__Default environment variable (Docker).");

        services.AddDbContext<FrameDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<DatabaseSeeder>();

        // ===== Repositories =====
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IStudioRepository, StudioRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();

        // ===== Time =====
        services.AddSingleton<IClock, MuscatClock>();

        // ===== Security =====
        services.AddSingleton(JwtOptions.FromConfiguration(configuration));
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IPasswordHasher, IdentityPasswordHasher>();

        return services;
    }

    /// <summary>
    /// Applies migrations and seeds studios + admin. Called once from Program.cs at startup.
    /// </summary>
    public static async Task InitializeDatabaseAsync(
        this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync(cancellationToken);
    }
}