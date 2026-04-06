using Microsoft.EntityFrameworkCore;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces;
using TourManagement.Infrastructure.Persistence;

namespace TourManagement.Infrastructure.Repositories;

public class TourReviewRepository : ITourReviewRepository
{
    private readonly TourManagementDbContext _context;

    public TourReviewRepository(TourManagementDbContext context)
    {
        _context = context;
    }

    public async Task<List<TourReview>> GetByTourIdAsync(long tourId, CancellationToken cancellationToken = default)
    {
        return await _context.TourReviews.Where(r => r.TourId == tourId).ToListAsync(cancellationToken);
    }

    public async Task<TourReview?> GetByTouristAndTourAsync(long touristId, long tourId, CancellationToken cancellationToken = default)
    {
        return await _context.TourReviews.FirstOrDefaultAsync(r => r.TouristId == touristId && r.TourId == tourId, cancellationToken);
    }

    public async Task AddAsync(TourReview review, CancellationToken cancellationToken = default)
    {
        await _context.TourReviews.AddAsync(review, cancellationToken);
    }
}
