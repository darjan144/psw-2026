using TourManagement.Application.DTOs;
using TourManagement.Domain.Entities;

namespace TourManagement.Application.Mappings;

public static class TourMappingExtensions
{
    public static TourDto ToDto(this Tour tour)
    {
        return new TourDto(
            tour.Id,
            tour.Name,
            tour.Description,
            tour.Difficulty.ToString(),
            tour.Category.ToString(),
            tour.Price,
            tour.Status.ToString(),
            tour.ScheduledDate,
            tour.PublishedDate,
            tour.GuideId,
            tour.SeekingSubstitute,
            tour.KeyPoints.Select(kp => kp.ToDto()).ToList()
        );
    }

    public static KeyPointDto ToDto(this KeyPoint kp)
    {
        return new KeyPointDto(
            kp.Id,
            kp.Name,
            kp.Description,
            kp.Position.Latitude,
            kp.Position.Longitude,
            kp.ImageUrl,
            kp.Order
        );
    }
}
