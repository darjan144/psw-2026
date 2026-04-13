using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Interfaces;

public interface ITourPurchaseRepository
{
    Task<List<TourPurchase>> GetByTouristIdAsync(long touristId, CancellationToken cancellationToken = default);
    Task<List<TourPurchase>> GetByTourIdAsync(long tourId, CancellationToken cancellationToken = default);
    Task<bool> HasPurchasedAsync(long touristId, long tourId, CancellationToken cancellationToken = default);
    Task<List<UpcomingTourPurchaseInfo>> GetPurchasesForUpcomingToursAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task AddAsync(TourPurchase purchase, CancellationToken cancellationToken = default);
}

public record UpcomingTourPurchaseInfo(string TouristEmail, string TouristName, string TourName, DateTime ScheduledDate);
