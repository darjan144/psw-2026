using MediatR;
using TourManagement.Application.DTOs;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Application.Queries;

public class GetTouristPurchasesQueryHandler : IRequestHandler<GetTouristPurchasesQuery, List<TouristPurchaseDto>>
{
    private readonly ITourPurchaseRepository _purchaseRepository;
    private readonly ITourRepository _tourRepository;

    public GetTouristPurchasesQueryHandler(ITourPurchaseRepository purchaseRepository, ITourRepository tourRepository)
    {
        _purchaseRepository = purchaseRepository;
        _tourRepository = tourRepository;
    }

    public async Task<List<TouristPurchaseDto>> Handle(GetTouristPurchasesQuery query, CancellationToken cancellationToken)
    {
        var purchases = await _purchaseRepository.GetByTouristIdAsync(query.TouristId, cancellationToken);
        var result = new List<TouristPurchaseDto>();

        foreach (var purchase in purchases)
        {
            var tour = await _tourRepository.GetByIdAsync(purchase.TourId, cancellationToken);
            if (tour is null) continue;

            result.Add(new TouristPurchaseDto(
                purchase.Id,
                tour.Id,
                tour.Name,
                tour.Category.ToString(),
                tour.ScheduledDate,
                purchase.PricePaid,
                purchase.PurchasedAt,
                tour.Status.ToString()
            ));
        }

        return result;
    }
}
