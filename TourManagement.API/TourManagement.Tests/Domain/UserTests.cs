using FluentAssertions;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;

namespace TourManagement.Tests.Domain;

public class UserTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateUser()
    {
        var user = new User("jovan", "Jovan", "Jovic", "jovan@example.com", "HashedPass123", UserRole.Tourist);

        user.Username.Should().Be("jovan");
        user.FirstName.Should().Be("Jovan");
        user.LastName.Should().Be("Jovic");
        user.Email.Should().Be("jovan@example.com");
        user.PasswordHash.Should().Be("HashedPass123");
        user.Role.Should().Be(UserRole.Tourist);
        user.IsBlocked.Should().BeFalse();
        user.BlockCount.Should().Be(0);
        user.FailedLoginAttempts.Should().Be(0);
        user.BonusPoints.Should().Be(0);
        user.RecommendationsEnabled.Should().BeFalse();
        user.Interests.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Create_WithInvalidUsername_ShouldThrow(string? username)
    {
        var act = () => new User(username!, "Jovan", "Jovic", "jovan@example.com", "HashedPass123", UserRole.Tourist);

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Create_WithInvalidEmail_ShouldThrow(string? email)
    {
        var act = () => new User("jovan", "Jovan", "Jovic", email!, "HashedPass123", UserRole.Tourist);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void SetInterests_ShouldUpdateInterests()
    {
        var user = new User("jovan", "Jovan", "Jovic", "jovan@example.com", "HashedPass123", UserRole.Tourist);
        var interests = new List<Interest> { Interest.Nature, Interest.Art };

        user.SetInterests(interests);

        user.Interests.Should().BeEquivalentTo(interests);
    }

    [Fact]
    public void EnableRecommendations_ShouldSetFlag()
    {
        var user = new User("jovan", "Jovan", "Jovic", "jovan@example.com", "HashedPass123", UserRole.Tourist);

        user.EnableRecommendations();

        user.RecommendationsEnabled.Should().BeTrue();
    }

    [Fact]
    public void DisableRecommendations_ShouldClearFlag()
    {
        var user = new User("jovan", "Jovan", "Jovic", "jovan@example.com", "HashedPass123", UserRole.Tourist);
        user.EnableRecommendations();

        user.DisableRecommendations();

        user.RecommendationsEnabled.Should().BeFalse();
    }

    [Fact]
    public void RegisterFailedLogin_ShouldIncrementAttempts()
    {
        var user = new User("jovan", "Jovan", "Jovic", "jovan@example.com", "HashedPass123", UserRole.Tourist);

        user.RegisterFailedLogin();

        user.FailedLoginAttempts.Should().Be(1);
    }

    [Fact]
    public void RegisterFailedLogin_AfterFiveAttempts_ShouldBlockUser()
    {
        var user = new User("jovan", "Jovan", "Jovic", "jovan@example.com", "HashedPass123", UserRole.Tourist);

        for (int i = 0; i < 5; i++)
            user.RegisterFailedLogin();

        user.IsBlocked.Should().BeTrue();
        user.BlockCount.Should().Be(1);
        user.FailedLoginAttempts.Should().Be(0);
    }

    [Fact]
    public void ResetFailedLoginAttempts_ShouldResetToZero()
    {
        var user = new User("jovan", "Jovan", "Jovic", "jovan@example.com", "HashedPass123", UserRole.Tourist);
        user.RegisterFailedLogin();
        user.RegisterFailedLogin();

        user.ResetFailedLoginAttempts();

        user.FailedLoginAttempts.Should().Be(0);
    }

    [Fact]
    public void Unblock_WhenBlockedLessThanThreeTimes_ShouldUnblock()
    {
        var user = new User("jovan", "Jovan", "Jovic", "jovan@example.com", "HashedPass123", UserRole.Tourist);
        for (int i = 0; i < 5; i++)
            user.RegisterFailedLogin();

        user.Unblock();

        user.IsBlocked.Should().BeFalse();
    }

    [Fact]
    public void Unblock_WhenBlockedThreeTimes_ShouldThrow()
    {
        var user = new User("jovan", "Jovan", "Jovic", "jovan@example.com", "HashedPass123", UserRole.Tourist);

        // Block 3 times
        for (int block = 0; block < 3; block++)
        {
            for (int i = 0; i < 5; i++)
                user.RegisterFailedLogin();

            if (block < 2)
                user.Unblock();
        }

        var act = () => user.Unblock();

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void AddBonusPoints_ShouldIncreasePoints()
    {
        var user = new User("jovan", "Jovan", "Jovic", "jovan@example.com", "HashedPass123", UserRole.Tourist);

        user.AddBonusPoints(100);

        user.BonusPoints.Should().Be(100);
    }

    [Fact]
    public void UseBonusPoints_ShouldDecreasePoints()
    {
        var user = new User("jovan", "Jovan", "Jovic", "jovan@example.com", "HashedPass123", UserRole.Tourist);
        user.AddBonusPoints(100);

        var used = user.UseBonusPoints(60);

        used.Should().Be(60);
        user.BonusPoints.Should().Be(40);
    }

    [Fact]
    public void UseBonusPoints_WhenNotEnough_ShouldUseAllAvailable()
    {
        var user = new User("jovan", "Jovan", "Jovic", "jovan@example.com", "HashedPass123", UserRole.Tourist);
        user.AddBonusPoints(30);

        var used = user.UseBonusPoints(50);

        used.Should().Be(30);
        user.BonusPoints.Should().Be(0);
    }
}
