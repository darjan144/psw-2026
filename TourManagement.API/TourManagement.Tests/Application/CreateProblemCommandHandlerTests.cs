using FluentAssertions;
using Moq;
using TourManagement.Application.Commands;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Tests.Application;

public class CreateProblemCommandHandlerTests
{
    private readonly Mock<IProblemRepository> _problemRepoMock;
    private readonly Mock<ITourRepository> _tourRepoMock;
    private readonly Mock<ITourPurchaseRepository> _purchaseRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateProblemCommandHandler _handler;

    public CreateProblemCommandHandlerTests()
    {
        _problemRepoMock = new Mock<IProblemRepository>();
        _tourRepoMock = new Mock<ITourRepository>();
        _purchaseRepoMock = new Mock<ITourPurchaseRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new CreateProblemCommandHandler(
            _problemRepoMock.Object, _tourRepoMock.Object,
            _purchaseRepoMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_ValidProblem_ShouldReturnDto()
    {
        var tour = new Tour("Tura", "Opis", TourDifficulty.Easy, Interest.Nature, 1500, DateTime.UtcNow.AddDays(7), 1);
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        _purchaseRepoMock.Setup(r => r.HasPurchasedAsync(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var command = new CreateProblemCommand(TourId: 1, TouristId: 1, Title: "Problem", Description: "Opis problema");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Title.Should().Be("Problem");
        result.Status.Should().Be("Pending");
        result.TourId.Should().Be(1);
    }

    [Fact]
    public async Task Handle_ValidProblem_ShouldSave()
    {
        var tour = new Tour("Tura", "Opis", TourDifficulty.Easy, Interest.Nature, 1500, DateTime.UtcNow.AddDays(7), 1);
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        _purchaseRepoMock.Setup(r => r.HasPurchasedAsync(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var command = new CreateProblemCommand(TourId: 1, TouristId: 1, Title: "Problem", Description: "Opis");

        await _handler.Handle(command, CancellationToken.None);

        _problemRepoMock.Verify(r => r.AddAsync(It.IsAny<Problem>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_TourNotFound_ShouldThrow()
    {
        _tourRepoMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Tour?)null);
        var command = new CreateProblemCommand(TourId: 99, TouristId: 1, Title: "Problem", Description: "Opis");

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*not found*");
    }

    [Fact]
    public async Task Handle_NotPurchased_ShouldThrow()
    {
        var tour = new Tour("Tura", "Opis", TourDifficulty.Easy, Interest.Nature, 1500, DateTime.UtcNow.AddDays(7), 1);
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        _purchaseRepoMock.Setup(r => r.HasPurchasedAsync(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        var command = new CreateProblemCommand(TourId: 1, TouristId: 1, Title: "Problem", Description: "Opis");

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*purchased*");
    }

    [Fact]
    public async Task Handle_EmptyTitle_ShouldThrow()
    {
        var tour = new Tour("Tura", "Opis", TourDifficulty.Easy, Interest.Nature, 1500, DateTime.UtcNow.AddDays(7), 1);
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        _purchaseRepoMock.Setup(r => r.HasPurchasedAsync(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var command = new CreateProblemCommand(TourId: 1, TouristId: 1, Title: "", Description: "Opis");

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*title*");
    }
}
