using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;

namespace TourManagement.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Username).IsRequired().HasMaxLength(100);
        builder.HasIndex(u => u.Username).IsUnique();

        builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.LastName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.PasswordHash).IsRequired();

        builder.Property(u => u.Role).HasConversion<string>().HasMaxLength(20);

        builder.Property<List<Interest>>("_interests")
            .HasColumnName("Interests")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Ignore(u => u.DomainEvents);
    }
}
