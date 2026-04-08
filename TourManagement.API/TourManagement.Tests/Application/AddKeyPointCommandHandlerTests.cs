using FluentAssertions;
using Moq;
using TourManagement.Application.Commands;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;
using TourManagement.Domain.Interfaces;
using TourManagement.Domain.ValueObjects;

namespace TourManagement.Tests.Application;

public class AddKeyPointCommandHandlerTests
{
    private readonly Mock<ITourRepository> _tourRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly AddKeyPointCommandHandler _handler;

    public AddKeyPointCommandHandlerTests()
    {
        _tourRepoMock = new Mock<ITourRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new AddKeyPointCommandHandler(_tourRepoMock.Object, _unitOfWorkMock.Object);
    }

    private Tour CreateTour(long guideId = 1)
    {
        return new Tour("Tura", "Opis", TourDifficulty.Easy,
            Interest.Nature, 1500, DateTime.UtcNow.AddDays(7), guideId);
    }

    [Fact]
    public async Task Handle_ValidData_ShouldReturnKeyPoint()
    {
        var tour = CreateTour();
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        var command = new AddKeyPointCommand(1, "Muzej", "Opis", 45.25, 19.85, "img.jpg", GuideId: 1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Name.Should().Be("Muzej");
        result.Latitude.Should().Be(45.25);
        result.Longitude.Should().Be(19.85);
    }

    [Fact]
    public async Task Handle_ValidData_ShouldSave()
    {
        var tour = CreateTour();
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        var command = new AddKeyPointCommand(1, "Muzej", "Opis", 45.25, 19.85, "img.jpg", GuideId: 1);

        await _handler.Handle(command, CancellationToken.None);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_TourNotFound_ShouldThrow()
    {
        _tourRepoMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Tour?)null);
        var command = new AddKeyPointCommand(99, "Muzej", "Opis", 45.25, 19.85, "img.jpg", GuideId: 1);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*not found*");
    }

    [Fact]
    public async Task Handle_WrongGuide_ShouldThrow()
    {
        var tour = CreateTour(guideId: 1);
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        var command = new AddKeyPointCommand(1, "Muzej", "Opis", 45.25, 19.85, "img.jpg", GuideId: 999);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*not authorized*");
    }
}
