using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Persistence.Configurations;

public class TourPurchaseConfiguration : IEntityTypeConfiguration<TourPurchase>
{
    public void Configure(EntityTypeBuilder<TourPurchase> builder)
    {
        builder.HasKey(tp => tp.Id);

        builder.HasIndex(tp => new { tp.TouristId, tp.TourId });

        builder.Ignore(tp => tp.DomainEvents);
    }
}
