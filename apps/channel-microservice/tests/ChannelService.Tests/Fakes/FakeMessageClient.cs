using MessageClient.Interfaces;

namespace ChannelService.Tests.Fakes;

public class FakeMessageClient : IMessageClient
{
    public List<object> Published { get; } = new();

    public Task SubscribeAsync<T>(string subscription, Action<T> handler, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task PublishAsync<T>(string subscription, T message, CancellationToken cancellationToken = default)
    {
        Published.Add(message!);
        return Task.CompletedTask;
    }

    public T SinglePublished<T>()
    {
        return Published.OfType<T>().Single();
    }

    public Task UnsubscribeAsync(string subscription,CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}