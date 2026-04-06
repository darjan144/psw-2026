using TourManagement.Domain.Entities;

namespace TourManagement.Domain.Interfaces;

public interface IProblemRepository
{
    Task<Problem?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<List<Problem>> GetByTourIdAsync(long tourId, CancellationToken cancellationToken = default);
    Task<List<Problem>> GetByTouristIdAsync(long touristId, CancellationToken cancellationToken = default);
    Task AddAsync(Problem problem, CancellationToken cancellationToken = default);
    void Update(Problem problem);
}
