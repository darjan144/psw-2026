using MediatR;
using TourManagement.Application.DTOs;
using TourManagement.Application.Mappings;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Application.Queries;

public class GetPublishedToursQueryHandler : IRequestHandler<GetPublishedToursQuery, List<TourDto>>
{
    private readonly ITourRepository _tourRepository;

    public GetPublishedToursQueryHandler(ITourRepository tourRepository)
    {
        _tourRepository = tourRepository;
    }

    public async Task<List<TourDto>> Handle(GetPublishedToursQuery query, CancellationToken cancellationToken)
    {
        var tours = await _tourRepository.GetPublishedAsync(cancellationToken);
        return tours.Select(t => t.ToDto()).ToList();
    }
}
