using MediatR;
using TourManagement.Application.DTOs;
using TourManagement.Application.Mappings;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Application.Commands;

public class MarkSeekingSubstituteCommandHandler : IRequestHandler<MarkSeekingSubstituteCommand, TourDto>
{
    private readonly ITourRepository _tourRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MarkSeekingSubstituteCommandHandler(ITourRepository tourRepository, IUnitOfWork unitOfWork)
    {
        _tourRepository = tourRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TourDto> Handle(MarkSeekingSubstituteCommand command, CancellationToken cancellationToken)
    {
        var tour = await _tourRepository.GetByIdAsync(command.TourId, cancellationToken)
            ?? throw new InvalidOperationException("Tour not found.");

        if (tour.GuideId != command.GuideId)
            throw new InvalidOperationException("Not authorized to modify this tour.");

        tour.MarkSeekingSubstitute();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return tour.ToDto();
    }
}
