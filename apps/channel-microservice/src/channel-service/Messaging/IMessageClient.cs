using EasyNetQ;

namespace Messaging;

public interface IMessageClient
{
    Task PublishAsync<T>(T message, CancellationToken ct = default);

    Task SubscribeAsync<T>(
        string subscriptionId, Func<T, CancellationToken, Task> handler,
        CancellationToken ct = default);
}