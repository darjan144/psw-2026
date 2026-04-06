namespace TourManagement.Domain.Entities;

public class TourReview : Entity
{
    public long TourId { get; private set; }
    public long TouristId { get; private set; }
    public int Rating { get; private set; }
    public string? Comment { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private TourReview() { } // EF Core

    public TourReview(long tourId, long touristId, int rating, string? comment, DateTime tourScheduledDate)
    {
        if (rating < 1 || rating > 5)
            throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 5.");

        if (rating <= 2 && string.IsNullOrWhiteSpace(comment))
            throw new ArgumentException("Comment is required for ratings of 1 or 2.", nameof(comment));

        if (DateTime.UtcNow < tourScheduledDate)
            throw new InvalidOperationException("Cannot review a tour before its scheduled date.");

        if (DateTime.UtcNow > tourScheduledDate.AddDays(7))
            throw new InvalidOperationException("Review window has closed (7 days after the tour).");

        TourId = tourId;
        TouristId = touristId;
        Rating = rating;
        Comment = comment;
        CreatedAt = DateTime.UtcNow;
    }
}
