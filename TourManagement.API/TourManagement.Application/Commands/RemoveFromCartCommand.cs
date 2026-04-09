using MediatR;
using TourManagement.Application.DTOs;

namespace TourManagement.Application.Commands;

public record RemoveFromCartCommand(
    long TourId,
    long TouristId
) : IRequest<CartDto>;
