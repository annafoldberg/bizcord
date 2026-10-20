namespace ChannelService.Application.Messaging;

public interface IMessageHandlerSubscriptionManager
{
    Task SubscribeAsync(CancellationToken ct);
    Task UnsubscribeAsync();
}