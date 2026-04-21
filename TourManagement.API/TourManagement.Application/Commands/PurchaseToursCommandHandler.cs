using MediatR;
using TourManagement.Application.DTOs;
using TourManagement.Application.Mappings;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Application.Commands;

public class PurchaseToursCommandHandler : IRequestHandler<PurchaseToursCommand, List<PurchaseDto>>
{
    private readonly IShoppingCartRepository _cartRepository;
    private readonly ITourPurchaseRepository _purchaseRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITourRepository _tourRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;

    public PurchaseToursCommandHandler(
        IShoppingCartRepository cartRepository,
        ITourPurchaseRepository purchaseRepository,
        IUserRepository userRepository,
        ITourRepository tourRepository,
        IUnitOfWork unitOfWork,
        IEmailService emailService)
    {
        _cartRepository = cartRepository;
        _purchaseRepository = purchaseRepository;
        _userRepository = userRepository;
        _tourRepository = tourRepository;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
    }

    public async Task<List<PurchaseDto>> Handle(PurchaseToursCommand command, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(command.TouristId, cancellationToken)
            ?? throw new InvalidOperationException("User not found.");

        var cart = await _cartRepository.GetByTouristIdAsync(command.TouristId, cancellationToken)
            ?? throw new InvalidOperationException("Cart not found.");

        if (!cart.Items.Any())
            throw new InvalidOperationException("Cart is empty.");

        var totalPrice = cart.GetTotalPrice();
        var discount = 0.0;

        if (command.UseBonusPoints)
        {
            var used = user.UseBonusPoints((int)totalPrice);
            discount = used;
        }

        var pricePerItem = cart.Items.Count == 1
            ? new[] { totalPrice - discount }
            : DistributeDiscount(cart.Items.ToList(), discount);

        var purchases = new List<TourPurchase>();
        var tourInfos = new List<PurchasedTourInfo>();
        var index = 0;

        foreach (var item in cart.Items)
        {
            var alreadyPurchased = await _purchaseRepository.HasPurchasedAsync(command.TouristId, item.TourId, cancellationToken);
            if (alreadyPurchased)
            {
                index++;
                continue;
            }

            var pricePaid = pricePerItem[index];
            var purchase = new TourPurchase(command.TouristId, item.TourId, pricePaid);
            await _purchaseRepository.AddAsync(purchase, cancellationToken);
            purchases.Add(purchase);

            var tour = await _tourRepository.GetByIdAsync(item.TourId, cancellationToken);
            tourInfos.Add(new PurchasedTourInfo(item.TourName, pricePaid, tour?.ScheduledDate ?? DateTime.UtcNow));
            index++;
        }

        cart.Clear();
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        try
        {
            await _emailService.SendPurchaseConfirmationAsync(
                user.Email, user.FirstName, tourInfos, totalPrice - discount, cancellationToken);
        }
        catch
        {
            // Email failure should not fail the purchase
        }

        return purchases.Select(p => p.ToDto()).ToList();
    }

    private static double[] DistributeDiscount(List<CartItem> items, double discount)
    {
        var total = items.Sum(i => i.Price);
        var prices = new double[items.Count];

        for (int i = 0; i < items.Count; i++)
        {
            var proportion = items[i].Price / total;
            prices[i] = items[i].Price - (discount * proportion);
        }

        return prices;
    }
}
