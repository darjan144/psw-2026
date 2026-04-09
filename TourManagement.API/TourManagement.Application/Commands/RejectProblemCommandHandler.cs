using MediatR;
using TourManagement.Application.DTOs;
using TourManagement.Application.Mappings;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Application.Commands;

public class RejectProblemCommandHandler : IRequestHandler<RejectProblemCommand, ProblemDto>
{
    private readonly IProblemRepository _problemRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RejectProblemCommandHandler(IProblemRepository problemRepository, IUnitOfWork unitOfWork)
    {
        _problemRepository = problemRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProblemDto> Handle(RejectProblemCommand command, CancellationToken cancellationToken)
    {
        var problem = await _problemRepository.GetByIdAsync(command.ProblemId, cancellationToken)
            ?? throw new InvalidOperationException("Problem not found.");

        problem.Reject();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return problem.ToDto();
    }
}
