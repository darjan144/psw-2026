using FluentAssertions;
using Moq;
using TourManagement.Application.Services;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Tests.Application;

public class TourReminderProcessorTests
{
    private readonly Mock<ITourPurchaseRepository> _purchaseRepoMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly TourReminderProcessor _processor;

    public TourReminderProcessorTests()
    {
        _purchaseRepoMock = new Mock<ITourPurchaseRepository>();
        _emailServiceMock = new Mock<IEmailService>();
        _processor = new TourReminderProcessor(_purchaseRepoMock.Object, _emailServiceMock.Object);
    }

    [Fact]
    public async Task ProcessRemindersAsync_WithUpcomingTours_SendsEmails()
    {
        var upcoming = new List<UpcomingTourPurchaseInfo>
        {
            new("jovan@test.com", "Jovan", "Fruska Gora Hiking", DateTime.UtcNow.AddHours(36)),
            new("ana@test.com", "Ana", "City Walk", DateTime.UtcNow.AddHours(24))
        };
        _purchaseRepoMock.Setup(r => r.GetPurchasesForUpcomingToursAsync(
            It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(upcoming);

        await _processor.ProcessRemindersAsync(CancellationToken.None);

        _emailServiceMock.Verify(e => e.SendTourReminderAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }

    [Fact]
    public async Task ProcessRemindersAsync_NoUpcomingTours_SendsNoEmails()
    {
        _purchaseRepoMock.Setup(r => r.GetPurchasesForUpcomingToursAsync(
            It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<UpcomingTourPurchaseInfo>());

        await _processor.ProcessRemindersAsync(CancellationToken.None);

        _emailServiceMock.Verify(e => e.SendTourReminderAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ProcessRemindersAsync_SendsCorrectDataToEmailService()
    {
        var scheduledDate = DateTime.UtcNow.AddHours(40);
        var upcoming = new List<UpcomingTourPurchaseInfo>
        {
            new("marko@test.com", "Marko", "Nature Walk", scheduledDate)
        };
        _purchaseRepoMock.Setup(r => r.GetPurchasesForUpcomingToursAsync(
            It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(upcoming);

        await _processor.ProcessRemindersAsync(CancellationToken.None);

        _emailServiceMock.Verify(e => e.SendTourReminderAsync(
            "marko@test.com", "Marko", "Nature Walk", scheduledDate, It.IsAny<CancellationToken>()), Times.Once);
    }
}
