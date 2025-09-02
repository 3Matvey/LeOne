namespace LeOne.Domain.DomainEvents
{
    public sealed record ProductCreatedDomainEvent(Guid ProductId) : DomainEvent;
}
