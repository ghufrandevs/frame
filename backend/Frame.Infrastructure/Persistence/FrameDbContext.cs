using Frame.Domain.Common;
using Frame.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Frame.Infrastructure.Persistence;

/// <summary>
/// The EF Core gateway to SQL Server. Knows the tables and how to save them.
/// Table rules (lengths, keys, indexes) live in the Configurations folder,
/// one file per table, not here.
/// </summary>
public sealed class FrameDbContext : DbContext
{
    /// <summary>Name of the SQL sequence that gives booking numbers (FR-1001, FR-1002...).</summary>
    public const string BookingNumberSequence = "BookingNumbers";

    public FrameDbContext(DbContextOptions<FrameDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Studio> Studios => Set<Studio>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<EmailMessage> EmailMessages => Set<EmailMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Running number for bookings, safe even when two people pay at the same moment.
        modelBuilder.HasSequence<int>(BookingNumberSequence)
                    .StartsAt(1001)
                    .IncrementsBy(1);

        // Picks up every IEntityTypeConfiguration in this project automatically.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FrameDbContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Every money value in the system: OMR with 3 decimal places.
        configurationBuilder.Properties<decimal>().HavePrecision(10, 3);
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        StampCreatedAt();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        StampCreatedAt();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    /// <summary>Fills CreatedAt (UTC) for every new row, so no service has to remember it.</summary>
    private void StampCreatedAt()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
                entry.Property(nameof(BaseEntity.CreatedAt)).CurrentValue = now;
        }
    }
}
