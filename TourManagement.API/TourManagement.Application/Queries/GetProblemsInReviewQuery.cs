using MediatR;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.Queries;

public record GetProblemsInReviewQuery() : IRequest<List<ProblemDto>>;
