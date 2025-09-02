namespace LeOne.Domain.DomainEvents
{
    public interface IDomainEventHandler<in TEvent>
        where TEvent : DomainEvent
    {
        Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken);
    }
}
