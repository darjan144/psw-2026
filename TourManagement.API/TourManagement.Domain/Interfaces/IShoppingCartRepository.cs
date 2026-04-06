using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Interfaces;

public interface IShoppingCartRepository
{
    Task<ShoppingCart?> GetByTouristIdAsync(long touristId, CancellationToken cancellationToken = default);
    Task AddAsync(ShoppingCart cart, CancellationToken cancellationToken = default);
    void Update(ShoppingCart cart);
    void Delete(ShoppingCart cart);
}
