using MediatR;
using TourManagement.Application.DTOs;
using TourManagement.Application.Mappings;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces;
using TourManagement.Domain.ValueObjects;

namespace TourManagement.Application.Commands;

public class AddKeyPointCommandHandler : IRequestHandler<AddKeyPointCommand, KeyPointDto>
{
    private readonly ITourRepository _tourRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddKeyPointCommandHandler(ITourRepository tourRepository, IUnitOfWork unitOfWork)
    {
        _tourRepository = tourRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<KeyPointDto> Handle(AddKeyPointCommand command, CancellationToken cancellationToken)
    {
        var tour = await _tourRepository.GetByIdAsync(command.TourId, cancellationToken)
            ?? throw new InvalidOperationException("Tour not found.");

        if (tour.GuideId != command.GuideId)
            throw new InvalidOperationException("Guide is not authorized to modify this tour.");

        var keyPoint = new KeyPoint(
            command.Name,
            command.Description,
            new Coordinate(command.Latitude, command.Longitude),
            command.ImageUrl,
            tour.KeyPoints.Count + 1
        );

        tour.AddKeyPoint(keyPoint);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return keyPoint.ToDto();
    }
}
