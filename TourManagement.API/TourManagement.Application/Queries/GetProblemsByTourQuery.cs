using MediatR;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.Queries;

public record GetProblemsByTourQuery(long TourId) : IRequest<List<ProblemDto>>;
