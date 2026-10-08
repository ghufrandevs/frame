using Frame.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Frame.Infrastructure.Persistence.Configurations;

/// <summary>
/// Database shape of the Bookings table: relations, unique numbers,
/// the index used by the clash check, and check constraints that
/// repeat the booking rules inside SQL Server.
/// </summary>
internal sealed class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings", table =>
        {
            table.HasCheckConstraint("CK_Bookings_Hours_Valid",
                $"[EndHour] > [StartHour] AND [EndHour] - [StartHour] <= {Booking.MaxHours}");
            table.HasCheckConstraint("CK_Bookings_Amounts_Valid",
                "[Subtotal] > 0 AND [VatAmount] >= 0 AND [TotalAmount] = [Subtotal] + [VatAmount]");
        });

        builder.HasKey(b => b.Id);

        // ===== Public numbers =====
        builder.Property(b => b.BookingNumber).HasMaxLength(20).IsUnicode(false).IsRequired();
        builder.HasIndex(b => b.BookingNumber).IsUnique();

        builder.Property(b => b.InvoiceNumber).HasMaxLength(30).IsUnicode(false).IsRequired();
        builder.HasIndex(b => b.InvoiceNumber).IsUnique();

        // ===== Relations: Restrict = a user or studio with bookings can never be deleted =====
        builder.HasOne(b => b.User)
               .WithMany()
               .HasForeignKey(b => b.UserId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(b => b.Studio)
               .WithMany()
               .HasForeignKey(b => b.StudioId)
               .OnDelete(DeleteBehavior.Restrict);

        // ===== Time =====
        builder.Property(b => b.BookingDate).IsRequired();
        builder.Property(b => b.StartHour).IsRequired();
        builder.Property(b => b.EndHour).IsRequired();

        // Fast clash check and availability: "bookings of studio X on day Y that are Confirmed".
        builder.HasIndex(b => new { b.StudioId, b.BookingDate, b.Status });

        // ===== Price snapshot (precision 10,3 from the decimal convention) =====
        builder.Property(b => b.HourlyRate).IsRequired();
        builder.Property(b => b.Subtotal).IsRequired();
        builder.Property(b => b.VatAmount).IsRequired();
        builder.Property(b => b.TotalAmount).IsRequired();

        // ===== Payment: safe data only =====
        builder.Property(b => b.PaymentReference).HasMaxLength(100).IsUnicode(false).IsRequired();
        builder.Property(b => b.CardBrand).HasMaxLength(20).IsUnicode(false).IsRequired();
        builder.Property(b => b.CardLast4).HasMaxLength(4).IsFixedLength().IsUnicode(false).IsRequired();
        builder.Property(b => b.PaidAt).IsRequired();

        // ===== Status =====
        builder.Property(b => b.Status)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(b => b.CancelledAt);
        builder.Property(b => b.RefundAmount);

        builder.Property(b => b.CreatedAt).IsRequired();

        // ===== Calculated in code, never stored =====
        builder.Ignore(b => b.Hours);
        builder.Ignore(b => b.StartsAt);
        builder.Ignore(b => b.EndsAt);
    }
}