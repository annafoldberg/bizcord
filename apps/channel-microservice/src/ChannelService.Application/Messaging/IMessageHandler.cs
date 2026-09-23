namespace ChannelService.Application.Messaging;

public interface IMessageHandler<TMessage>
{
    Task HandleAsync(TMessage message, CancellationToken ct);
}