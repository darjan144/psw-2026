using MediatR;
using TourManagement.Application.DTOs;
using TourManagement.Application.Mappings;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Application.Queries;

public class GetGuideToursQueryHandler : IRequestHandler<GetGuideToursQuery, List<TourDto>>
{
    private readonly ITourRepository _tourRepository;

    public GetGuideToursQueryHandler(ITourRepository tourRepository)
    {
        _tourRepository = tourRepository;
    }

    public async Task<List<TourDto>> Handle(GetGuideToursQuery query, CancellationToken cancellationToken)
    {
        var tours = await _tourRepository.GetByGuideIdAsync(query.GuideId, cancellationToken);

        var sorted = query.SortAscending
            ? tours.OrderBy(t => t.ScheduledDate).ToList()
            : tours.OrderByDescending(t => t.ScheduledDate).ToList();

        return sorted.Select(t => t.ToDto()).ToList();
    }
}
