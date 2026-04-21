using MediatR;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.Queries;

public record GetTouristPurchasesQuery(long TouristId) : IRequest<List<TouristPurchaseDto>>;
