using MediatR;
using TourManagement.Application.DTOs;
using TourManagement.Application.Mappings;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Application.Commands;

public class ResolveProblemCommandHandler : IRequestHandler<ResolveProblemCommand, ProblemDto>
{
    private readonly IProblemRepository _problemRepository;
    private readonly ITourRepository _tourRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ResolveProblemCommandHandler(IProblemRepository problemRepository, ITourRepository tourRepository, IUnitOfWork unitOfWork)
    {
        _problemRepository = problemRepository;
        _tourRepository = tourRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProblemDto> Handle(ResolveProblemCommand command, CancellationToken cancellationToken)
    {
        var problem = await _problemRepository.GetByIdAsync(command.ProblemId, cancellationToken)
            ?? throw new InvalidOperationException("Problem not found.");

        var tour = await _tourRepository.GetByIdAsync(problem.TourId, cancellationToken)
            ?? throw new InvalidOperationException("Tour not found.");

        if (tour.GuideId != command.GuideId)
            throw new InvalidOperationException("Not authorized to resolve this problem.");

        problem.Resolve();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return problem.ToDto();
    }
}
