using FluentAssertions;
using Moq;
using TourManagement.Application.Queries;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;
using TourManagement.Domain.Interfaces;

namespace TourManagement.Tests.Application;

public class GetProfileQueryHandlerTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly GetProfileQueryHandler _handler;

    public GetProfileQueryHandlerTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _handler = new GetProfileQueryHandler(_userRepoMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsProfileWithInterests()
    {
        var user = new User("jovan", "Jovan", "Jovic", "jovan@test.com", "hash", UserRole.Tourist);
        user.SetInterests(new List<Interest> { Interest.Art, Interest.Nature });
        user.EnableRecommendations();
        _userRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await _handler.Handle(new GetProfileQuery(1), CancellationToken.None);

        result.Username.Should().Be("jovan");
        result.Interests.Should().BeEquivalentTo(new[] { "Art", "Nature" });
        result.RecommendationsEnabled.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_UserNotFound_Throws()
    {
        _userRepoMock.Setup(r => r.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        var act = () => _handler.Handle(new GetProfileQuery(99), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("*not found*");
    }
}
