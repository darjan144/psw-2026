using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TourManagement.Application.EventHandlers;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;
using TourManagement.Domain.Events;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Tests.Application;

public class ProblemCreatedEventHandlerTests
{
    private readonly Mock<ITourRepository> _tourRepoMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly ProblemCreatedEventHandler _handler;

    public ProblemCreatedEventHandlerTests()
    {
        _tourRepoMock = new Mock<ITourRepository>();
        _userRepoMock = new Mock<IUserRepository>();
        _emailServiceMock = new Mock<IEmailService>();
        _handler = new ProblemCreatedEventHandler(
            _tourRepoMock.Object, _userRepoMock.Object, _emailServiceMock.Object,
            Mock.Of<ILogger<ProblemCreatedEventHandler>>());
    }

    [Fact]
    public async Task Handle_SendsNotificationToGuide()
    {
        var tour = new Tour("Fruska Gora", "Opis", TourDifficulty.Easy, Interest.Nature, 1500, DateTime.UtcNow.AddDays(7), 10);
        var guide = new User("vodic", "Petar", "Petrovic", "petar@test.com", "hash", UserRole.Guide);
        _tourRepoMock.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        _userRepoMock.Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(guide);

        var domainEvent = new ProblemCreatedEvent(1, 5, 99, "Problem sa turom");

        await _handler.Handle(domainEvent, CancellationToken.None);

        _emailServiceMock.Verify(e => e.SendProblemNotificationAsync(
            "petar@test.com", "Petar", "Problem sa turom", "Fruska Gora", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_TourNotFound_DoesNotSendEmail()
    {
        _tourRepoMock.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync((Tour?)null);

        var domainEvent = new ProblemCreatedEvent(1, 5, 99, "Problem sa turom");

        await _handler.Handle(domainEvent, CancellationToken.None);

        _emailServiceMock.Verify(e => e.SendProblemNotificationAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_GuideNotFound_DoesNotSendEmail()
    {
        var tour = new Tour("Tura", "Opis", TourDifficulty.Easy, Interest.Nature, 1500, DateTime.UtcNow.AddDays(7), 10);
        _tourRepoMock.Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        _userRepoMock.Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        var domainEvent = new ProblemCreatedEvent(1, 5, 99, "Problem sa turom");

        await _handler.Handle(domainEvent, CancellationToken.None);

        _emailServiceMock.Verify(e => e.SendProblemNotificationAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
