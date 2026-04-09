using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;

namespace TourManagement.Domain.Interfaces;

public interface ITourRepository
{
    Task<Tour?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<List<Tour>> GetByGuideIdAsync(long guideId, CancellationToken cancellationToken = default);
    Task<List<Tour>> GetPublishedAsync(CancellationToken cancellationToken = default);
    Task<List<Tour>> GetSeekingSubstituteAsync(CancellationToken cancellationToken = default);
    Task<bool> HasTourOnDateAsync(long guideId, DateTime date, CancellationToken cancellationToken = default);
    Task AddAsync(Tour tour, CancellationToken cancellationToken = default);
    void Update(Tour tour);
}
