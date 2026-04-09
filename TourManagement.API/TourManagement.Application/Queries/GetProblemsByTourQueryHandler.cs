using MediatR;
using TourManagement.Application.DTOs;
using TourManagement.Application.Mappings;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Application.Queries;

public class GetProblemsByTourQueryHandler : IRequestHandler<GetProblemsByTourQuery, List<ProblemDto>>
{
    private readonly IProblemRepository _problemRepository;

    public GetProblemsByTourQueryHandler(IProblemRepository problemRepository)
    {
        _problemRepository = problemRepository;
    }

    public async Task<List<ProblemDto>> Handle(GetProblemsByTourQuery query, CancellationToken cancellationToken)
    {
        var problems = await _problemRepository.GetByTourIdAsync(query.TourId, cancellationToken);
        return problems.Select(p => p.ToDto()).ToList();
    }
}
