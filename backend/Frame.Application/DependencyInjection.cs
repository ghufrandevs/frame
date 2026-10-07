using FluentValidation;
using Frame.Application.Auth;
using Frame.Application.Studios;
using Microsoft.Extensions.DependencyInjection;

namespace Frame.Application;

/// <summary>
/// Registers everything the Application layer provides:
/// all FluentValidation validators and the feature services.
/// Program.cs calls it once: builder.Services.AddApplication();
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Finds every AbstractValidator<T> in this project automatically.
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        // ===== Feature services =====
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IStudioService, StudioService>();

        return services;
    }
}