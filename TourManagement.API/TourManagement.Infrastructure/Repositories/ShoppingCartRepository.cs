using Microsoft.EntityFrameworkCore;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces;
using TourManagement.Infrastructure.Persistence;

namespace TourManagement.Infrastructure.Repositories;

public class ShoppingCartRepository : IShoppingCartRepository
{
    private readonly TourManagementDbContext _context;

    public ShoppingCartRepository(TourManagementDbContext context)
    {
        _context = context;
    }

    public async Task<ShoppingCart?> GetByTouristIdAsync(long touristId, CancellationToken cancellationToken = default)
    {
        return await _context.ShoppingCarts
            .Include(sc => sc.Items)
            .FirstOrDefaultAsync(sc => sc.TouristId == touristId, cancellationToken);
    }

    public async Task AddAsync(ShoppingCart cart, CancellationToken cancellationToken = default)
    {
        await _context.ShoppingCarts.AddAsync(cart, cancellationToken);
    }

    public void Update(ShoppingCart cart)
    {
        _context.ShoppingCarts.Update(cart);
    }

    public void Delete(ShoppingCart cart)
    {
        _context.ShoppingCarts.Remove(cart);
    }
}
