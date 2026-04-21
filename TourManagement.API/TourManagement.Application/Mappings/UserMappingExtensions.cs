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

    public static ProfileDto ToProfileDto(this User user)
    {
        return new ProfileDto(
            user.Id,
            user.Username,
            user.Email,
            user.Interests.Select(i => i.ToString()).ToList(),
            user.RecommendationsEnabled,
            user.BonusPoints
        );
    }
}
