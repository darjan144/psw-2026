using MediatR;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.Queries;

public record GetProblemsByTouristQuery(long TouristId) : IRequest<List<ProblemDto>>;
