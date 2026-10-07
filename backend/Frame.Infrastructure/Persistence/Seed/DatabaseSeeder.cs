using Frame.Application.Common.Abstractions;
using Frame.Domain.Entities;
using Frame.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Frame.Infrastructure.Persistence.Seed;

/// <summary>
/// Prepares the database on startup: applies pending migrations,
/// then adds the four studios and the admin account if they are missing.
/// Safe to run on every start: it never duplicates or overwrites data.
/// </summary>
internal sealed class DatabaseSeeder
{
    private const int DefaultOpenHour = 9;
    private const int DefaultCloseHour = 22;

    private readonly FrameDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(
        FrameDbContext db,
        IPasswordHasher passwordHasher,
        IConfiguration configuration,
        ILogger<DatabaseSeeder> logger)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await _db.Database.MigrateAsync(cancellationToken);
        await SeedStudiosAsync(cancellationToken);
        await SeedAdminAsync(cancellationToken);
    }

    private async Task SeedStudiosAsync(CancellationToken cancellationToken)
    {
        if (await _db.Studios.AnyAsync(cancellationToken))
            return;

        _db.Studios.AddRange(
            Studio.Create(
                "الاستوديو الأبيض", "White Studio",
                "خلفية بيضاء لا نهائية مع إضاءة احترافية، مثالي لتصوير المنتجات والبورتريه.",
                "Infinite white backdrop with professional lighting, ideal for product and portrait shoots.",
                "/images/studios/white.jpg", 25.000m, DefaultOpenHour, DefaultCloseHour),

            Studio.Create(
                "استوديو البودكاست", "Podcast Studio",
                "غرفة معزولة صوتياً مع أربعة مايكات وكاميرات، جاهزة للتسجيل مباشرة.",
                "Soundproof room with four microphones and cameras, ready to record.",
                "/images/studios/podcast.jpg", 30.000m, DefaultOpenHour, DefaultCloseHour),

            Studio.Create(
                "استوديو الكروما", "Chroma Studio",
                "جدار أخضر بإضاءة متساوية لتصوير المؤثرات واستبدال الخلفية.",
                "Evenly lit green screen for visual effects and background replacement.",
                "/images/studios/chroma.jpg", 30.000m, DefaultOpenHour, DefaultCloseHour),

            Studio.Create(
                "غرفة المونتاج", "Editing Suite",
                "جهاز مونتاج قوي مع شاشات معايرة الألوان وبرامج تحرير احترافية.",
                "Powerful editing workstation with color-calibrated monitors and pro software.",
                "/images/studios/editing.jpg", 25.000m, DefaultOpenHour, DefaultCloseHour));

        await _db.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Seeded 4 studios");
    }

    private async Task SeedAdminAsync(CancellationToken cancellationToken)
    {
        if (await _db.Users.AnyAsync(u => u.Role == UserRole.Admin, cancellationToken))
            return;

        var email = Required("Admin:Email");
        var password = Required("Admin:Password");
        var fullName = _configuration["Admin:FullName"] ?? "Frame Admin";
        var phone = _configuration["Admin:Phone"] ?? "+96890000000";

        var admin = User.CreateAdmin(fullName, email, phone, _passwordHasher.Hash(password));

        _db.Users.Add(admin);
        await _db.SaveChangesAsync(cancellationToken);

        // Never log the password, only who was created.
        _logger.LogInformation("Seeded admin account {Email}", admin.Email);
    }

    private string Required(string key)
        => _configuration[key] is { Length: > 0 } value
            ? value
            : throw new InvalidOperationException(
                $"Setting '{key}' is missing. Set it with user-secrets (local) or an environment variable (Docker).");
}
