using FluentAssertions;
using Moq;
using TourManagement.Application.Commands;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Tests.Application;

public class UpdateProfileCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly UpdateProfileCommandHandler _handler;

    public UpdateProfileCommandHandlerTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _handler = new UpdateProfileCommandHandler(_userRepoMock.Object, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_UpdatesInterests()
    {
        var user = new User("jovan", "Jovan", "Jovic", "jovan@test.com", "hash", UserRole.Tourist);
        user.SetInterests(new List<Interest> { Interest.Nature });
        _userRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var command = new UpdateProfileCommand(1, new List<Interest> { Interest.Sport, Interest.Food }, true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Interests.Should().BeEquivalentTo(new[] { "Sport", "Food" });
        result.RecommendationsEnabled.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_DisablesRecommendations()
    {
        var user = new User("ana", "Ana", "Anic", "ana@test.com", "hash", UserRole.Tourist);
        user.EnableRecommendations();
        _userRepoMock.Setup(r => r.GetByIdAsync(2, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var command = new UpdateProfileCommand(2, new List<Interest> { Interest.Nature }, false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.RecommendationsEnabled.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_UserNotFound_Throws()
    {
        _userRepoMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        var command = new UpdateProfileCommand(99, new List<Interest> { Interest.Art }, true);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*not found*");
    }

    [Fact]
    public async Task Handle_SavesChanges()
    {
        var user = new User("marko", "Marko", "Markovic", "marko@test.com", "hash", UserRole.Tourist);
        _userRepoMock.Setup(r => r.GetByIdAsync(3, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var command = new UpdateProfileCommand(3, new List<Interest> { Interest.Shopping }, true);

        await _handler.Handle(command, CancellationToken.None);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
