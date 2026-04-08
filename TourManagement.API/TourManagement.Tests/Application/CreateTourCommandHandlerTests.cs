using FluentAssertions;
using Moq;
using TourManagement.Application.Commands;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Tests.Application;

public class CreateTourCommandHandlerTests
{
    private readonly Mock<ITourRepository> _tourRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateTourCommandHandler _handler;

    public CreateTourCommandHandlerTests()
    {
        _tourRepoMock = new Mock<ITourRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new CreateTourCommandHandler(_tourRepoMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ValidData_ShouldReturnDraftTour()
    {
        var command = new CreateTourCommand(
            "Tura po Novom Sadu", "Opis", TourDifficulty.Easy,
            Interest.Nature, 1500, DateTime.UtcNow.AddDays(7), GuideId: 1);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Name.Should().Be("Tura po Novom Sadu");
        result.Status.Should().Be("Draft");
        result.GuideId.Should().Be(1);
    }

    [Fact]
    public async Task Handle_ValidData_ShouldCallAddAndSave()
    {
        var command = new CreateTourCommand(
            "Tura", "Opis", TourDifficulty.Medium,
            Interest.Art, 2000, DateTime.UtcNow.AddDays(7), GuideId: 1);

        await _handler.Handle(command, CancellationToken.None);

        _tourRepoMock.Verify(r => r.AddAsync(It.IsAny<Tour>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidName_ShouldThrow()
    {
        var command = new CreateTourCommand(
            "", "Opis", TourDifficulty.Easy,
            Interest.Nature, 1500, DateTime.UtcNow.AddDays(7), GuideId: 1);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task Handle_NegativePrice_ShouldThrow()
    {
        var command = new CreateTourCommand(
            "Tura", "Opis", TourDifficulty.Easy,
            Interest.Nature, -100, DateTime.UtcNow.AddDays(7), GuideId: 1);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>();
    }
}
