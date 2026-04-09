using MediatR;
using TourManagement.Application.DTOs;
using TourManagement.Application.Mappings;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Application.Commands;

public class ReturnProblemToGuideCommandHandler : IRequestHandler<ReturnProblemToGuideCommand, ProblemDto>
{
    private readonly IProblemRepository _problemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReturnProblemToGuideCommandHandler(IProblemRepository problemRepository, IUnitOfWork unitOfWork)
    {
        _problemRepository = problemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProblemDto> Handle(ReturnProblemToGuideCommand command, CancellationToken cancellationToken)
    {
        var problem = await _problemRepository.GetByIdAsync(command.ProblemId, cancellationToken)
            ?? throw new InvalidOperationException("Problem not found.");

        problem.ReturnToGuide();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return problem.ToDto();
    }
}
