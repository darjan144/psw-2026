using MediatR;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.Commands;

public record ResolveProblemCommand(long ProblemId, long GuideId) : IRequest<ProblemDto>;
