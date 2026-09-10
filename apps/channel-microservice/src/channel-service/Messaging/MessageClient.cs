using EasyNetQ;

namespace Messaging;

public class MessageClient : IMessageClient
{
    private readonly IBus _bus;

    public MessageClient(IBus bus)
    {
        _bus = bus;
    }

    public async Task PublishAsync<T>(T message, CancellationToken ct = default)
    {
        await _bus.PubSub.PublishAsync(message, ct);
    }

    public async Task SubscribeAsync<T>(string subscriptionId, Func<T, CancellationToken, Task> handler, CancellationToken ct = default)
    {
        await _bus.PubSub.SubscribeAsync<T>(subscriptionId, message => handler(message, ct), ct);
    }
}