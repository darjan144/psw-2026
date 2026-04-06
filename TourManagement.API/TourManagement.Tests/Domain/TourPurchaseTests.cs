using FluentAssertions;
using TourManagement.Domain.Entities;

namespace TourManagement.Tests.Domain;

public class TourPurchaseTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreatePurchase()
    {
        var purchase = new TourPurchase(touristId: 1, tourId: 10, pricePaid: 1500);

        purchase.TouristId.Should().Be(1);
        purchase.TourId.Should().Be(10);
        purchase.PricePaid.Should().Be(1500);
        purchase.PurchasedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }
}
