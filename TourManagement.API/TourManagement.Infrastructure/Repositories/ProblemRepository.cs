using Microsoft.EntityFrameworkCore;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;
using TourManagement.Domain.Interfaces;
using TourManagement.Infrastructure.Persistence;

namespace TourManagement.Infrastructure.Repositories;

public class ProblemRepository : IProblemRepository
{
    private readonly TourManagementDbContext _context;

    public ProblemRepository(TourManagementDbContext context)
    {
        _context = context;
    }

    public async Task<Problem?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.Problems
            .Include(p => p.Events)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<List<Problem>> GetByTourIdAsync(long tourId, CancellationToken cancellationToken = default)
    {
        return await _context.Problems
            .Include(p => p.Events)
            .AsSplitQuery()
            .Where(p => p.TourId == tourId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Problem>> GetByTouristIdAsync(long touristId, CancellationToken cancellationToken = default)
    {
        return await _context.Problems
            .Include(p => p.Events)
            .AsSplitQuery()
            .Where(p => p.TouristId == touristId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Problem>> GetByStatusAsync(ProblemStatus status, CancellationToken cancellationToken = default)
    {
        var all = await _context.Problems
            .Include(p => p.Events)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        return all.Where(p => p.Status == status).ToList();
    }

    public async Task AddAsync(Problem problem, CancellationToken cancellationToken = default)
    {
        await _context.Problems.AddAsync(problem, cancellationToken);
    }

    public void Update(Problem problem)
    {
        _context.Problems.Update(problem);
    }
}
