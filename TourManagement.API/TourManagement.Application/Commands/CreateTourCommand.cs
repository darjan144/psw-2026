using MediatR;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Enums;

namespace TourManagement.Application.Commands;

public record CreateTourCommand(
    string Name,
    string Description,
    TourDifficulty Difficulty,
    Interest Category,
    double Price,
    DateTime ScheduledDate,
    long GuideId
) : IRequest<TourDto>;
