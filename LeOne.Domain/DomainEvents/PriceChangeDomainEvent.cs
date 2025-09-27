namespace LeOne.Domain.DomainEvents
{
    public sealed record PriceChangedDomainEvent(
        Guid ChangedEntityId,
        long OldPriceInCents,
        long NewPriceInCents) : DomainEvent
    {
        public string? Description { get; init; }
    }
}
