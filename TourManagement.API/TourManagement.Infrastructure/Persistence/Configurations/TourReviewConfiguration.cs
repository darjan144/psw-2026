using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Persistence.Configurations;

public class TourReviewConfiguration : IEntityTypeConfiguration<TourReview>
{
    public void Configure(EntityTypeBuilder<TourReview> builder)
    {
        builder.HasKey(tr => tr.Id);

        builder.Property(tr => tr.Comment).HasMaxLength(1000);

        builder.HasIndex(tr => new { tr.TouristId, tr.TourId }).IsUnique();

        builder.Ignore(tr => tr.DomainEvents);
    }
}
