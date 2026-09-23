using Messages;

namespace ChannelService.Application.Messaging.Handlers;

public sealed class PongMessageHandler : IMessageHandler<PongMessage>
{
    public Task HandleAsync(PongMessage message, CancellationToken ct)
    {
        Console.WriteLine($"Received PongMessage: {message}");

        return Task.CompletedTask;
    }
}