using System.Reflection;
using MessageClient.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ChannelService.Application.Messaging;

public class MessageHandlerSubscriptionManager : IMessageHandlerSubscriptionManager
{
    private readonly IMessageClient _messageClient;
    private readonly IServiceProvider _serviceProvider;
    private readonly IEnumerable<MessageHandlerMetadata> _handlers;
    private readonly List<string> _subscriptions = [];

    public MessageHandlerSubscriptionManager(
        IMessageClient messageClient,
        IServiceProvider serviceProvider,
        IEnumerable<MessageHandlerMetadata> handlers)
    {
        _messageClient = messageClient;
        _serviceProvider = serviceProvider;
        _handlers = handlers;
    }

    public async Task SubscribeAsync(CancellationToken ct)
    {
        foreach (var handler in _handlers)
        {
            // Create SubscribeHandlerAsync with the handler's message type
            var method = GetType()
                .GetMethod(nameof(SubscribeHandlerAsync), BindingFlags.NonPublic | BindingFlags.Instance)!
                .MakeGenericMethod(handler.MessageType);

            // Call SubscribeHandlerAsync for the handler
            await (Task)method.Invoke(this, [ct])!;
        }
    }

    public async Task UnsubscribeAsync()
    {
        foreach (var _subscription in _subscriptions)
            await _messageClient.UnsubscribeAsync(_subscription);
    }

    private async Task SubscribeHandlerAsync<TMessage>(CancellationToken ct)
    {
        // Set subscription to the message type name
        var subscription = typeof(TMessage).Name;

        // Subscribe the handler to the message type
        await _messageClient.SubscribeAsync<TMessage>(
            subscription,
            message => _ = HandleMessageAsync(message, ct), ct);

        _subscriptions.Add(subscription);
    }

    private async Task HandleMessageAsync<TMessage>(TMessage message, CancellationToken ct)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();

        // Get the registered handler for the message type
        var handler = scope.ServiceProvider.GetRequiredService<IMessageHandler<TMessage>>();

        await handler.HandleAsync(message, ct);
    }
}