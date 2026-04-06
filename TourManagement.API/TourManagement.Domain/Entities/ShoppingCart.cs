namespace TourManagement.Domain.Entities;

public class ShoppingCart : Entity
{
    public long TouristId { get; private set; }

    private readonly List<CartItem> _items = new();
    public IReadOnlyCollection<CartItem> Items => _items.AsReadOnly();

    private ShoppingCart() { } // EF Core

    public ShoppingCart(long touristId)
    {
        TouristId = touristId;
    }

    public void AddItem(long tourId, string tourName, double price)
    {
        if (_items.Any(i => i.TourId == tourId))
            throw new InvalidOperationException("Tour is already in the cart.");

        _items.Add(new CartItem(tourId, tourName, price));
    }

    public void RemoveItem(long tourId)
    {
        var item = _items.FirstOrDefault(i => i.TourId == tourId);
        if (item == null)
            throw new InvalidOperationException("Tour is not in the cart.");

        _items.Remove(item);
    }

    public double GetTotalPrice()
    {
        return _items.Sum(i => i.Price);
    }

    public double ApplyBonusPoints(int bonusPoints)
    {
        var total = GetTotalPrice();
        var discount = Math.Min(bonusPoints, total);
        return total - discount;
    }

    public void Clear()
    {
        _items.Clear();
    }
}
