namespace MessageClient.Interfaces;

public interface IMessageClient {
    Task SubscribeAsync<T>(string subscription, Action<T> handler, CancellationToken cancellationToken = default);
    Task PublishAsync<T>(string subscription, T message, CancellationToken cancellationToken = default);
    Task UnsubscribeAsync(string subscription, CancellationToken cancellationToken = default);
}