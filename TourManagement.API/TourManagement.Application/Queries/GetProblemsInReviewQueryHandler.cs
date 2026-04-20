using MediatR;
using TourManagement.Application.DTOs;
using TourManagement.Application.Mappings;
using TourManagement.Domain.Enums;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Application.Queries;

public class GetProblemsInReviewQueryHandler : IRequestHandler<GetProblemsInReviewQuery, List<ProblemDto>>
{
    private readonly IProblemRepository _problemRepository;

    public GetProblemsInReviewQueryHandler(IProblemRepository problemRepository)
    {
        _problemRepository = problemRepository;
    }

    public async Task<List<ProblemDto>> Handle(GetProblemsInReviewQuery query, CancellationToken cancellationToken)
    {
        var problems = await _problemRepository.GetByStatusAsync(ProblemStatus.InReview, cancellationToken);
        return problems.Select(p => p.ToDto()).ToList();
    }
}
