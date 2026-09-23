using System.Reflection;
using MessageClient.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ChannelService.Application.Messaging;

public class MessageHandlerSubscriptionManager : IMessageHandlerSubscriptionManager
{
    private readonly IMessageClient _messageClient;
    private readonly IServiceProvider _serviceProvider;
    private readonly IEnumerable<MessageHandlerMetadata> _handlers;
    private readonly List<string> _subscriptionIds = [];

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
            await (Task)method.Invoke(this, [handler.HandlerType, ct])!;
        }
    }

    public async Task UnsubscribeAsync()
    {
        foreach (var subscriptionId in _subscriptionIds)
            await _messageClient.UnsubscribeAsync(subscriptionId);
    }

    private async Task SubscribeHandlerAsync<TMessage>(Type handlerType, CancellationToken ct)
    {
        // Get the registered handler for the message type
        var handler = (IMessageHandler<TMessage>)_serviceProvider.GetRequiredService(handlerType);

        // Create subscription ID
        var subscriptionId = typeof(TMessage).Name;
        _subscriptionIds.Add(subscriptionId);

        // Subscribe the handler to the message type
        await _messageClient.SubscribeAsync<TMessage>(subscriptionId, message => handler.HandleAsync(message, ct), ct);
    }
}