namespace LeOne.Domain.DomainEvents
{
    public sealed record SpaServiceCreatedDomainEvent(Guid SpaServiceId) : DomainEvent;
}
