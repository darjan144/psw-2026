using MediatR;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Application.Commands;

public class CancelTourCommandHandler : IRequestHandler<CancelTourCommand>
{
    private readonly ITourRepository _tourRepository;
    private readonly ITourPurchaseRepository _purchaseRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelTourCommandHandler(
        ITourRepository tourRepository,
        ITourPurchaseRepository purchaseRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _tourRepository = tourRepository;
        _purchaseRepository = purchaseRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(CancelTourCommand command, CancellationToken cancellationToken)
    {
        var tour = await _tourRepository.GetByIdAsync(command.TourId, cancellationToken)
            ?? throw new InvalidOperationException("Tour not found.");

        if (tour.GuideId != command.GuideId)
            throw new InvalidOperationException("Guide is not authorized to cancel this tour.");

        tour.Cancel();

        var purchases = await _purchaseRepository.GetByTourIdAsync(command.TourId, cancellationToken);
        foreach (var purchase in purchases)
        {
            var tourist = await _userRepository.GetByIdAsync(purchase.TouristId, cancellationToken);
            tourist?.AddBonusPoints((int)tour.Price);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
