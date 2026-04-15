using MediatR;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.Queries;

public record GetProblemHistoryQuery(long ProblemId) : IRequest<List<ProblemEventDto>>;
