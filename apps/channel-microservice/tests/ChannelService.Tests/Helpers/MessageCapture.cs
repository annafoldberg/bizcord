using MessageClient.Interfaces;

namespace ChannelService.Tests.Helpers;

/// <summary>
/// Subscribes to events of type T, captures the first event received,
/// and waits until an event arrives or the timeout expires.
/// </summary>
public sealed class MessageCapture<T>
{
    private readonly TaskCompletionSource<T> _result = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public MessageCapture(IMessageClient client)
    {
        client.SubscribeAsync<T>(
            $"test-{Guid.NewGuid()}",
            message => _result.TrySetResult(message)
        ).GetAwaiter().GetResult();
    }

    public async Task<T> WaitForMessageAsync(TimeSpan timeout)
    {
        return await _result.Task.WaitAsync(timeout);
    }
}