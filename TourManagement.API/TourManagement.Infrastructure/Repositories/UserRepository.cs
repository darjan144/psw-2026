using Microsoft.EntityFrameworkCore;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;
using TourManagement.Domain.Interfaces;
using TourManagement.Infrastructure.Persistence;

namespace TourManagement.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly TourManagementDbContext _context;

    public UserRepository(TourManagementDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<List<User>> GetBlockedUsersAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Users.Where(u => u.IsBlocked).ToListAsync(cancellationToken);
    }

    public async Task<List<User>> GetTouristsWithInterestAsync(Interest interest, CancellationToken cancellationToken = default)
    {
        var tourists = await _context.Users
            .Where(u => u.Role == UserRole.Tourist && u.RecommendationsEnabled)
            .ToListAsync(cancellationToken);

        return tourists.Where(u => u.Interests.Contains(interest)).ToList();
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(user, cancellationToken);
    }

    public void Update(User user)
    {
        _context.Users.Update(user);
    }
}
