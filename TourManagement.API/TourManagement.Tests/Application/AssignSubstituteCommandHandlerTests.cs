using FluentAssertions;
using Moq;
using TourManagement.Application.Commands;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;
using TourManagement.Domain.Interfaces;
using TourManagement.Domain.ValueObjects;

namespace TourManagement.Tests.Application;

public class AssignSubstituteCommandHandlerTests
{
    private readonly Mock<ITourRepository> _tourRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly AssignSubstituteCommandHandler _handler;

    public AssignSubstituteCommandHandlerTests()
    {
        _tourRepoMock = new Mock<ITourRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new AssignSubstituteCommandHandler(_tourRepoMock.Object, _unitOfWorkMock.Object);
    }

    private Tour CreateSeekingSubstituteTour(long guideId = 1)
    {
        var tour = new Tour("Tura", "Opis", TourDifficulty.Easy,
            Interest.Nature, 1500, DateTime.UtcNow.AddDays(7), guideId);
        tour.AddKeyPoint(new KeyPoint("KT1", "Opis", new Coordinate(45.25, 19.85), "img.jpg", 1));
        tour.AddKeyPoint(new KeyPoint("KT2", "Opis", new Coordinate(45.26, 19.86), "img2.jpg", 2));
        tour.Publish();
        tour.MarkSeekingSubstitute();
        return tour;
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldAssignNewGuide()
    {
        var tour = CreateSeekingSubstituteTour(guideId: 1);
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        _tourRepoMock.Setup(r => r.HasTourOnDateAsync(5, tour.ScheduledDate, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        var command = new AssignSubstituteCommand(TourId: 1, NewGuideId: 5);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.GuideId.Should().Be(5);
        result.SeekingSubstitute.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_GuideHasDateConflict_ShouldThrow()
    {
        var tour = CreateSeekingSubstituteTour(guideId: 1);
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        _tourRepoMock.Setup(r => r.HasTourOnDateAsync(5, tour.ScheduledDate, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var command = new AssignSubstituteCommand(TourId: 1, NewGuideId: 5);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*already has a tour*");
    }

    [Fact]
    public async Task Handle_SameGuide_ShouldThrow()
    {
        var tour = CreateSeekingSubstituteTour(guideId: 1);
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        var command = new AssignSubstituteCommand(TourId: 1, NewGuideId: 1);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*own tour*");
    }

    [Fact]
    public async Task Handle_TourNotFound_ShouldThrow()
    {
        _tourRepoMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Tour?)null);
        var command = new AssignSubstituteCommand(TourId: 99, NewGuideId: 5);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*not found*");
    }
}
