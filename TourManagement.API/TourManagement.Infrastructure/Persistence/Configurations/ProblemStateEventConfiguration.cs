using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TourManagement.Domain.Entities.ProblemEvents;

namespace TourManagement.Infrastructure.Persistence.Configurations;

public class ProblemStateEventConfiguration : IEntityTypeConfiguration<ProblemStateEvent>
{
    public void Configure(EntityTypeBuilder<ProblemStateEvent> builder)
    {
        builder.ToTable("ProblemStateEvents");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.ProblemId).IsRequired();
        builder.Property(e => e.SequenceNumber).IsRequired();
        builder.Property(e => e.OccurredAt).IsRequired();
        builder.Property(e => e.CausedByUserId);

        builder.HasIndex(e => e.ProblemId);
        builder.HasIndex(e => new { e.ProblemId, e.SequenceNumber }).IsUnique();

        builder.HasDiscriminator<string>("EventType")
            .HasValue<ProblemCreatedStateEvent>("Created")
            .HasValue<ProblemResolvedStateEvent>("Resolved")
            .HasValue<ProblemSentToReviewStateEvent>("SentToReview")
            .HasValue<ProblemReturnedToGuideStateEvent>("ReturnedToGuide")
            .HasValue<ProblemRejectedStateEvent>("Rejected");

        builder.Ignore(e => e.DomainEvents);
    }
}
