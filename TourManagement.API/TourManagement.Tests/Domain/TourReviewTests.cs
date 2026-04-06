using FluentAssertions;
using TourManagement.Domain.Entities;

namespace TourManagement.Tests.Domain;

public class TourReviewTests
{
    [Fact]
    public void Create_WithValidRatingAndComment_ShouldCreateReview()
    {
        var tourDate = DateTime.UtcNow.AddDays(-1);

        var review = new TourReview(tourId: 1, touristId: 1, rating: 4, comment: "Odlična tura", tourDate);

        review.TourId.Should().Be(1);
        review.TouristId.Should().Be(1);
        review.Rating.Should().Be(4);
        review.Comment.Should().Be("Odlična tura");
    }

    [Fact]
    public void Create_WithRating3AndNoComment_ShouldSucceed()
    {
        var tourDate = DateTime.UtcNow.AddDays(-1);

        var review = new TourReview(tourId: 1, touristId: 1, rating: 3, comment: null, tourDate);

        review.Rating.Should().Be(3);
        review.Comment.Should().BeNull();
    }

    [Fact]
    public void Create_WithRating5AndNoComment_ShouldSucceed()
    {
        var tourDate = DateTime.UtcNow.AddDays(-1);

        var review = new TourReview(tourId: 1, touristId: 1, rating: 5, comment: null, tourDate);

        review.Rating.Should().Be(5);
    }

    [Fact]
    public void Create_WithRating1AndNoComment_ShouldThrow()
    {
        var tourDate = DateTime.UtcNow.AddDays(-1);

        var act = () => new TourReview(tourId: 1, touristId: 1, rating: 1, comment: null, tourDate);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Comment is required*");
    }

    [Fact]
    public void Create_WithRating2AndNoComment_ShouldThrow()
    {
        var tourDate = DateTime.UtcNow.AddDays(-1);

        var act = () => new TourReview(tourId: 1, touristId: 1, rating: 2, comment: "", tourDate);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Comment is required*");
    }

    [Fact]
    public void Create_WithRating1AndComment_ShouldSucceed()
    {
        var tourDate = DateTime.UtcNow.AddDays(-1);

        var review = new TourReview(tourId: 1, touristId: 1, rating: 1, comment: "Loša tura", tourDate);

        review.Rating.Should().Be(1);
        review.Comment.Should().Be("Loša tura");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public void Create_WithInvalidRating_ShouldThrow(int rating)
    {
        var tourDate = DateTime.UtcNow.AddDays(-1);

        var act = () => new TourReview(tourId: 1, touristId: 1, rating: rating, comment: "Komentar", tourDate);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Create_BeforeTourDate_ShouldThrow()
    {
        var tourDate = DateTime.UtcNow.AddDays(5);

        var act = () => new TourReview(tourId: 1, touristId: 1, rating: 4, comment: null, tourDate);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*before its scheduled date*");
    }

    [Fact]
    public void Create_MoreThanSevenDaysAfterTour_ShouldThrow()
    {
        var tourDate = DateTime.UtcNow.AddDays(-8);

        var act = () => new TourReview(tourId: 1, touristId: 1, rating: 4, comment: null, tourDate);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Review window has closed*");
    }

    [Fact]
    public void Create_ExactlySevenDaysAfterTour_ShouldSucceed()
    {
        // 6 days and 23 hours ago — still within the window
        var tourDate = DateTime.UtcNow.AddDays(-6).AddHours(-23);

        var review = new TourReview(tourId: 1, touristId: 1, rating: 4, comment: null, tourDate);

        review.Rating.Should().Be(4);
    }
}
