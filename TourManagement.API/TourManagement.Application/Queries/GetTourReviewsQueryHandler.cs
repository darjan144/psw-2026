using MediatR;
using TourManagement.Application.DTOs;
using TourManagement.Application.Mappings;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Application.Queries;

public class GetTourReviewsQueryHandler : IRequestHandler<GetTourReviewsQuery, List<TourReviewDto>>
{
    private readonly ITourReviewRepository _reviewRepository;

    public GetTourReviewsQueryHandler(ITourReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<List<TourReviewDto>> Handle(GetTourReviewsQuery query, CancellationToken cancellationToken)
    {
        var reviews = await _reviewRepository.GetByTourIdAsync(query.TourId, cancellationToken);
        return reviews.Select(r => r.ToDto()).ToList();
    }
}
