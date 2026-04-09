using MediatR;
using TourManagement.Application.DTOs;
using TourManagement.Application.Mappings;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Application.Queries;

public class GetSeekingSubstituteToursQueryHandler : IRequestHandler<GetSeekingSubstituteToursQuery, List<TourDto>>
{
    private readonly ITourRepository _tourRepository;

    public GetSeekingSubstituteToursQueryHandler(ITourRepository tourRepository)
    {
        _tourRepository = tourRepository;
    }

    public async Task<List<TourDto>> Handle(GetSeekingSubstituteToursQuery query, CancellationToken cancellationToken)
    {
        var tours = await _tourRepository.GetSeekingSubstituteAsync(cancellationToken);
        return tours.Select(t => t.ToDto()).ToList();
    }
}
