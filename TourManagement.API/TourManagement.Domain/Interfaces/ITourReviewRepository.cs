using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Interfaces;

public interface ITourReviewRepository
{
    Task<List<TourReview>> GetByTourIdAsync(long tourId, CancellationToken cancellationToken = default);
    Task<TourReview?> GetByTouristAndTourAsync(long touristId, long tourId, CancellationToken cancellationToken = default);
    Task AddAsync(TourReview review, CancellationToken cancellationToken = default);
}
