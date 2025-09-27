using LeOne.Application.Common.DomainEvents;
using LeOne.Application.Common.Interfaces;
using LeOne.Domain.DomainEvents;
using Microsoft.AspNetCore.SignalR;

namespace LeOne.API.SignalR
{
    public sealed class DomainEventSignalRBridge(
        IDomainEventBus domainEventBus,
        IHubContext<DomainEventsHub, IDomainEventsClient> hubContext,
        IDomainEventTypeProvider typeProvider,
        ILogger<DomainEventSignalRBridge> logger) : IHostedService, IDomainEventHandler<DomainEvent>
    {

        private IDisposable? _subscription;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _subscription = domainEventBus.Subscribe<DomainEvent>(this);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _subscription?.Dispose();
            _subscription = null;
            return Task.CompletedTask;
        }

        public async Task HandleAsync(DomainEvent domainEvent, CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(domainEvent);

            try
            {
                var eventType = domainEvent.GetType();
                var descriptor = typeProvider.FindByType(eventType);
                if (descriptor is null)
                {
                    logger.LogError(
                        "No descriptor found for domain event type {EventType}. Ensure it is registered in the provider.",
                        eventType.FullName ?? eventType.Name);
                    return;
                }

                var identifier = descriptor.Identifier;
                var displayName = descriptor.Name;
                var occurredAt = domainEvent.CreatedAt;

                var notification = new DomainEventNotification(
                    identifier,
                    displayName,
                    occurredAt,
                    domainEvent);

                await hubContext.Clients
                    .Group(identifier)
                    .DomainEventOccurred(notification)
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Failed to relay domain event {EventType} to SignalR clients.",
                    domainEvent.GetType().FullName);
            }
        }
    }
}