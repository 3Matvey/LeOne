using LeOne.Domain.DomainEvents;
using System.Collections.ObjectModel;

namespace LeOne.Application.Common.DomainEvents
{
    public sealed class HardcodedDomainEventTypeProvider : IDomainEventTypeProvider
    {
        private static readonly (Type Type, DomainEventTypeDescriptor Descriptor)[] Registrations =
            DomainEventIds.All
                .Select(static kvp => CreateRegistration(kvp.Key, kvp.Value))
                .ToArray();

        private static readonly IReadOnlyDictionary<string, Type> TypesByIdentifier =
            new ReadOnlyDictionary<string, Type>(
                Registrations.ToDictionary(
                    static r => r.Descriptor.Identifier,
                    static r => r.Type,
                    StringComparer.Ordinal));

        private static readonly IReadOnlyDictionary<string, DomainEventTypeDescriptor> DescriptorsByIdentifier =
            new ReadOnlyDictionary<string, DomainEventTypeDescriptor>(
                Registrations.ToDictionary(
                    static r => r.Descriptor.Identifier,
                    static r => r.Descriptor,
                    StringComparer.Ordinal));

        private static readonly IReadOnlyDictionary<Type, DomainEventTypeDescriptor> DescriptorsByType =
            new ReadOnlyDictionary<Type, DomainEventTypeDescriptor>(
                Registrations.ToDictionary(
                    static r => r.Type,
                    static r => r.Descriptor));

        private static readonly IReadOnlyCollection<DomainEventTypeDescriptor> OrderedDescriptors =
            Array.AsReadOnly(
                Registrations
                    .Select(static r => r.Descriptor)
                    .OrderBy(static d => d.Name, StringComparer.Ordinal)
                    .ToArray());

        public bool TryGetEventType(string identifier, out Type eventType)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                eventType = null!;
                return false;
            }

            return TypesByIdentifier.TryGetValue(identifier, out eventType);
        }

        public DomainEventTypeDescriptor? FindByIdentifier(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                return null;

            return DescriptorsByIdentifier.TryGetValue(identifier, out var descriptor)
                ? descriptor
                : null;
        }

        public DomainEventTypeDescriptor? FindByType(Type eventType)
        {
            ArgumentNullException.ThrowIfNull(eventType);

            return DescriptorsByType.TryGetValue(eventType, out var descriptor)
                ? descriptor
                : null;
        }

        public IReadOnlyCollection<DomainEventTypeDescriptor> GetAll() => OrderedDescriptors;

        private static (Type Type, DomainEventTypeDescriptor Descriptor) CreateRegistration(Type type, string identifier)
        {
            var name = DomainEventNameFormatter.GetDisplayName(type);
            var fullName = type.FullName ?? type.Name ?? identifier;

            return (type, new DomainEventTypeDescriptor(name, fullName, identifier));
        }
    }
}
