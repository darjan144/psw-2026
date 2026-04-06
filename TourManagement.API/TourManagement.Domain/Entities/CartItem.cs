namespace TourManagement.Domain.Entities;

public class CartItem : Entity
{
    public long TourId { get; private set; }
    public string TourName { get; private set; }
    public double Price { get; private set; }
    public long ShoppingCartId { get; private set; }

    private CartItem() { } // EF Core

    public CartItem(long tourId, string tourName, double price)
    {
        TourId = tourId;
        TourName = tourName;
        Price = price;
    }
}
