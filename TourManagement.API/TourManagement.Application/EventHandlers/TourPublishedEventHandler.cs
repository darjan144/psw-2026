using MediatR;
using TourManagement.Application.Services;
using TourManagement.Domain.Events;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Application.EventHandlers;

public class TourPublishedEventHandler : INotificationHandler<TourPublishedEvent>
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;

    public TourPublishedEventHandler(IUserRepository userRepository, IEmailService emailService)
    {
        _userRepository = userRepository;
        _emailService = emailService;
    }

    public async Task Handle(TourPublishedEvent notification, CancellationToken cancellationToken)
    {
        var tourists = await _userRepository.GetTouristsWithInterestAsync(notification.Category, cancellationToken);

        foreach (var tourist in tourists)
        {
            await _emailService.SendTourRecommendationAsync(
                tourist.Email,
                tourist.FirstName,
                notification.TourName,
                notification.Category.ToString(),
                cancellationToken);
        }
    }
}
