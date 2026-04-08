using TourManagement.Domain.Enums;

namespace TourManagement.Application.DTOs;

public record TourDto(
    long Id,
    string Name,
    string Description,
    string Difficulty,
    string Category,
    double Price,
    string Status,
    DateTime ScheduledDate,
    DateTime? PublishedDate,
    long GuideId,
    bool SeekingSubstitute,
    List<KeyPointDto> KeyPoints
);

public record KeyPointDto(
    long Id,
    string Name,
    string Description,
    double Latitude,
    double Longitude,
    string ImageUrl,
    int Order
);
