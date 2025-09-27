using LeOne.Domain.DomainEvents;

namespace LeOne.Application.Common.DomainEvents
{
    public static class DomainEventNameFormatter
    {
        private const string Suffix = "DomainEvent";

        public static string GetDisplayName(Type eventType)
        {
            ArgumentNullException.ThrowIfNull(eventType);

            var name = eventType.Name ?? eventType.FullName;
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidOperationException("Domain event type must have a name.");

            return name.EndsWith(Suffix, StringComparison.Ordinal)
                ? name[..^Suffix.Length]
                : name;
        }

        public static string GetDisplayName(DomainEvent domainEvent)
        {
            ArgumentNullException.ThrowIfNull(domainEvent);
            return GetDisplayName(domainEvent.GetType());
        }
    }
}
