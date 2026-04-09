using MediatR;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.Commands;

public record RejectProblemCommand(long ProblemId) : IRequest<ProblemDto>;
