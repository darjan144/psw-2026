using Microsoft.EntityFrameworkCore;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;
using TourManagement.Domain.Interfaces;
using TourManagement.Infrastructure.Persistence;

namespace TourManagement.Infrastructure.Repositories;

public class TourRepository : ITourRepository
{
    private readonly TourManagementDbContext _context;

    public TourRepository(TourManagementDbContext context)
    {
        _context = context;
    }

    public async Task<Tour?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.Tours
            .Include(t => t.KeyPoints)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<List<Tour>> GetByGuideIdAsync(long guideId, CancellationToken cancellationToken = default)
    {
        return await _context.Tours
            .Include(t => t.KeyPoints)
            .Where(t => t.GuideId == guideId)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Tour>> GetPublishedAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Tours
            .Include(t => t.KeyPoints)
            .Where(t => t.Status == TourStatus.Published)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Tour>> GetSeekingSubstituteAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Tours
            .Include(t => t.KeyPoints)
            .Where(t => t.SeekingSubstitute)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Tour tour, CancellationToken cancellationToken = default)
    {
        await _context.Tours.AddAsync(tour, cancellationToken);
    }

    public void Update(Tour tour)
    {
        _context.Tours.Update(tour);
    }
}
