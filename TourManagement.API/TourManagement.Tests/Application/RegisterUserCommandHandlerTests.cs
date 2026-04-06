using FluentAssertions;
using Moq;
using TourManagement.Application.Commands;
using TourManagement.Application.Services;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Tests.Application;

public class RegisterUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly RegisterUserCommandHandler _handler;

    public RegisterUserCommandHandlerTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _handler = new RegisterUserCommandHandler(
            _userRepoMock.Object,
            _unitOfWorkMock.Object,
            _passwordHasherMock.Object
        );
    }

    private RegisterUserCommand CreateValidCommand()
    {
        return new RegisterUserCommand(
            Username: "jovan",
            FirstName: "Jovan",
            LastName: "Jovic",
            Email: "jovan@example.com",
            Password: "Lozinka123!",
            Interests: new List<Interest> { Interest.Nature, Interest.Art },
            RecommendationsEnabled: true
        );
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldCreateUser()
    {
        var command = CreateValidCommand();
        _passwordHasherMock.Setup(p => p.Hash(command.Password)).Returns("hashed_password");
        _userRepoMock.Setup(r => r.GetByUsernameAsync(command.Username, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _userRepoMock.Setup(r => r.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.Username.Should().Be("jovan");
        result.Email.Should().Be("jovan@example.com");
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldHashPassword()
    {
        var command = CreateValidCommand();
        _passwordHasherMock.Setup(p => p.Hash(command.Password)).Returns("hashed_password");

        await _handler.Handle(command, CancellationToken.None);

        _passwordHasherMock.Verify(p => p.Hash("Lozinka123!"), Times.Once);
    }

    [Fact]
    public async Task Handle_WithValidData_ShouldCallAddAndSave()
    {
        var command = CreateValidCommand();
        _passwordHasherMock.Setup(p => p.Hash(It.IsAny<string>())).Returns("hashed");

        await _handler.Handle(command, CancellationToken.None);

        _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInterests_ShouldSetInterestsOnUser()
    {
        var command = CreateValidCommand();
        User? capturedUser = null;
        _passwordHasherMock.Setup(p => p.Hash(It.IsAny<string>())).Returns("hashed");
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((u, _) => capturedUser = u);

        await _handler.Handle(command, CancellationToken.None);

        capturedUser.Should().NotBeNull();
        capturedUser!.Interests.Should().BeEquivalentTo(new[] { Interest.Nature, Interest.Art });
    }

    [Fact]
    public async Task Handle_WithRecommendationsEnabled_ShouldEnableRecommendations()
    {
        var command = CreateValidCommand();
        User? capturedUser = null;
        _passwordHasherMock.Setup(p => p.Hash(It.IsAny<string>())).Returns("hashed");
        _userRepoMock.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((u, _) => capturedUser = u);

        await _handler.Handle(command, CancellationToken.None);

        capturedUser!.RecommendationsEnabled.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WhenUsernameExists_ShouldThrow()
    {
        var command = CreateValidCommand();
        _userRepoMock.Setup(r => r.GetByUsernameAsync(command.Username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User("jovan", "Existing", "User", "other@email.com", "hash", UserRole.Tourist));

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Username*already*");
    }

    [Fact]
    public async Task Handle_WhenEmailExists_ShouldThrow()
    {
        var command = CreateValidCommand();
        _userRepoMock.Setup(r => r.GetByUsernameAsync(command.Username, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _userRepoMock.Setup(r => r.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User("other", "Other", "User", "jovan@example.com", "hash", UserRole.Tourist));

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Email*already*");
    }
}
