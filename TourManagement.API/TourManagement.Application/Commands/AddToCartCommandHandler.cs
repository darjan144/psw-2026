using MediatR;
using TourManagement.Application.DTOs;
using TourManagement.Application.Mappings;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Application.Commands;

public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, CartDto>
{
    private readonly IShoppingCartRepository _cartRepository;
    private readonly ITourRepository _tourRepository;
    private readonly ITourPurchaseRepository _purchaseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddToCartCommandHandler(IShoppingCartRepository cartRepository, ITourRepository tourRepository, ITourPurchaseRepository purchaseRepository, IUnitOfWork unitOfWork)
    {
        _cartRepository = cartRepository;
        _tourRepository = tourRepository;
        _purchaseRepository = purchaseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CartDto> Handle(AddToCartCommand command, CancellationToken cancellationToken)
    {
        var tour = await _tourRepository.GetByIdAsync(command.TourId, cancellationToken)
            ?? throw new InvalidOperationException("Tour not found.");

        if (tour.Status != TourStatus.Published)
            throw new InvalidOperationException("Only published tours can be added to cart.");

        if (await _purchaseRepository.HasPurchasedAsync(command.TouristId, command.TourId, cancellationToken))
            throw new InvalidOperationException("Tour is already purchased.");

        var cart = await _cartRepository.GetByTouristIdAsync(command.TouristId, cancellationToken);

        if (cart == null)
        {
            cart = new ShoppingCart(command.TouristId);
            await _cartRepository.AddAsync(cart, cancellationToken);
        }

        cart.AddItem(tour.Id, tour.Name, tour.Price);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return cart.ToDto();
    }
}
