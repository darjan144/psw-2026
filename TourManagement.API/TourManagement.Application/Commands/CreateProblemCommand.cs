using MediatR;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.Commands;

public record CreateProblemCommand(
    long TourId,
    long TouristId,
    string Title,
    string Description
) : IRequest<ProblemDto>;
