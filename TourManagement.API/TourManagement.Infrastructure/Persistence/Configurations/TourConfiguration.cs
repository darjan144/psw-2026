using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Persistence.Configurations;

public class TourConfiguration : IEntityTypeConfiguration<Tour>
{
    public void Configure(EntityTypeBuilder<Tour> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Description).HasMaxLength(2000);
        builder.Property(t => t.Difficulty).HasConversion<string>().HasMaxLength(20);
        builder.Property(t => t.Category).HasConversion<string>().HasMaxLength(20);
        builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasMany(t => t.KeyPoints)
            .WithOne()
            .HasForeignKey(kp => kp.TourId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(t => t.DomainEvents);
    }
}
