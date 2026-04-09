using MediatR;
using TourManagement.Application.DTOs;
using TourManagement.Application.Mappings;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Application.Commands;

public class CreateTourReviewCommandHandler : IRequestHandler<CreateTourReviewCommand, TourReviewDto>
{
    private readonly ITourReviewRepository _reviewRepository;
    private readonly ITourRepository _tourRepository;
    private readonly ITourPurchaseRepository _purchaseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTourReviewCommandHandler(
        ITourReviewRepository reviewRepository,
        ITourRepository tourRepository,
        ITourPurchaseRepository purchaseRepository,
        IUnitOfWork unitOfWork)
    {
        _reviewRepository = reviewRepository;
        _tourRepository = tourRepository;
        _purchaseRepository = purchaseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TourReviewDto> Handle(CreateTourReviewCommand command, CancellationToken cancellationToken)
    {
        var tour = await _tourRepository.GetByIdAsync(command.TourId, cancellationToken)
            ?? throw new InvalidOperationException("Tour not found.");

        var hasPurchased = await _purchaseRepository.HasPurchasedAsync(command.TouristId, command.TourId, cancellationToken);
        if (!hasPurchased)
            throw new InvalidOperationException("Tourist has not purchased this tour.");

        var existing = await _reviewRepository.GetByTouristAndTourAsync(command.TouristId, command.TourId, cancellationToken);
        if (existing != null)
            throw new InvalidOperationException("Tourist has already reviewed this tour.");

        var review = new TourReview(command.TourId, command.TouristId, command.Rating, command.Comment, tour.ScheduledDate);

        await _reviewRepository.AddAsync(review, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return review.ToDto();
    }
}
