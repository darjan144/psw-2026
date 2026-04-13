namespace TourManagement.Application.Services;

public interface IEmailService
{
    Task SendPurchaseConfirmationAsync(string toEmail, string touristName, List<PurchasedTourInfo> tours, double totalPaid, CancellationToken cancellationToken = default);
    Task SendTourReminderAsync(string toEmail, string touristName, string tourName, DateTime scheduledDate, CancellationToken cancellationToken = default);
    Task SendTourRecommendationAsync(string toEmail, string touristName, string tourName, string category, CancellationToken cancellationToken = default);
    Task SendProblemNotificationAsync(string toEmail, string guideName, string problemTitle, string tourName, CancellationToken cancellationToken = default);
}

public record PurchasedTourInfo(string TourName, double Price, DateTime ScheduledDate);
