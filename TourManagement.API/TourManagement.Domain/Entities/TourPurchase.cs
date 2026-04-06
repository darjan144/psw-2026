namespace TourManagement.Domain.Entities;

public class TourPurchase : Entity
{
    public long TouristId { get; private set; }
    public long TourId { get; private set; }
    public double PricePaid { get; private set; }
    public DateTime PurchasedAt { get; private set; }

    private TourPurchase() { } // EF Core

    public TourPurchase(long touristId, long tourId, double pricePaid)
    {
        TouristId = touristId;
        TourId = tourId;
        PricePaid = pricePaid;
        PurchasedAt = DateTime.UtcNow;
    }
}
