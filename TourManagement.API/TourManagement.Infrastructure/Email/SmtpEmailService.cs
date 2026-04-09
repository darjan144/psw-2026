using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using TourManagement.Application.Services;

namespace TourManagement.Infrastructure.Email;

public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public SmtpEmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendPurchaseConfirmationAsync(string toEmail, string touristName, List<PurchasedTourInfo> tours, double totalPaid, CancellationToken cancellationToken = default)
    {
        var tourLines = string.Join("\n", tours.Select(t => $"- {t.TourName} | {t.Price:F2} RSD | {t.ScheduledDate:dd.MM.yyyy}"));
        var body = $"Pozdrav {touristName},\n\nUspešno ste kupili sledeće ture:\n{tourLines}\n\nUkupno plaćeno: {totalPaid:F2} RSD\n\nHvala na kupovini!";

        await SendEmailAsync(toEmail, "Potvrda kupovine tura", body, cancellationToken);
    }

    public async Task SendTourReminderAsync(string toEmail, string touristName, string tourName, DateTime scheduledDate, CancellationToken cancellationToken = default)
    {
        var body = $"Pozdrav {touristName},\n\nPodsetnik: Vaša tura \"{tourName}\" je zakazana za {scheduledDate:dd.MM.yyyy HH:mm}.\n\nNe zaboravite da se pripremite!";

        await SendEmailAsync(toEmail, $"Podsetnik za turu: {tourName}", body, cancellationToken);
    }

    public async Task SendTourRecommendationAsync(string toEmail, string touristName, string tourName, string category, CancellationToken cancellationToken = default)
    {
        var body = $"Pozdrav {touristName},\n\nNova tura koja odgovara vašim interesovanjima je objavljena!\n\nTura: {tourName}\nKategorija: {category}\n\nPogledajte ponudu na našem sajtu!";

        await SendEmailAsync(toEmail, $"Preporuka ture: {tourName}", body, cancellationToken);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string body, CancellationToken cancellationToken)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(
            _configuration["Email:SenderName"] ?? "Tour Management",
            _configuration["Email:SenderAddress"] ?? "noreply@tourmanagement.com"));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = subject;
        message.Body = new TextPart("plain") { Text = body };

        using var client = new SmtpClient();
        await client.ConnectAsync(
            _configuration["Email:SmtpHost"] ?? "localhost",
            int.Parse(_configuration["Email:SmtpPort"] ?? "587"),
            MailKit.Security.SecureSocketOptions.StartTls,
            cancellationToken);

        var username = _configuration["Email:Username"];
        if (!string.IsNullOrEmpty(username))
            await client.AuthenticateAsync(username, _configuration["Email:Password"], cancellationToken);

        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}
