using TourManagement.Domain.ValueObjects;

namespace TourManagement.Domain.Entities;

public class KeyPoint : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Coordinate Position { get; private set; }
    public string ImageUrl { get; private set; }
    public int Order { get; private set; }
    public long TourId { get; private set; }

    private KeyPoint() { } // EF Core

    public KeyPoint(string name, string description, Coordinate position, string imageUrl, int order)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Key point name is required.", nameof(name));

        Name = name;
        Description = description ?? string.Empty;
        Position = position ?? throw new ArgumentNullException(nameof(position));
        ImageUrl = imageUrl ?? string.Empty;
        Order = order;
    }

    public void Update(string name, string description, Coordinate position, string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Key point name is required.", nameof(name));

        Name = name;
        Description = description ?? string.Empty;
        Position = position ?? throw new ArgumentNullException(nameof(position));
        ImageUrl = imageUrl ?? string.Empty;
    }

    public void SetOrder(int order) => Order = order;
}
