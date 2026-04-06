using TourManagement.Domain.Entities;
using TourManagement.Domain.Enums;

namespace TourManagement.Domain.Events;

public class TourPublishedEvent : DomainEvent
{
    public long TourId { get; }
    public Interest Category { get; }
    public string TourName { get; }

    public TourPublishedEvent(long tourId, Interest category, string tourName)
    {
        TourId = tourId;
        Category = category;
        TourName = tourName;
    }
}
