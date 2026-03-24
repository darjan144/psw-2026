using MediatR;

namespace TourManagement.Domain.Entities;

public abstract class DomainEvent : INotification
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
