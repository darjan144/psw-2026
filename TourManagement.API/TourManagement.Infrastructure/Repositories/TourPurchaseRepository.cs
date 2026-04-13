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

    public async Task<List<UpcomingTourPurchaseInfo>> GetPurchasesForUpcomingToursAsync(DateTime rangeStart, DateTime rangeEnd, CancellationToken cancellationToken = default)
    {
        return await _context.TourPurchases
            .Join(_context.Tours, p => p.TourId, t => t.Id, (p, t) => new { Purchase = p, Tour = t })
            .Join(_context.Users, pt => pt.Purchase.TouristId, u => u.Id, (pt, u) => new { pt.Tour, User = u })
            .Where(x => x.Tour.ScheduledDate >= rangeStart && x.Tour.ScheduledDate <= rangeEnd)
            .Select(x => new UpcomingTourPurchaseInfo(x.User.Email, x.User.FirstName, x.Tour.Name, x.Tour.ScheduledDate))
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(TourPurchase purchase, CancellationToken cancellationToken = default)
    {
        await _context.TourPurchases.AddAsync(purchase, cancellationToken);
    }
}
