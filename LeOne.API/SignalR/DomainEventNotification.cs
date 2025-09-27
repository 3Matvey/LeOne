using LeOne.Domain.DomainEvents;

namespace LeOne.API.SignalR
{
    public sealed record DomainEventNotification
    (
        string EventType,
        string DisplayName,
        DateTimeOffset OccurredAt,
        DomainEvent Data
    );
}
