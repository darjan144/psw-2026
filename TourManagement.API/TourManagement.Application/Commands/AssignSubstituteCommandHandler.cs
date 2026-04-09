using MediatR;
using TourManagement.Application.DTOs;
using TourManagement.Application.Mappings;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Application.Commands;

public class AssignSubstituteCommandHandler : IRequestHandler<AssignSubstituteCommand, TourDto>
{
    private readonly ITourRepository _tourRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignSubstituteCommandHandler(ITourRepository tourRepository, IUnitOfWork unitOfWork)
    {
        _tourRepository = tourRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TourDto> Handle(AssignSubstituteCommand command, CancellationToken cancellationToken)
    {
        var tour = await _tourRepository.GetByIdAsync(command.TourId, cancellationToken)
            ?? throw new InvalidOperationException("Tour not found.");

        if (tour.GuideId == command.NewGuideId)
            throw new InvalidOperationException("Cannot substitute your own tour.");

        var hasConflict = await _tourRepository.HasTourOnDateAsync(command.NewGuideId, tour.ScheduledDate, cancellationToken);
        if (hasConflict)
            throw new InvalidOperationException("Guide already has a tour on this date.");

        tour.AssignSubstitute(command.NewGuideId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return tour.ToDto();
    }
}
