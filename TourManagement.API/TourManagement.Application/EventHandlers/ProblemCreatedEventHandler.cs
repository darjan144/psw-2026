using MediatR;
using Microsoft.Extensions.Logging;
using TourManagement.Application.Services;
using TourManagement.Domain.Events;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Application.EventHandlers;

public class ProblemCreatedEventHandler : INotificationHandler<ProblemCreatedEvent>
{
    private readonly ITourRepository _tourRepository;
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;
    private readonly ILogger<ProblemCreatedEventHandler> _logger;

    public ProblemCreatedEventHandler(ITourRepository tourRepository, IUserRepository userRepository, IEmailService emailService, ILogger<ProblemCreatedEventHandler> logger)
    {
        _tourRepository = tourRepository;
        _userRepository = userRepository;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Handle(ProblemCreatedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            var tour = await _tourRepository.GetByIdAsync(notification.TourId, cancellationToken);
            if (tour is null) return;

            var guide = await _userRepository.GetByIdAsync(tour.GuideId, cancellationToken);
            if (guide is null) return;

            await _emailService.SendProblemNotificationAsync(
                guide.Email,
                guide.FirstName,
                notification.ProblemTitle,
                tour.Name,
                cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send problem notification email for problem {Title}", notification.ProblemTitle);
        }
    }
}
