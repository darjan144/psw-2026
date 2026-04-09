using FluentAssertions;
using Moq;
using TourManagement.Application.Commands;
using TourManagement.Application.Queries;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Tests.Application;

public class AdminHandlerTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public AdminHandlerTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
    }

    private User CreateBlockedUser(int blockCount = 1)
    {
        var user = new User("blocked", "Marko", "Markovic", "marko@test.com", "hash", UserRole.Tourist);
        for (int i = 0; i < blockCount; i++)
        {
            for (int j = 0; j < 5; j++)
                user.RegisterFailedLogin();
        }
        return user;
    }

    // --- GetBlockedUsersQueryHandler ---

    [Fact]
    public async Task GetBlocked_WithUsers_ShouldReturnDtos()
    {
        var users = new List<User> { CreateBlockedUser(1), CreateBlockedUser(2) };
        _userRepoMock.Setup(r => r.GetBlockedUsersAsync(It.IsAny<CancellationToken>())).ReturnsAsync(users);
        var handler = new GetBlockedUsersQueryHandler(_userRepoMock.Object);

        var result = await handler.Handle(new GetBlockedUsersQuery(), CancellationToken.None);

        result.Should().HaveCount(2);
        result[0].BlockCount.Should().Be(1);
        result[1].BlockCount.Should().Be(2);
    }

    [Fact]
    public async Task GetBlocked_NoUsers_ShouldReturnEmpty()
    {
        _userRepoMock.Setup(r => r.GetBlockedUsersAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<User>());
        var handler = new GetBlockedUsersQueryHandler(_userRepoMock.Object);

        var result = await handler.Handle(new GetBlockedUsersQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }

    // --- UnblockUserCommandHandler ---

    [Fact]
    public async Task Unblock_ValidUser_ShouldUnblock()
    {
        var user = CreateBlockedUser(1);
        _userRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        var handler = new UnblockUserCommandHandler(_userRepoMock.Object, _unitOfWorkMock.Object);

        var result = await handler.Handle(new UnblockUserCommand(1), CancellationToken.None);

        result.BlockCount.Should().Be(1);
        user.IsBlocked.Should().BeFalse();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Unblock_ThreeTimesBlocked_ShouldThrow()
    {
        var user = CreateBlockedUser(3);
        _userRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        var handler = new UnblockUserCommandHandler(_userRepoMock.Object, _unitOfWorkMock.Object);

        var act = () => handler.Handle(new UnblockUserCommand(1), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*3 times*");
    }

    [Fact]
    public async Task Unblock_UserNotFound_ShouldThrow()
    {
        _userRepoMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);
        var handler = new UnblockUserCommandHandler(_userRepoMock.Object, _unitOfWorkMock.Object);

        var act = () => handler.Handle(new UnblockUserCommand(99), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*not found*");
    }
}
