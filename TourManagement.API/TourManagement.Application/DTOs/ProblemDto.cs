namespace TourManagement.Application.DTOs;

public record ProblemDto(
    long Id,
    string Title,
    string Description,
    string Status,
    long TourId,
    long TouristId,
    DateTime CreatedAt
);
