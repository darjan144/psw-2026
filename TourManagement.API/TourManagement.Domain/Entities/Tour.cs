using TourManagement.Domain.Enums;
using TourManagement.Domain.Events;

namespace TourManagement.Domain.Entities;

public class Tour : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public TourDifficulty Difficulty { get; private set; }
    public Interest Category { get; private set; }
    public double Price { get; private set; }
    public TourStatus Status { get; private set; }
    public DateTime ScheduledDate { get; private set; }
    public DateTime? PublishedDate { get; private set; }
    public DateTime? ArchivedDate { get; private set; }
    public long GuideId { get; private set; }
    public bool SeekingSubstitute { get; private set; }

    private readonly List<KeyPoint> _keyPoints = new();
    public IReadOnlyCollection<KeyPoint> KeyPoints => _keyPoints.AsReadOnly();

    private Tour() { } // EF Core

    public Tour(string name, string description, TourDifficulty difficulty, Interest category,
                double price, DateTime scheduledDate, long guideId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tour name is required.", nameof(name));
        if (price < 0)
            throw new ArgumentException("Price cannot be negative.", nameof(price));

        Name = name;
        Description = description ?? string.Empty;
        Difficulty = difficulty;
        Category = category;
        Price = price;
        ScheduledDate = scheduledDate;
        GuideId = guideId;
        Status = TourStatus.Draft;
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tour name is required.", nameof(name));
        Name = name;
    }

    public void UpdateDescription(string description)
    {
        Description = description ?? string.Empty;
    }

    public void SetDifficulty(TourDifficulty difficulty) => Difficulty = difficulty;

    public void SetCategory(Interest category) => Category = category;

    public void SetPrice(double price)
    {
        if (price < 0)
            throw new ArgumentException("Price cannot be negative.", nameof(price));
        Price = price;
    }

    public void AddKeyPoint(KeyPoint keyPoint)
    {
        _keyPoints.Add(keyPoint);
    }

    public void RemoveKeyPoint(long keyPointId)
    {
        var keyPoint = _keyPoints.FirstOrDefault(kp => kp.Id == keyPointId);
        if (keyPoint != null) _keyPoints.Remove(keyPoint);
    }

    public void Publish()
    {
        if (Status != TourStatus.Draft)
            throw new InvalidOperationException("Only draft tours can be published.");
        if (_keyPoints.Count < 2)
            throw new InvalidOperationException("Tour must have at least 2 key points to be published.");
        if (string.IsNullOrWhiteSpace(Name) || Price <= 0)
            throw new InvalidOperationException("Tour must have a name and a valid price to be published.");

        Status = TourStatus.Published;
        PublishedDate = DateTime.UtcNow;
        AddDomainEvent(new TourPublishedEvent(Id, Category, Name));
    }

    public void Archive()
    {
        if (Status != TourStatus.Published)
            throw new InvalidOperationException("Only published tours can be archived.");

        Status = TourStatus.Archived;
        ArchivedDate = DateTime.UtcNow;
    }

    public void MarkSeekingSubstitute()
    {
        if (Status != TourStatus.Published)
            throw new InvalidOperationException("Only published tours can seek substitutes.");
        SeekingSubstitute = true;
    }

    public void AssignSubstitute(long newGuideId)
    {
        if (!SeekingSubstitute)
            throw new InvalidOperationException("Tour is not seeking a substitute.");

        GuideId = newGuideId;
        SeekingSubstitute = false;
    }

    public void Cancel()
    {
        Status = TourStatus.Archived;
        ArchivedDate = DateTime.UtcNow;
        SeekingSubstitute = false;
    }
}
