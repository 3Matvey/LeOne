using LeOne.Application.Common.Interfaces;
using LeOne.Domain.DomainEvents;

namespace LeOne.Application.Common
{
    public static class ExtensionsForPublishEvents
    {
        /// <summary>
        /// Publishes a single event if the transaction task completed with true.
        /// </summary>
        public static async Task PublishIfOk(
            this Task<bool> transactionTask,
            IDomainEventBus bus,
            DomainEvent domainEvent,
            CancellationToken ct = default)
        {
            ArgumentNullException.ThrowIfNull(transactionTask);
            ArgumentNullException.ThrowIfNull(bus);
            ArgumentNullException.ThrowIfNull(domainEvent);

            if (await transactionTask.ConfigureAwait(false))
                await bus.Publish(domainEvent, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Publishes multiple events if the transaction task completed with true.
        /// </summary>
        public static async Task PublishIfOk(
            this Task<bool> transactionTask,
            IDomainEventBus bus,
            CancellationToken ct = default,
            params DomainEvent[] events)
        {
            ArgumentNullException.ThrowIfNull(transactionTask);
            ArgumentNullException.ThrowIfNull(bus);
            ArgumentNullException.ThrowIfNull(events);

            if (events.Length == 0) return;

            if (await transactionTask.ConfigureAwait(false))
                await bus.Publish(events, ct).ConfigureAwait(false);
        }
    }
}
