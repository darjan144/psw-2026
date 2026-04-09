using MediatR;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.Commands;

public record CreateTourReviewCommand(
    long TourId,
    long TouristId,
    int Rating,
    string? Comment
) : IRequest<TourReviewDto>;
