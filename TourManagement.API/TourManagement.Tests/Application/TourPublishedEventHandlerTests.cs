using FluentAssertions;
using Moq;
using TourManagement.Application.EventHandlers;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;
using TourManagement.Domain.Events;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Tests.Application;

public class TourPublishedEventHandlerTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly TourPublishedEventHandler _handler;

    public TourPublishedEventHandlerTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _emailServiceMock = new Mock<IEmailService>();
        _handler = new TourPublishedEventHandler(_userRepoMock.Object, _emailServiceMock.Object);
    }

    [Fact]
    public async Task Handle_SendsRecommendationToMatchingTourists()
    {
        var tourist = new User("jovan", "Jovan", "Jovic", "jovan@test.com", "hash", UserRole.Tourist);
        tourist.SetInterests(new List<Interest> { Interest.Nature });
        tourist.EnableRecommendations();

        _userRepoMock.Setup(r => r.GetTouristsWithInterestAsync(Interest.Nature, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User> { tourist });

        var domainEvent = new TourPublishedEvent(1, Interest.Nature, "Fruska Gora Hiking");

        await _handler.Handle(domainEvent, CancellationToken.None);

        _emailServiceMock.Verify(e => e.SendTourRecommendationAsync(
            "jovan@test.com", "Jovan", "Fruska Gora Hiking", "Nature", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NoMatchingTourists_SendsNoEmails()
    {
        _userRepoMock.Setup(r => r.GetTouristsWithInterestAsync(Interest.Art, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>());

        var domainEvent = new TourPublishedEvent(1, Interest.Art, "Art Gallery Tour");

        await _handler.Handle(domainEvent, CancellationToken.None);

        _emailServiceMock.Verify(e => e.SendTourRecommendationAsync(
            It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_MultipleTourists_SendsEmailToEach()
    {
        var tourist1 = new User("ana", "Ana", "Anic", "ana@test.com", "hash", UserRole.Tourist);
        tourist1.SetInterests(new List<Interest> { Interest.Sport });
        tourist1.EnableRecommendations();

        var tourist2 = new User("marko", "Marko", "Markovic", "marko@test.com", "hash", UserRole.Tourist);
        tourist2.SetInterests(new List<Interest> { Interest.Sport });
        tourist2.EnableRecommendations();

        _userRepoMock.Setup(r => r.GetTouristsWithInterestAsync(Interest.Sport, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User> { tourist1, tourist2 });

        var domainEvent = new TourPublishedEvent(1, Interest.Sport, "Football Tour");

        await _handler.Handle(domainEvent, CancellationToken.None);

        _emailServiceMock.Verify(e => e.SendTourRecommendationAsync(
            It.IsAny<string>(), It.IsAny<string>(), "Football Tour", "Sport", It.IsAny<CancellationToken>()), Times.Exactly(2));
    }
}
