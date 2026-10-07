using FluentValidation;
using Frame.Application.Auth;
using Frame.Application.Bookings;
using Frame.Application.Studios;
using Microsoft.Extensions.DependencyInjection;

namespace Frame.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IStudioService, StudioService>();
        services.AddScoped<IBookingService, BookingService>();

        services.AddSingleton<IPriceCalculator, PriceCalculator>();

        return services;
    }
}