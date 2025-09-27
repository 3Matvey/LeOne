using LeOne.Application.Common.DomainEvents;
using Microsoft.AspNetCore.SignalR;

namespace LeOne.API.SignalR
{

    public sealed class DomainEventsHub(IDomainEventTypeProvider typeProvider) : Hub<IDomainEventsClient>
    {
        private readonly IDomainEventTypeProvider _typeProvider = typeProvider
            ?? throw new ArgumentNullException(nameof(typeProvider));

        public async Task SubscribeToEvent(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                throw new HubException("Event identifier is required.");

            var descriptor = _typeProvider.FindByIdentifier(identifier)
                ?? throw new HubException($"Unknown event type '{identifier}'.");

            await Groups.AddToGroupAsync(Context.ConnectionId, descriptor.Identifier).ConfigureAwait(false);
        }

        public async Task UnsubscribeFromEvent(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                return;

            var descriptor = _typeProvider.FindByIdentifier(identifier);
            if (descriptor is null)
                return;

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, descriptor.Identifier).ConfigureAwait(false);
        }

        public Task<IReadOnlyCollection<DomainEventTypeDescriptor>> GetAvailableEventTypes()
            => Task.FromResult(_typeProvider.GetAll());
    }
}