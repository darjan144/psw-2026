using MediatR;
using TourManagement.Application.DTOs;
using TourManagement.Application.Mappings;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Application.Commands;

public class CreateProblemCommandHandler : IRequestHandler<CreateProblemCommand, ProblemDto>
{
    private readonly IProblemRepository _problemRepository;
    private readonly ITourRepository _tourRepository;
    private readonly ITourPurchaseRepository _purchaseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProblemCommandHandler(
        IProblemRepository problemRepository,
        ITourRepository tourRepository,
        ITourPurchaseRepository purchaseRepository,
        IUnitOfWork unitOfWork)
    {
        _problemRepository = problemRepository;
        _tourRepository = tourRepository;
        _purchaseRepository = purchaseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProblemDto> Handle(CreateProblemCommand command, CancellationToken cancellationToken)
    {
        var tour = await _tourRepository.GetByIdAsync(command.TourId, cancellationToken)
            ?? throw new InvalidOperationException("Tour not found.");

        var hasPurchased = await _purchaseRepository.HasPurchasedAsync(command.TouristId, command.TourId, cancellationToken);
        if (!hasPurchased)
            throw new InvalidOperationException("Tourist has not purchased this tour.");

        var problem = new Problem(command.Title, command.Description, command.TourId, command.TouristId);

        await _problemRepository.AddAsync(problem, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return problem.ToDto();
    }
}
