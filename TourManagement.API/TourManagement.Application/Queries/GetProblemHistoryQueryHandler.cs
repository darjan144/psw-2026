using MediatR;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Application.Queries;

public class GetProblemHistoryQueryHandler : IRequestHandler<GetProblemHistoryQuery, List<ProblemEventDto>>
{
    private readonly IProblemRepository _problemRepository;

    public GetProblemHistoryQueryHandler(IProblemRepository problemRepository)
    {
        _problemRepository = problemRepository;
    }

    public async Task<List<ProblemEventDto>> Handle(GetProblemHistoryQuery query, CancellationToken cancellationToken)
    {
        var problem = await _problemRepository.GetByIdAsync(query.ProblemId, cancellationToken)
            ?? throw new InvalidOperationException("Problem not found.");

        return problem.Events
            .OrderBy(e => e.SequenceNumber)
            .Select(e => new ProblemEventDto(
                e.Id,
                e.SequenceNumber,
                e.GetType().Name.Replace("Problem", "").Replace("StateEvent", ""),
                e.OccurredAt,
                e.CausedByUserId))
            .ToList();
    }
}
