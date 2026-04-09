using TourManagement.Application.DTOs;
using TourManagement.Domain.Entities;

namespace TourManagement.Application.Mappings;

public static class UserMappingExtensions
{
    public static BlockedUserDto ToBlockedDto(this User user)
    {
        return new BlockedUserDto(
            user.Id,
            user.Username,
            user.Email,
            user.BlockCount
        );
    }
}
