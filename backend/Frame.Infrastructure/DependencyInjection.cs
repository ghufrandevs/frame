using Frame.Application.Common.Abstractions;
using Frame.Infrastructure.Persistence;
using Frame.Infrastructure.Persistence.Seed;
using Frame.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Frame.Infrastructure;

/// <summary>
/// The single entry point the API calls to register everything
/// this layer provides (database, security; repositories, email,
/// payment and auth services as we build them).
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

        services.AddScoped<DatabaseSeeder>();

        // ===== Security =====
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