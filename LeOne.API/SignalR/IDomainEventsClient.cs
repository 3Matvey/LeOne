namespace LeOne.API.SignalR
{
    public interface IDomainEventsClient
    {
        Task DomainEventOccurred(DomainEventNotification notification);
    }
}
