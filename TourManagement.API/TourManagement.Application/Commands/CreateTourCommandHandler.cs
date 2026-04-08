using MediatR;
using TourManagement.Application.DTOs;
using TourManagement.Application.Mappings;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Application.Commands;

public class CreateTourCommandHandler : IRequestHandler<CreateTourCommand, TourDto>
{
    private readonly ITourRepository _tourRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTourCommandHandler(ITourRepository tourRepository, IUnitOfWork unitOfWork)
    {
        _tourRepository = tourRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TourDto> Handle(CreateTourCommand command, CancellationToken cancellationToken)
    {
        var tour = new Tour(
            command.Name,
            command.Description,
            command.Difficulty,
            command.Category,
            command.Price,
            command.ScheduledDate,
            command.GuideId
        );

        await _tourRepository.AddAsync(tour, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return tour.ToDto();
    }
}
