namespace LeOne.Domain.DomainEvents
{
    public abstract record DomainEvent
    {
        public DateTimeOffset CreatedAt { get; } = DateTimeOffset.UtcNow;
    }
}
