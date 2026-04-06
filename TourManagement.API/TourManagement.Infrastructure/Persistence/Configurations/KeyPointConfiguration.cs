using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Persistence.Configurations;

public class KeyPointConfiguration : IEntityTypeConfiguration<KeyPoint>
{
    public void Configure(EntityTypeBuilder<KeyPoint> builder)
    {
        builder.HasKey(kp => kp.Id);

        builder.Property(kp => kp.Name).IsRequired().HasMaxLength(200);
        builder.Property(kp => kp.Description).HasMaxLength(1000);
        builder.Property(kp => kp.ImageUrl).HasMaxLength(500);

        builder.OwnsOne(kp => kp.Position, pos =>
        {
            pos.Property(p => p.Latitude).HasColumnName("Latitude");
            pos.Property(p => p.Longitude).HasColumnName("Longitude");
        });

        builder.Ignore(kp => kp.DomainEvents);
    }
}
