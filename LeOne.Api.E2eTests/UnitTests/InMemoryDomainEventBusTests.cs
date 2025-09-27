// InMemoryDomainEventBusTests.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LeOne.Application.Common.Interfaces;
using LeOne.Domain.DomainEvents;
using LeOne.Infrastructure.Data.DomainEvents;
using Microsoft.Extensions.Logging;
using Xunit;

namespace LeOne.Infrastructure.Data.DomainEvents.Tests
{
    // Minimal domain event types for tests
    public sealed record TestEvent(string Payload) : DomainEvent();
    public abstract record BaseEvent : DomainEvent
    {
        protected BaseEvent() : base() { }
    }
    public sealed record DerivedEvent(string Name) : BaseEvent;

    // Simple handler that records received events
    public sealed class RecordingHandler<TEvent>(Func<TEvent, Task>? onHandle = null) 
        : IDomainEventHandler<TEvent> 
        where TEvent : DomainEvent
    {
        private readonly List<TEvent> _received = new();
        private readonly Func<TEvent, Task>? _onHandle = onHandle;

        public IReadOnlyList<TEvent> Received => _received;

        public async Task HandleAsync(TEvent @event, CancellationToken ct)
        {
            _received.Add(@event);
            if (_onHandle is not null)
                await _onHandle(@event);
        }
    }

    public sealed class TestLogger<T> : ILogger<T>
    {
        public readonly List<(LogLevel level, EventId id, string message, Exception? ex)> Entries = new();

        public IDisposable BeginScope<TState>(TState state) where TState : notnull 
            => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            Entries.Add((logLevel, eventId, formatter(state, exception), exception));
        }

        private sealed class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new();
            public void Dispose() { }
        }
    }

    public class InMemoryDomainEventBusTests
    {
        [Fact]
        public async Task Publish_SingleEvent_DeliversToSubscribedHandler()
        {
            var logger = new TestLogger<InMemoryDomainEventBus>();
            var bus = new InMemoryDomainEventBus(logger);

            var handler = new RecordingHandler<TestEvent>();
            using var sub = bus.Subscribe(handler);

            var ev = new TestEvent("hello");
            await bus.Publish(ev);

            Assert.Single(handler.Received);
            Assert.Equal("hello", handler.Received[0].Payload);
        }

        [Fact]
        public async Task Publish_MultipleEvents_DeliversAll()
        {
            var logger = new TestLogger<InMemoryDomainEventBus>();
            var bus = new InMemoryDomainEventBus(logger);

            var handler = new RecordingHandler<TestEvent>();
            using var sub = bus.Subscribe(handler);

            var events = new[]
            {
                new TestEvent("a"),
                new TestEvent("b"),
                new TestEvent("c")
            };

            await bus.Publish(events);

            Assert.Equal(3, handler.Received.Count);
            Assert.Equal(new[] { "a", "b", "c" }, handler.Received.Select(e => e.Payload).ToArray());
        }

        [Fact]
        public async Task Publish_PolymorphicDispatch_DeliversToBaseTypeHandler()
        {
            var logger = new TestLogger<InMemoryDomainEventBus>();
            var bus = new InMemoryDomainEventBus(logger);

            var baseHandler = new RecordingHandler<BaseEvent>();
            using var sub = bus.Subscribe(baseHandler);

            await bus.Publish(new DerivedEvent("X"));

            Assert.Single(baseHandler.Received);
            Assert.IsType<DerivedEvent>(baseHandler.Received[0]);
        }

        [Fact]
        public async Task Dispose_Unsubscribes_HandlerNotInvoked()
        {
            var logger = new TestLogger<InMemoryDomainEventBus>();
            var bus = new InMemoryDomainEventBus(logger);

            var handler = new RecordingHandler<TestEvent>();
            var sub = bus.Subscribe(handler);

            sub.Dispose();
            sub.Dispose();

            await bus.Publish(new TestEvent("ignored"));

            Assert.Empty(handler.Received);
        }

        [Fact]
        public async Task Publish_RespectsCancellation_BeforeDispatch()
        {
            var logger = new TestLogger<InMemoryDomainEventBus>();
            var bus = new InMemoryDomainEventBus(logger);

            var handler = new RecordingHandler<TestEvent>();
            using var sub = bus.Subscribe(handler);

            using var cts = new CancellationTokenSource();
            cts.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(async () =>
                await bus.Publish(new TestEvent("x"), cts.Token));

            Assert.Empty(handler.Received); 
        }

        [Fact]
        public async Task Handler_Exception_IsLogged_AndDoesNotStopOthers()
        {
            var logger = new TestLogger<InMemoryDomainEventBus>();
            var bus = new InMemoryDomainEventBus(logger);

            var throwing = new RecordingHandler<TestEvent>(_ => throw new InvalidOperationException("boom"));
            var ok = new RecordingHandler<TestEvent>();

            using var s1 = bus.Subscribe(throwing);
            using var s2 = bus.Subscribe(ok);

            await bus.Publish(new TestEvent("e"));

            Assert.Contains(logger.Entries, e => e.level == LogLevel.Error && e.ex is InvalidOperationException);
            Assert.Single(ok.Received);
        }

        [Fact]
        public void Subscribe_Returns_Disposable_ThatIsIdempotent()
        {
            var logger = new TestLogger<InMemoryDomainEventBus>();
            var bus = new InMemoryDomainEventBus(logger);

            var handler = new RecordingHandler<TestEvent>();
            var sub = bus.Subscribe(handler);

            sub.Dispose();
            sub.Dispose();
        }

        [Fact]
        public async Task Publish_ThrowsOnNullArguments()
        {
            var logger = new TestLogger<InMemoryDomainEventBus>();
            var bus = new InMemoryDomainEventBus(logger);

            await Assert.ThrowsAsync<ArgumentNullException>(() => bus.Publish((DomainEvent)null!));
            await Assert.ThrowsAsync<ArgumentNullException>(() => bus.Publish((IEnumerable<DomainEvent>)null!));
        }

        [Fact]
        public void Subscribe_ThrowsOnNullHandler()
        {
            var logger = new TestLogger<InMemoryDomainEventBus>();
            var bus = new InMemoryDomainEventBus(logger);

            Assert.Throws<ArgumentNullException>(() => bus.Subscribe<TestEvent>(null!));
        }
    }
}
