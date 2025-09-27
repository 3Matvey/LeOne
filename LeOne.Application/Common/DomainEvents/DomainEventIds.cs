using LeOne.Domain.DomainEvents;
using System.Collections.ObjectModel;

namespace LeOne.Application.Common.DomainEvents
{
    public static class DomainEventIds
    {
        public const string ProductCreated = "product.created.v1";
        public const string PriceChanged = "product.price_changed.v1";
        public const string SpaServiceCreated = "spa_service.created.v1";

        private static readonly IReadOnlyDictionary<Type, string> IdsByType =
            new ReadOnlyDictionary<Type, string>(
                new Dictionary<Type, string>
                {
                    [typeof(ProductCreatedDomainEvent)] = ProductCreated,
                    [typeof(PriceChangedDomainEvent)] = PriceChanged,
                    [typeof(SpaServiceCreatedDomainEvent)] = SpaServiceCreated,
                });

        private static readonly IReadOnlyDictionary<string, Type> TypesById =
            new ReadOnlyDictionary<string, Type>(
                IdsByType.ToDictionary(
                    static kvp => kvp.Value,
                    static kvp => kvp.Key,
                    StringComparer.Ordinal));

        public static IReadOnlyDictionary<Type, string> All => IdsByType;

        public static bool TryGetType(string identifier, out Type? eventType)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                eventType = null;
                return false;
            }

            return TypesById.TryGetValue(identifier, out eventType);
        }

        public static string GetIdentifier(Type eventType)
        {
            ArgumentNullException.ThrowIfNull(eventType);

            if (!IdsByType.TryGetValue(eventType, out var identifier))
            {
                throw new KeyNotFoundException($"No domain event identifier registered for type {eventType.FullName ?? eventType.Name}.");
            }

            return identifier;
        }
    }
}
