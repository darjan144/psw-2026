using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;

namespace TourManagement.Domain.Interfaces;

public interface IProblemRepository
{
    Task<Problem?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<List<Problem>> GetByTourIdAsync(long tourId, CancellationToken cancellationToken = default);
    Task<List<Problem>> GetByTouristIdAsync(long touristId, CancellationToken cancellationToken = default);
    Task<List<Problem>> GetByStatusAsync(ProblemStatus status, CancellationToken cancellationToken = default);
    Task AddAsync(Problem problem, CancellationToken cancellationToken = default);
    void Update(Problem problem);
}
