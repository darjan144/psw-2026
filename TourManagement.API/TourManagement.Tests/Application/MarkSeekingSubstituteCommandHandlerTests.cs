using FluentAssertions;
using Moq;
using TourManagement.Application.Commands;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;
using TourManagement.Domain.Interfaces;
using TourManagement.Domain.ValueObjects;

namespace TourManagement.Tests.Application;

public class MarkSeekingSubstituteCommandHandlerTests
{
    private readonly Mock<ITourRepository> _tourRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly MarkSeekingSubstituteCommandHandler _handler;

    public MarkSeekingSubstituteCommandHandlerTests()
    {
        _tourRepoMock = new Mock<ITourRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new MarkSeekingSubstituteCommandHandler(_tourRepoMock.Object, _unitOfWorkMock.Object);
    }

    private Tour CreatePublishedTour(long guideId = 1)
    {
        var tour = new Tour("Tura", "Opis", TourDifficulty.Easy,
            Interest.Nature, 1500, DateTime.UtcNow.AddDays(7), guideId);
        tour.AddKeyPoint(new KeyPoint("KT1", "Opis", new Coordinate(45.25, 19.85), "img.jpg", 1));
        tour.AddKeyPoint(new KeyPoint("KT2", "Opis", new Coordinate(45.26, 19.86), "img2.jpg", 2));
        tour.Publish();
        return tour;
    }

    [Fact]
    public async Task Handle_ValidRequest_ShouldMarkSeekingSubstitute()
    {
        var tour = CreatePublishedTour(guideId: 5);
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        var command = new MarkSeekingSubstituteCommand(TourId: 1, GuideId: 5);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.SeekingSubstitute.Should().BeTrue();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WrongGuide_ShouldThrow()
    {
        var tour = CreatePublishedTour(guideId: 5);
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        var command = new MarkSeekingSubstituteCommand(TourId: 1, GuideId: 99);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*authorize*");
    }

    [Fact]
    public async Task Handle_TourNotFound_ShouldThrow()
    {
        _tourRepoMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Tour?)null);
        var command = new MarkSeekingSubstituteCommand(TourId: 99, GuideId: 1);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*not found*");
    }
}
