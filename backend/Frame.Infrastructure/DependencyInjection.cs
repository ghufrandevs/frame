using Frame.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Frame.Infrastructure;

/// <summary>
/// The single entry point the API calls to register everything
/// this layer provides (database now; repositories, email,
/// payment and auth services as we build them).
/// Program.cs stays one line: builder.Services.AddInfrastructure(...)
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException(
                "Connection string 'Default' is missing. " +
                "Set it with user-secrets (local) or the ConnectionStrings__Default environment variable (Docker).");

        services.AddDbContext<FrameDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }
}
