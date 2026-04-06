using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourManagement.Domain.Entities;

namespace TourManagement.Infrastructure.Persistence.Configurations;

public class ShoppingCartConfiguration : IEntityTypeConfiguration<ShoppingCart>
{
    public void Configure(EntityTypeBuilder<ShoppingCart> builder)
    {
        builder.HasKey(sc => sc.Id);

        builder.HasIndex(sc => sc.TouristId).IsUnique();

        builder.HasMany(sc => sc.Items)
            .WithOne()
            .HasForeignKey(ci => ci.ShoppingCartId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(sc => sc.DomainEvents);
    }
}
