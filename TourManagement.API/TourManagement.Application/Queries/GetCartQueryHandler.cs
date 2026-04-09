using MediatR;
using TourManagement.Application.DTOs;
using TourManagement.Application.Mappings;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Application.Queries;

public class GetCartQueryHandler : IRequestHandler<GetCartQuery, CartDto>
{
    private readonly IShoppingCartRepository _cartRepository;

    public GetCartQueryHandler(IShoppingCartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task<CartDto> Handle(GetCartQuery query, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetByTouristIdAsync(query.TouristId, cancellationToken);

        if (cart == null)
            return new CartDto(0, query.TouristId, new List<CartItemDto>(), 0);

        return cart.ToDto();
    }
}
