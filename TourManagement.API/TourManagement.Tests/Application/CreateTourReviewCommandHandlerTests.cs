using FluentAssertions;
using Moq;
using TourManagement.Application.Commands;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Tests.Application;

public class CreateTourReviewCommandHandlerTests
{
    private readonly Mock<ITourReviewRepository> _reviewRepoMock;
    private readonly Mock<ITourRepository> _tourRepoMock;
    private readonly Mock<ITourPurchaseRepository> _purchaseRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CreateTourReviewCommandHandler _handler;

    public CreateTourReviewCommandHandlerTests()
    {
        _reviewRepoMock = new Mock<ITourReviewRepository>();
        _tourRepoMock = new Mock<ITourRepository>();
        _purchaseRepoMock = new Mock<ITourPurchaseRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new CreateTourReviewCommandHandler(
            _reviewRepoMock.Object, _tourRepoMock.Object,
            _purchaseRepoMock.Object, _unitOfWorkMock.Object);
    }

    private Tour CreatePastTour()
    {
        return new Tour("Tura", "Opis", TourDifficulty.Easy,
            Interest.Nature, 1500, DateTime.UtcNow.AddDays(-3), guideId: 1);
    }

    [Fact]
    public async Task Handle_ValidReview_ShouldReturnDto()
    {
        var tour = CreatePastTour();
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        _purchaseRepoMock.Setup(r => r.HasPurchasedAsync(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _reviewRepoMock.Setup(r => r.GetByTouristAndTourAsync(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync((TourReview?)null);
        var command = new CreateTourReviewCommand(TourId: 1, TouristId: 1, Rating: 4, Comment: "Odlicno");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Rating.Should().Be(4);
        result.Comment.Should().Be("Odlicno");
        result.TourId.Should().Be(1);
    }

    [Fact]
    public async Task Handle_ValidReview_ShouldSave()
    {
        var tour = CreatePastTour();
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        _purchaseRepoMock.Setup(r => r.HasPurchasedAsync(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _reviewRepoMock.Setup(r => r.GetByTouristAndTourAsync(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync((TourReview?)null);
        var command = new CreateTourReviewCommand(TourId: 1, TouristId: 1, Rating: 5, Comment: null);

        await _handler.Handle(command, CancellationToken.None);

        _reviewRepoMock.Verify(r => r.AddAsync(It.IsAny<TourReview>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_TourNotFound_ShouldThrow()
    {
        _tourRepoMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Tour?)null);
        var command = new CreateTourReviewCommand(TourId: 99, TouristId: 1, Rating: 5, Comment: null);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*not found*");
    }

    [Fact]
    public async Task Handle_NotPurchased_ShouldThrow()
    {
        var tour = CreatePastTour();
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        _purchaseRepoMock.Setup(r => r.HasPurchasedAsync(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        var command = new CreateTourReviewCommand(TourId: 1, TouristId: 1, Rating: 5, Comment: null);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*purchased*");
    }

    [Fact]
    public async Task Handle_AlreadyReviewed_ShouldThrow()
    {
        var tour = CreatePastTour();
        var existingReview = new TourReview(1, 1, 5, null, DateTime.UtcNow.AddDays(-3));
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        _purchaseRepoMock.Setup(r => r.HasPurchasedAsync(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _reviewRepoMock.Setup(r => r.GetByTouristAndTourAsync(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(existingReview);
        var command = new CreateTourReviewCommand(TourId: 1, TouristId: 1, Rating: 3, Comment: null);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*already reviewed*");
    }

    [Fact]
    public async Task Handle_LowRatingWithoutComment_ShouldThrow()
    {
        var tour = CreatePastTour();
        _tourRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(tour);
        _purchaseRepoMock.Setup(r => r.HasPurchasedAsync(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _reviewRepoMock.Setup(r => r.GetByTouristAndTourAsync(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync((TourReview?)null);
        var command = new CreateTourReviewCommand(TourId: 1, TouristId: 1, Rating: 1, Comment: null);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<ArgumentException>().WithMessage("*Comment*");
    }
}
