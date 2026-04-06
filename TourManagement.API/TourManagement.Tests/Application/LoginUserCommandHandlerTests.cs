using FluentAssertions;
using Moq;
using TourManagement.Application.Commands;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Tests.Application;

public class LoginUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IJwtTokenService> _jwtServiceMock;
    private readonly LoginUserCommandHandler _handler;

    public LoginUserCommandHandlerTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _jwtServiceMock = new Mock<IJwtTokenService>();
        _handler = new LoginUserCommandHandler(
            _userRepoMock.Object,
            _unitOfWorkMock.Object,
            _passwordHasherMock.Object,
            _jwtServiceMock.Object
        );
    }

    private User CreateUser(string username = "jovan", bool blocked = false)
    {
        var user = new User(username, "Jovan", "Jovic", "jovan@example.com", "hashed_pass", UserRole.Tourist);
        if (blocked)
        {
            for (int i = 0; i < 5; i++)
                user.RegisterFailedLogin();
        }
        return user;
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ShouldReturnToken()
    {
        var user = CreateUser();
        var command = new LoginUserCommand("jovan", "Lozinka123!");
        _userRepoMock.Setup(r => r.GetByUsernameAsync("jovan", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasherMock.Setup(p => p.Verify("Lozinka123!", "hashed_pass")).Returns(true);
        _jwtServiceMock.Setup(j => j.GenerateToken(user)).Returns("jwt_token_123");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Token.Should().Be("jwt_token_123");
        result.Username.Should().Be("jovan");
        result.Role.Should().Be("Tourist");
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ShouldResetFailedAttempts()
    {
        var user = CreateUser();
        user.RegisterFailedLogin(); // 1 failed attempt
        var command = new LoginUserCommand("jovan", "Lozinka123!");
        _userRepoMock.Setup(r => r.GetByUsernameAsync("jovan", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasherMock.Setup(p => p.Verify("Lozinka123!", "hashed_pass")).Returns(true);
        _jwtServiceMock.Setup(j => j.GenerateToken(user)).Returns("token");

        await _handler.Handle(command, CancellationToken.None);

        user.FailedLoginAttempts.Should().Be(0);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidUsername_ShouldThrow()
    {
        var command = new LoginUserCommand("nepostoji", "Lozinka123!");
        _userRepoMock.Setup(r => r.GetByUsernameAsync("nepostoji", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Invalid username or password*");
    }

    [Fact]
    public async Task Handle_WithWrongPassword_ShouldThrow()
    {
        var user = CreateUser();
        var command = new LoginUserCommand("jovan", "pogresna");
        _userRepoMock.Setup(r => r.GetByUsernameAsync("jovan", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasherMock.Setup(p => p.Verify("pogresna", "hashed_pass")).Returns(false);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Invalid username or password*");
    }

    [Fact]
    public async Task Handle_WithWrongPassword_ShouldIncrementFailedAttempts()
    {
        var user = CreateUser();
        var command = new LoginUserCommand("jovan", "pogresna");
        _userRepoMock.Setup(r => r.GetByUsernameAsync("jovan", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasherMock.Setup(p => p.Verify("pogresna", "hashed_pass")).Returns(false);

        try { await _handler.Handle(command, CancellationToken.None); } catch { }

        user.FailedLoginAttempts.Should().Be(1);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_AfterFiveFailedAttempts_ShouldBlockUser()
    {
        var user = CreateUser();
        var command = new LoginUserCommand("jovan", "pogresna");
        _userRepoMock.Setup(r => r.GetByUsernameAsync("jovan", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasherMock.Setup(p => p.Verify("pogresna", "hashed_pass")).Returns(false);

        // Simulate 4 prior failed attempts
        for (int i = 0; i < 4; i++)
            user.RegisterFailedLogin();

        try { await _handler.Handle(command, CancellationToken.None); } catch { }

        user.IsBlocked.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenUserIsBlocked_ShouldThrow()
    {
        var user = CreateUser(blocked: true);
        var command = new LoginUserCommand("jovan", "Lozinka123!");
        _userRepoMock.Setup(r => r.GetByUsernameAsync("jovan", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*blocked*");
    }

    [Fact]
    public async Task Handle_WhenUserIsBlocked_ShouldNotCheckPassword()
    {
        var user = CreateUser(blocked: true);
        var command = new LoginUserCommand("jovan", "Lozinka123!");
        _userRepoMock.Setup(r => r.GetByUsernameAsync("jovan", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        try { await _handler.Handle(command, CancellationToken.None); } catch { }

        _passwordHasherMock.Verify(p => p.Verify(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}
