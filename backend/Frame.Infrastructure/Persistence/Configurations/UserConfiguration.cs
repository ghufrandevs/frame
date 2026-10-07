using Frame.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Frame.Infrastructure.Persistence.Configurations;

/// <summary>
/// Database shape of the Users table.
/// Keeps every SQL detail out of the User entity.
/// </summary>
internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.FullName)
               .HasMaxLength(100)
               .IsRequired();

        builder.Property(u => u.Email)
               .HasMaxLength(256)
               .IsRequired();

        // Two accounts can never share an email, even if two sign-ups arrive together.
        builder.HasIndex(u => u.Email)
               .IsUnique();

        builder.Property(u => u.Phone)
               .HasMaxLength(20)
               .IsUnicode(false)
               .IsRequired();

        builder.Property(u => u.PasswordHash)
               .HasMaxLength(512)
               .IsRequired();

        // Stored as readable text ("Customer" / "Admin") instead of 1 / 2.
        builder.Property(u => u.Role)
               .HasConversion<string>()
               .HasMaxLength(20)
               .IsRequired();

        builder.Property(u => u.CreatedAt)
               .IsRequired();
    }
}
