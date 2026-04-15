using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Entities.ProblemEvents;

namespace TourManagement.Infrastructure.Persistence.Configurations;

public class ProblemConfiguration : IEntityTypeConfiguration<Problem>
{
    public void Configure(EntityTypeBuilder<Problem> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Description).IsRequired().HasMaxLength(2000);

        builder.HasIndex(p => p.TourId);
        builder.HasIndex(p => p.TouristId);

        builder.Ignore(p => p.DomainEvents);
        builder.Ignore(p => p.Status);

        builder.HasMany<ProblemStateEvent>("_events")
            .WithOne()
            .HasForeignKey(e => e.ProblemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata.FindNavigation("_events")!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
