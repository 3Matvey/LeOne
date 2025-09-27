using LeOne.API.SignalR;
using LeOne.Application.Common.DomainEvents;
using LeOne.Application.Common.Interfaces;
using LeOne.Domain.DomainEvents;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;

namespace LeOne.API.E2ETests;

[Collection("E2E Database")]
public sealed class DomainEventsHubTests
{
    [Fact]
    public async Task GetAvailableEventTypes_ReturnsRegisteredDescriptors()
    {
        using var factory = new CustomWebApplicationFactory();
        await using var connection = await CreateAndStartConnectionAsync(factory);

        var descriptors = await connection.InvokeAsync<IReadOnlyCollection<DomainEventTypeDescriptor>>(nameof(DomainEventsHub.GetAvailableEventTypes));

        Assert.NotNull(descriptors);
        Assert.NotEmpty(descriptors);

        var productCreatedId = DomainEventIds.ProductCreated;
        Assert.Contains(descriptors!, descriptor => descriptor.Identifier == productCreatedId);
    }

    [Fact]
    public async Task SubscribeToEvent_DeliversPublishedDomainEvent()
    {
        using var factory = new CustomWebApplicationFactory();
        await using var connection = await CreateAndStartConnectionAsync(factory);

        var identifier = DomainEventIds.ProductCreated;
        var completion = new TaskCompletionSource<DomainEventNotification>(TaskCreationOptions.RunContinuationsAsynchronously);

        connection.On<DomainEventNotification>(nameof(IDomainEventsClient.DomainEventOccurred), notification =>
        {
            completion.TrySetResult(notification);
        });

        await connection.InvokeAsync(nameof(DomainEventsHub.SubscribeToEvent), identifier);

        using (var scope = factory.Services.CreateScope())
        {
            var bus = scope.ServiceProvider.GetRequiredService<IDomainEventBus>();
            await bus.Publish(new ProductCreatedDomainEvent(Guid.NewGuid()));
        }

        var completedTask = await Task.WhenAny(completion.Task, Task.Delay(TimeSpan.FromSeconds(5)));
        Assert.Same(completion.Task, completedTask);

        var notification = await completion.Task;
        Assert.Equal(identifier, notification.EventType);
        var payload = Assert.IsType<ProductCreatedDomainEvent>(notification.Data);
        Assert.NotEqual(Guid.Empty, payload.ProductId);
    }

    private static async Task<HubConnection> CreateAndStartConnectionAsync(CustomWebApplicationFactory factory)
    {
        var baseAddress = factory.Server.BaseAddress ?? new Uri("http://localhost");
        var connection = new HubConnectionBuilder()
            .WithUrl(new Uri(baseAddress, "/hubs/domain-events"), options =>
            {
                options.HttpMessageHandlerFactory = _ => factory.Server.CreateHandler();
                options.Transports = HttpTransportType.LongPolling;
            })
            .AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions.Converters.Add(new DomainEventNotificationJsonConverter());
            })
            .WithAutomaticReconnect()
            .Build();

        await connection.StartAsync();
        return connection;
    }
}