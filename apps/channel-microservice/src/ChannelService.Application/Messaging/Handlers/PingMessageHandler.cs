using Messages;

namespace ChannelService.Application.Messaging.Handlers;

public sealed class PingMessageHandler : IMessageHandler<PingMessage>
{
    public Task HandleAsync(PingMessage message, CancellationToken ct)
    {
        Console.WriteLine($"Received PingMessage: {message}");
        
        return Task.CompletedTask;
    }
}