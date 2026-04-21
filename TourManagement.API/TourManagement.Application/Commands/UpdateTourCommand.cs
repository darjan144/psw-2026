using MediatR;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.Commands;

public record UpdateTourCommand(
    long TourId,
    long GuideId,
    string Name,
    string Description,
    string Difficulty,
    string Category,
    double Price
) : IRequest<TourDto>;
