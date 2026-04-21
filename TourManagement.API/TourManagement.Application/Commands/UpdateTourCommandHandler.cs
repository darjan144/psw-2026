using MediatR;
using TourManagement.Application.DTOs;
using TourManagement.Application.Mappings;
using TourManagement.Domain.Enums;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Application.Commands;

public class UpdateTourCommandHandler : IRequestHandler<UpdateTourCommand, TourDto>
{
    private readonly ITourRepository _tourRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTourCommandHandler(ITourRepository tourRepository, IUnitOfWork unitOfWork)
    {
        _tourRepository = tourRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TourDto> Handle(UpdateTourCommand command, CancellationToken cancellationToken)
    {
        var tour = await _tourRepository.GetByIdAsync(command.TourId, cancellationToken)
            ?? throw new InvalidOperationException("Tour not found.");

        if (tour.GuideId != command.GuideId)
            throw new InvalidOperationException("Guide is not authorized to update this tour.");

        tour.Rename(command.Name);
        tour.UpdateDescription(command.Description);
        tour.SetDifficulty(Enum.Parse<TourDifficulty>(command.Difficulty));
        tour.SetCategory(Enum.Parse<Interest>(command.Category));
        tour.SetPrice(command.Price);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return tour.ToDto();
    }
}
