namespace LeOne.Application.Common.DomainEvents
{
    public sealed record DomainEventTypeDescriptor
    (
        string Name,
        string FullName,
        string Identifier
    );
}
