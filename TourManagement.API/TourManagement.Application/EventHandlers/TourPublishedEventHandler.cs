using MediatR;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Domain.Events;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Application.EventHandlers;

public class TourPublishedEventHandler : INotificationHandler<TourPublishedEvent>
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<TourPublishedEventHandler> _logger;

    public TourPublishedEventHandler(IUserRepository userRepository, IEmailService emailService, ILogger<TourPublishedEventHandler> logger)
    {
        _userRepository = userRepository;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Handle(TourPublishedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var tourists = await _userRepository.GetTouristsWithInterestAsync(notification.Category, cancellationToken);

            foreach (var tourist in tourists)
            {
                try
                {
                    await _emailService.SendTourRecommendationAsync(
                        tourist.Email,
                        tourist.FirstName,
                        notification.TourName,
                        notification.Category.ToString(),
                        cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to send recommendation email to {Email}", tourist.Email);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process tour published event for tour {TourName}", notification.TourName);
        }
    }
}
