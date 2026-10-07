using Frame.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Frame.Infrastructure.Persistence.Configurations;

/// <summary>
/// Database shape of the Studios table.
/// Check constraints repeat the entity rules inside SQL Server itself.
/// </summary>
internal sealed class StudioConfiguration : IEntityTypeConfiguration<Studio>
{
    public void Configure(EntityTypeBuilder<Studio> builder)
    {
        builder.ToTable("Studios", table =>
        {
            table.HasCheckConstraint("CK_Studios_Price_Positive", "[PricePerHour] > 0");
            table.HasCheckConstraint("CK_Studios_Hours_Valid",
                "[OpenHour] >= 0 AND [CloseHour] <= 24 AND [OpenHour] < [CloseHour]");
        });

        builder.HasKey(s => s.Id);

        builder.Property(s => s.NameAr).HasMaxLength(100).IsRequired();
        builder.Property(s => s.NameEn).HasMaxLength(100).IsRequired();

        builder.Property(s => s.DescriptionAr).HasMaxLength(1000).IsRequired();
        builder.Property(s => s.DescriptionEn).HasMaxLength(1000).IsRequired();

        builder.Property(s => s.ImageUrl)
               .HasMaxLength(500)
               .IsUnicode(false)
               .IsRequired();

        // Precision (10,3) comes from the decimal convention in FrameDbContext.
        builder.Property(s => s.PricePerHour).IsRequired();

        builder.Property(s => s.OpenHour).IsRequired();
        builder.Property(s => s.CloseHour).IsRequired();

        builder.Property(s => s.IsActive)
               .HasDefaultValue(true)
               .IsRequired();

        builder.Property(s => s.CreatedAt).IsRequired();
    }
}
