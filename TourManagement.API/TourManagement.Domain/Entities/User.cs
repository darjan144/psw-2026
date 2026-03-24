using TourManagement.Domain.Enums;

namespace TourManagement.Domain.Entities;

public class User : Entity
{
    public string Username { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public UserRole Role { get; private set; }
    public bool IsBlocked { get; private set; }
    public int BlockCount { get; private set; }
    public int FailedLoginAttempts { get; private set; }
    public int BonusPoints { get; private set; }
    public bool RecommendationsEnabled { get; private set; }

    private readonly List<Interest> _interests = new();
    public IReadOnlyCollection<Interest> Interests => _interests.AsReadOnly();

    private User() { } // EF Core

    public User(string username, string firstName, string lastName, string email, string passwordHash, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username is required.", nameof(username));
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name is required.", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name is required.", nameof(lastName));
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash is required.", nameof(passwordHash));

        Username = username;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
    }

    public void SetInterests(List<Interest> interests)
    {
        _interests.Clear();
        _interests.AddRange(interests);
    }

    public void EnableRecommendations() => RecommendationsEnabled = true;

    public void DisableRecommendations() => RecommendationsEnabled = false;

    public void RegisterFailedLogin()
    {
        FailedLoginAttempts++;

        if (FailedLoginAttempts >= 5)
        {
            IsBlocked = true;
            BlockCount++;
            FailedLoginAttempts = 0;
        }
    }

    public void ResetFailedLoginAttempts() => FailedLoginAttempts = 0;

    public void Unblock()
    {
        if (BlockCount >= 3)
            throw new InvalidOperationException("User has been blocked 3 times and cannot be unblocked.");

        IsBlocked = false;
        FailedLoginAttempts = 0;
    }

    public void AddBonusPoints(int points)
    {
        if (points < 0)
            throw new ArgumentException("Points must be positive.", nameof(points));

        BonusPoints += points;
    }

    public int UseBonusPoints(int requested)
    {
        var used = Math.Min(requested, BonusPoints);
        BonusPoints -= used;
        return used;
    }
}
