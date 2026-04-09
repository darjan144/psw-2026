using MediatR;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.Queries;

public record GetTourReviewsQuery(long TourId) : IRequest<List<TourReviewDto>>;
