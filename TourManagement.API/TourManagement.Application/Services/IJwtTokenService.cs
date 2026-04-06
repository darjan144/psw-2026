using TourManagement.Domain.Entities;

namespace TourManagement.Application.Services;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}
