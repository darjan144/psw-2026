using MediatR;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.Commands;

public record PurchaseToursCommand(
    long TouristId,
    bool UseBonusPoints = false
) : IRequest<List<PurchaseDto>>;
