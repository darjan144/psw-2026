using MediatR;
using TourManagement.Application.DTOs;
using TourManagement.Application.Mappings;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Application.Queries;

public class GetProblemsByTouristQueryHandler : IRequestHandler<GetProblemsByTouristQuery, List<ProblemDto>>
{
    private readonly IProblemRepository _problemRepository;

    public GetProblemsByTouristQueryHandler(IProblemRepository problemRepository)
    {
        _problemRepository = problemRepository;
    }

    public async Task<List<ProblemDto>> Handle(GetProblemsByTouristQuery query, CancellationToken cancellationToken)
    {
        var problems = await _problemRepository.GetByTouristIdAsync(query.TouristId, cancellationToken);
        return problems.Select(p => p.ToDto()).ToList();
    }
}
