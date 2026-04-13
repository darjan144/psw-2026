using TourManagement.Domain.Interfaces;

namespace TourManagement.Application.Services;

public class TourReminderProcessor
{
    private readonly ITourPurchaseRepository _purchaseRepository;
    private readonly IEmailService _emailService;

    public TourReminderProcessor(ITourPurchaseRepository purchaseRepository, IEmailService emailService)
    {
        _purchaseRepository = purchaseRepository;
        _emailService = emailService;
    }

    public async Task ProcessRemindersAsync(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var cutoff = now.AddHours(48);

        var upcoming = await _purchaseRepository.GetPurchasesForUpcomingToursAsync(now, cutoff, cancellationToken);

        foreach (var info in upcoming)
        {
            await _emailService.SendTourReminderAsync(
                info.TouristEmail,
                info.TouristName,
                info.TourName,
                info.ScheduledDate,
                cancellationToken);
        }
    }
}
