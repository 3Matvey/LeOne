using LeOne.Domain.DomainEvents;

namespace LeOne.Application.Common.Interfaces;

public interface IDomainEventBus
{
    /// <summary>
    /// Publishes a single domain event to all subscribed handlers.
    /// </summary>
    /// <param name="domainEvent">The domain event to publish.</param>
    /// <param name="cancellationToken">
    /// A token to observe while waiting for the task to complete. 
    /// Can be used to cancel the publishing operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous publish operation.
    /// All matching handlers will be invoked before the task completes.
    /// </returns>
    Task Publish(DomainEvent domainEvent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes multiple domain events to all subscribed handlers.
    /// </summary>
    /// <param name="domainEvents">The collection of domain events to publish.</param>
    /// <param name="cancellationToken">
    /// A token to observe while waiting for the task to complete. 
    /// Can be used to cancel the publishing operation.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous publish operation.
    /// All events in the collection will be dispatched sequentially,
    /// but handlers for each event may be executed concurrently.
    /// </returns>
    Task Publish(IEnumerable<DomainEvent> domainEvents, CancellationToken cancellationToken = default);

    /// <summary>
    /// Subscribes a handler to a specific domain event type.
    /// </summary>
    /// <typeparam name="TEvent">The type of domain event the handler will process.</typeparam>
    /// <param name="handler">The handler instance to subscribe.</param>
    /// <returns>
    /// An <see cref="IDisposable"/> subscription token. 
    /// Disposing it will unsubscribe the handler and release resources.
    /// </returns>
    IDisposable Subscribe<TEvent>(IDomainEventHandler<TEvent> handler)
        where TEvent : DomainEvent;
}