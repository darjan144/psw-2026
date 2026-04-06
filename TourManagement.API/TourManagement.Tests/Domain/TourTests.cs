using FluentAssertions;
using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;
using TourManagement.Domain.Events;
using TourManagement.Domain.ValueObjects;

namespace TourManagement.Tests.Domain;

public class TourTests
{
    private Tour CreateValidTour()
    {
        return new Tour("Tura po Novom Sadu", "Opis ture", TourDifficulty.Easy,
            Interest.Nature, 1500, DateTime.UtcNow.AddDays(7), guideId: 1);
    }

    private KeyPoint CreateKeyPoint(string name = "Tačka 1")
    {
        return new KeyPoint(name, "Opis", new Coordinate(45.25, 19.85), "img.jpg", 1);
    }

    // --- Creation ---

    [Fact]
    public void Create_WithValidData_ShouldCreateDraftTour()
    {
        var tour = CreateValidTour();

        tour.Name.Should().Be("Tura po Novom Sadu");
        tour.Description.Should().Be("Opis ture");
        tour.Difficulty.Should().Be(TourDifficulty.Easy);
        tour.Category.Should().Be(Interest.Nature);
        tour.Price.Should().Be(1500);
        tour.GuideId.Should().Be(1);
        tour.Status.Should().Be(TourStatus.Draft);
        tour.SeekingSubstitute.Should().BeFalse();
        tour.KeyPoints.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Create_WithInvalidName_ShouldThrow(string? name)
    {
        var act = () => new Tour(name!, "Opis", TourDifficulty.Easy,
            Interest.Nature, 1500, DateTime.UtcNow.AddDays(7), 1);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_WithNegativePrice_ShouldThrow()
    {
        var act = () => new Tour("Tura", "Opis", TourDifficulty.Easy,
            Interest.Nature, -100, DateTime.UtcNow.AddDays(7), 1);

        act.Should().Throw<ArgumentException>();
    }

    // --- Update operations ---

    [Fact]
    public void Rename_WithValidName_ShouldUpdateName()
    {
        var tour = CreateValidTour();

        tour.Rename("Novi naziv");

        tour.Name.Should().Be("Novi naziv");
    }

    [Fact]
    public void Rename_WithEmptyName_ShouldThrow()
    {
        var tour = CreateValidTour();

        var act = () => tour.Rename("");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void SetPrice_WithNegativePrice_ShouldThrow()
    {
        var tour = CreateValidTour();

        var act = () => tour.SetPrice(-50);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void SetPrice_WithValidPrice_ShouldUpdatePrice()
    {
        var tour = CreateValidTour();

        tour.SetPrice(2000);

        tour.Price.Should().Be(2000);
    }

    // --- Key Points ---

    [Fact]
    public void AddKeyPoint_ShouldAddToCollection()
    {
        var tour = CreateValidTour();
        var kp = CreateKeyPoint();

        tour.AddKeyPoint(kp);

        tour.KeyPoints.Should().HaveCount(1);
    }

    // --- Publish ---

    [Fact]
    public void Publish_WithTwoKeyPoints_ShouldPublish()
    {
        var tour = CreateValidTour();
        tour.AddKeyPoint(CreateKeyPoint("Tačka 1"));
        tour.AddKeyPoint(CreateKeyPoint("Tačka 2"));

        tour.Publish();

        tour.Status.Should().Be(TourStatus.Published);
        tour.PublishedDate.Should().NotBeNull();
    }

    [Fact]
    public void Publish_WithLessThanTwoKeyPoints_ShouldThrow()
    {
        var tour = CreateValidTour();
        tour.AddKeyPoint(CreateKeyPoint());

        var act = () => tour.Publish();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*at least 2 key points*");
    }

    [Fact]
    public void Publish_WhenAlreadyPublished_ShouldThrow()
    {
        var tour = CreateValidTour();
        tour.AddKeyPoint(CreateKeyPoint("1"));
        tour.AddKeyPoint(CreateKeyPoint("2"));
        tour.Publish();

        var act = () => tour.Publish();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Only draft*");
    }

    [Fact]
    public void Publish_WithZeroPrice_ShouldThrow()
    {
        var tour = new Tour("Tura", "Opis", TourDifficulty.Easy,
            Interest.Nature, 0, DateTime.UtcNow.AddDays(7), 1);
        tour.AddKeyPoint(CreateKeyPoint("1"));
        tour.AddKeyPoint(CreateKeyPoint("2"));

        var act = () => tour.Publish();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*valid price*");
    }

    // --- Archive ---

    [Fact]
    public void Archive_WhenPublished_ShouldArchive()
    {
        var tour = CreateValidTour();
        tour.AddKeyPoint(CreateKeyPoint("1"));
        tour.AddKeyPoint(CreateKeyPoint("2"));
        tour.Publish();

        tour.Archive();

        tour.Status.Should().Be(TourStatus.Archived);
        tour.ArchivedDate.Should().NotBeNull();
    }

    [Fact]
    public void Archive_WhenDraft_ShouldThrow()
    {
        var tour = CreateValidTour();

        var act = () => tour.Archive();

        act.Should().Throw<InvalidOperationException>();
    }

    // --- Substitute ---

    [Fact]
    public void MarkSeekingSubstitute_WhenPublished_ShouldSetFlag()
    {
        var tour = CreateValidTour();
        tour.AddKeyPoint(CreateKeyPoint("1"));
        tour.AddKeyPoint(CreateKeyPoint("2"));
        tour.Publish();

        tour.MarkSeekingSubstitute();

        tour.SeekingSubstitute.Should().BeTrue();
    }

    [Fact]
    public void MarkSeekingSubstitute_WhenDraft_ShouldThrow()
    {
        var tour = CreateValidTour();

        var act = () => tour.MarkSeekingSubstitute();

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void AssignSubstitute_ShouldChangeGuideAndClearFlag()
    {
        var tour = CreateValidTour();
        tour.AddKeyPoint(CreateKeyPoint("1"));
        tour.AddKeyPoint(CreateKeyPoint("2"));
        tour.Publish();
        tour.MarkSeekingSubstitute();

        tour.AssignSubstitute(newGuideId: 2);

        tour.GuideId.Should().Be(2);
        tour.SeekingSubstitute.Should().BeFalse();
    }

    [Fact]
    public void AssignSubstitute_WhenNotSeeking_ShouldThrow()
    {
        var tour = CreateValidTour();
        tour.AddKeyPoint(CreateKeyPoint("1"));
        tour.AddKeyPoint(CreateKeyPoint("2"));
        tour.Publish();

        var act = () => tour.AssignSubstitute(2);

        act.Should().Throw<InvalidOperationException>();
    }

    // --- Publish raises domain event ---

    [Fact]
    public void Publish_ShouldRaiseTourPublishedEvent()
    {
        var tour = CreateValidTour();
        tour.AddKeyPoint(CreateKeyPoint("1"));
        tour.AddKeyPoint(CreateKeyPoint("2"));

        tour.Publish();

        tour.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<TourPublishedEvent>();
    }

    [Fact]
    public void Publish_ShouldRaiseEventWithCorrectCategory()
    {
        var tour = new Tour("Tura", "Opis", TourDifficulty.Easy,
            Interest.Art, 1000, DateTime.UtcNow.AddDays(7), 1);
        tour.AddKeyPoint(CreateKeyPoint("1"));
        tour.AddKeyPoint(CreateKeyPoint("2"));

        tour.Publish();

        var evt = tour.DomainEvents.OfType<TourPublishedEvent>().Single();
        evt.Category.Should().Be(Interest.Art);
        evt.TourName.Should().Be("Tura");
    }

    // --- Cancel ---

    [Fact]
    public void Cancel_ShouldArchiveAndClearSubstituteFlag()
    {
        var tour = CreateValidTour();
        tour.AddKeyPoint(CreateKeyPoint("1"));
        tour.AddKeyPoint(CreateKeyPoint("2"));
        tour.Publish();
        tour.MarkSeekingSubstitute();

        tour.Cancel();

        tour.Status.Should().Be(TourStatus.Archived);
        tour.SeekingSubstitute.Should().BeFalse();
        tour.ArchivedDate.Should().NotBeNull();
    }
}
