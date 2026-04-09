using TourManagement.Application.DTOs;
using TourManagement.Domain.Entities;

namespace TourManagement.Application.Mappings;

public static class TourReviewMappingExtensions
{
    public static TourReviewDto ToDto(this TourReview review)
    {
        return new TourReviewDto(
            review.Id,
            review.TourId,
            review.TouristId,
            review.Rating,
            review.Comment,
            review.CreatedAt
        );
    }
}
