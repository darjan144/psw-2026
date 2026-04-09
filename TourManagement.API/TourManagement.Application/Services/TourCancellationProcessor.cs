using TourManagement.Domain.Interfaces;

namespace TourManagement.Application.Services;

public class TourCancellationProcessor
{
    private readonly ITourRepository _tourRepository;
    private readonly ITourPurchaseRepository _purchaseRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TourCancellationProcessor(
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

    public async Task ProcessCancellationsAsync(CancellationToken cancellationToken)
    {
        var seekingSubstitute = await _tourRepository.GetSeekingSubstituteAsync(cancellationToken);
        var cutoff = DateTime.UtcNow.AddHours(24);

        foreach (var tour in seekingSubstitute)
        {
            if (tour.ScheduledDate > cutoff)
                continue;

            tour.Cancel();

            var purchases = await _purchaseRepository.GetByTourIdAsync(tour.Id, cancellationToken);
            foreach (var purchase in purchases)
            {
                var tourist = await _userRepository.GetByIdAsync(purchase.TouristId, cancellationToken);
                tourist?.AddBonusPoints((int)tour.Price);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
