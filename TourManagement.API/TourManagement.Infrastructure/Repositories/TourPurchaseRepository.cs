using Microsoft.EntityFrameworkCore;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces;
using TourManagement.Infrastructure.Persistence;

namespace TourManagement.Infrastructure.Repositories;

public class TourPurchaseRepository : ITourPurchaseRepository
{
    private readonly TourManagementDbContext _context;

    public TourPurchaseRepository(TourManagementDbContext context)
    {
        _context = context;
    }

    public async Task<List<TourPurchase>> GetByTouristIdAsync(long touristId, CancellationToken cancellationToken = default)
    {
        return await _context.TourPurchases.Where(p => p.TouristId == touristId).ToListAsync(cancellationToken);
    }

    public async Task<List<TourPurchase>> GetByTourIdAsync(long tourId, CancellationToken cancellationToken = default)
    {
        return await _context.TourPurchases.Where(p => p.TourId == tourId).ToListAsync(cancellationToken);
    }

    public async Task<bool> HasPurchasedAsync(long touristId, long tourId, CancellationToken cancellationToken = default)
    {
        return await _context.TourPurchases.AnyAsync(p => p.TouristId == touristId && p.TourId == tourId, cancellationToken);
    }

    public async Task AddAsync(TourPurchase purchase, CancellationToken cancellationToken = default)
    {
        await _context.TourPurchases.AddAsync(purchase, cancellationToken);
    }
}
