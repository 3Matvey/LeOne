using System.Collections.Concurrent;
using LeOne.Application.Common.Interfaces;
using LeOne.Domain.DomainEvents;
using Microsoft.Extensions.Logging;

namespace LeOne.Infrastructure.Data.DomainEvents;

public sealed class InMemoryDomainEventBus(ILogger<InMemoryDomainEventBus>? logger) : IDomainEventBus
{
    private readonly ConcurrentDictionary<Type, ConcurrentDictionary<Guid, HandlerRegistration>> _handlers = new();
    private readonly int _maxDop = Environment.ProcessorCount;

    public async Task Publish(DomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(domainEvent);
        await Publish([domainEvent], cancellationToken).ConfigureAwait(false);
    }

    public async Task Publish(IEnumerable<DomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(domainEvents);

        foreach (var domainEvent in domainEvents)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Dispatch(domainEvent, cancellationToken).ConfigureAwait(false);
        }
    }

    public IDisposable Subscribe<TEvent>(IDomainEventHandler<TEvent> handler)
        where TEvent : DomainEvent
    {
        ArgumentNullException.ThrowIfNull(handler);

        var map = _handlers.GetOrAdd(typeof(TEvent), _ => new ConcurrentDictionary<Guid, HandlerRegistration>());
        var id = Guid.NewGuid();

        // capture handler type name for logging
        var handlerTypeName = handler.GetType().FullName ?? handler.GetType().Name;

        Task HandlerWrapper(DomainEvent evt, CancellationToken ct)
        {
            if (evt is TEvent typed)
                return handler.HandleAsync(typed, ct);
            return Task.CompletedTask;
        }

        map[id] = new HandlerRegistration(id, typeof(TEvent), handlerTypeName, HandlerWrapper);

        return new Subscription(typeof(TEvent), id, _handlers);
    }

    private async Task Dispatch(DomainEvent? domainEvent, CancellationToken ct)
    {
        if (domainEvent is null) return;

        var eventType = domainEvent.GetType();

        // snapshot of subscriptions at publish time
        var targets = _handlers
            .Where(kv => kv.Key.IsAssignableFrom(eventType))
            .SelectMany(kv => kv.Value.Values)
            .ToList();

        if (targets.Count == 0) return;

        using var throttler = new SemaphoreSlim(_maxDop, _maxDop);
        var failures = 0;

        var tasks = targets.Select(async registration =>
        {
            ct.ThrowIfCancellationRequested();
            await throttler.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                await registration.Invoke(domainEvent, ct).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Interlocked.Increment(ref failures);
                logger?.LogError(
                    ex,
                    "DomainEvent handler failed. EventType={EventType}, HandlerType={HandlerType}, SubscriptionId={SubscriptionId}",
                    eventType.FullName,
                    registration.HandlerTypeName,
                    registration.Id);
            }
            finally
            {
                throttler.Release();
            }
        });

        await Task.WhenAll(tasks).ConfigureAwait(false);

        if (failures > 0)
        {
            logger?.LogWarning(
                "DomainEvent publish completed with {FailureCount} handler error(s). EventType={EventType}",
                failures,
                eventType.FullName);
        }
    }

    private sealed class HandlerRegistration(
        Guid id,
        Type eventType,
        string handlerTypeName,
        Func<DomainEvent, CancellationToken, Task> handler)
    {
        public Guid Id { get; } = id;
        public Type EventType { get; } = eventType 
            ?? throw new ArgumentNullException(nameof(eventType));
        public string HandlerTypeName { get; } = handlerTypeName 
            ?? throw new ArgumentNullException(nameof(handlerTypeName));

        private readonly Func<DomainEvent, CancellationToken, Task> _handler = handler 
            ?? throw new ArgumentNullException(nameof(handler));

        public Task Invoke(DomainEvent domainEvent, CancellationToken ct)
            => _handler(domainEvent, ct);
    }

    private sealed class Subscription(
        Type eventType,
        Guid id,
        ConcurrentDictionary<Type, ConcurrentDictionary<Guid, HandlerRegistration>> handlers) : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, true)) // already disposed
                return;

            if (!handlers.TryGetValue(eventType, out var regs))
                return;

            regs.TryRemove(id, out _);

            if (regs.IsEmpty)
                handlers.TryRemove(eventType, out _);
        }
    }
}
