using MediatR;
using TourManagement.Application.DTOs;
using TourManagement.Application.Mappings;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Application.Commands;

public class RemoveFromCartCommandHandler : IRequestHandler<RemoveFromCartCommand, CartDto>
{
    private readonly IShoppingCartRepository _cartRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveFromCartCommandHandler(IShoppingCartRepository cartRepository, IUnitOfWork unitOfWork)
    {
        _cartRepository = cartRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CartDto> Handle(RemoveFromCartCommand command, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetByTouristIdAsync(command.TouristId, cancellationToken)
            ?? throw new InvalidOperationException("Cart not found.");

        cart.RemoveItem(command.TourId);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return cart.ToDto();
    }
}
