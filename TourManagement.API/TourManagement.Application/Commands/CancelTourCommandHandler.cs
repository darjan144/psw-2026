using MediatR;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Application.Commands;

public class CancelTourCommandHandler : IRequestHandler<CancelTourCommand>
{
    private readonly ITourRepository _tourRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelTourCommandHandler(ITourRepository tourRepository, IUnitOfWork unitOfWork)
    {
        _tourRepository = tourRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(CancelTourCommand command, CancellationToken cancellationToken)
    {
        var tour = await _tourRepository.GetByIdAsync(command.TourId, cancellationToken)
            ?? throw new InvalidOperationException("Tour not found.");

        if (tour.GuideId != command.GuideId)
            throw new InvalidOperationException("Guide is not authorized to cancel this tour.");

        tour.Cancel();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
