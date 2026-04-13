using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;

namespace TourManagement.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<List<User>> GetBlockedUsersAsync(CancellationToken cancellationToken = default);
    Task<List<User>> GetTouristsWithInterestAsync(Interest interest, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    void Update(User user);
}
