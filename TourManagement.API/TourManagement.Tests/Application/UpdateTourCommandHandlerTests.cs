using FluentAssertions;
using Moq;
using TourManagement.Application.Commands;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Tests.Application;

public class UpdateTourCommandHandlerTests
{
    private readonly Mock<ITourRepository> _tourRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UpdateTourCommandHandler _handler;

    public UpdateTourCommandHandlerTests()
    {
        _tourRepoMock = new Mock<ITourRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new UpdateTourCommandHandler(_tourRepoMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_UpdatesAllFields()
    {
        var tour = new Tour("Old Name", "Old desc", TourDifficulty.Easy, Interest.Nature, 500,
            DateTime.UtcNow.AddDays(7), guideId: 1);
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);

        var command = new UpdateTourCommand(1, 1, "New Name", "New desc", "Hard", "Art", 1200);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Name.Should().Be("New Name");
        result.Description.Should().Be("New desc");
        result.Difficulty.Should().Be("Hard");
        result.Category.Should().Be("Art");
        result.Price.Should().Be(1200);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WrongGuide_Throws()
    {
        var tour = new Tour("Tour", "Desc", TourDifficulty.Easy, Interest.Nature, 500,
            DateTime.UtcNow.AddDays(7), guideId: 1);
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);

        var command = new UpdateTourCommand(1, 999, "New Name", "Desc", "Easy", "Nature", 500);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*not authorized*");
    }

    [Fact]
    public async Task Handle_TourNotFound_Throws()
    {
        _tourRepoMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Tour?)null);

        var command = new UpdateTourCommand(99, 1, "Name", "Desc", "Easy", "Nature", 500);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*not found*");
    }
}
