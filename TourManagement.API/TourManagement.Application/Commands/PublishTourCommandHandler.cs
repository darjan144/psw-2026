using MediatR;
using TourManagement.Application.DTOs;
using TourManagement.Application.Mappings;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Application.Commands;

public class PublishTourCommandHandler : IRequestHandler<PublishTourCommand, TourDto>
{
    private readonly ITourRepository _tourRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PublishTourCommandHandler(ITourRepository tourRepository, IUnitOfWork unitOfWork)
    {
        _tourRepository = tourRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TourDto> Handle(PublishTourCommand command, CancellationToken cancellationToken)
    {
        var tour = await _tourRepository.GetByIdAsync(command.TourId, cancellationToken)
            ?? throw new InvalidOperationException("Tour not found.");

        if (tour.GuideId != command.GuideId)
            throw new InvalidOperationException("Guide is not authorized to publish this tour.");

        tour.Publish();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return tour.ToDto();
    }
}
