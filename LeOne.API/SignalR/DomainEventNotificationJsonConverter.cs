using System.Text.Json;
using System.Text.Json.Serialization;
using LeOne.Application.Common.DomainEvents;
using LeOne.Domain.DomainEvents;

namespace LeOne.API.SignalR
{
    public sealed class DomainEventNotificationJsonConverter : JsonConverter<DomainEventNotification>
    {
        public override DomainEventNotification Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var document = JsonDocument.ParseValue(ref reader);
            var root = document.RootElement;

            if (!root.TryGetProperty("eventType", out var eventTypeProperty) ||
                eventTypeProperty.ValueKind != JsonValueKind.String)
            {
                throw new JsonException("Domain event notifications must include an eventType property.");
            }

            var identifier = eventTypeProperty.GetString();
            if (string.IsNullOrWhiteSpace(identifier))
                throw new JsonException("Domain event notifications must specify a non-empty event identifier.");

            if (!DomainEventIds.TryGetType(identifier, out var eventType) || eventType is null)
                throw new JsonException($"Unknown domain event identifier '{identifier}'.");

            if (!root.TryGetProperty("displayName", out var displayNameProperty) ||
                displayNameProperty.ValueKind != JsonValueKind.String)
            {
                throw new JsonException("Domain event notifications must include a displayName property.");
            }

            var displayName = displayNameProperty.GetString() ?? identifier;

            if (!root.TryGetProperty("occurredAt", out var occurredAtProperty) ||
                occurredAtProperty.ValueKind != JsonValueKind.String)
            {
                throw new JsonException("Domain event notifications must include an occurredAt property.");
            }

            var occurredAt = occurredAtProperty.GetDateTimeOffset();

            if (!root.TryGetProperty("data", out var dataProperty))
            {
                throw new JsonException("Domain event notifications must include a data property.");
            }

            var payload = (DomainEvent?)dataProperty.Deserialize(eventType, options);

            return payload is null
                ? throw new JsonException($"Unable to deserialize the payload for domain event '{identifier}'.")
                : new DomainEventNotification(identifier, displayName, occurredAt, payload);
        }

        public override void Write(Utf8JsonWriter writer, DomainEventNotification value, JsonSerializerOptions options)
        {
            ArgumentNullException.ThrowIfNull(writer);
            ArgumentNullException.ThrowIfNull(value);

            writer.WriteStartObject();
            writer.WriteString("eventType", value.EventType);
            writer.WriteString("displayName", value.DisplayName);
            writer.WriteString("occurredAt", value.OccurredAt);
            writer.WritePropertyName("data");
            JsonSerializer.Serialize(writer, value.Data, value.Data.GetType(), options);
            writer.WriteEndObject();
        }
    }
}
