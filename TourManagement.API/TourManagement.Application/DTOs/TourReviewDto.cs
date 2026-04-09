namespace TourManagement.Application.DTOs;

public record TourReviewDto(
    long Id,
    long TourId,
    long TouristId,
    int Rating,
    string? Comment,
    DateTime CreatedAt
);
