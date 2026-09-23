namespace ChannelService.Application.Messaging;

public class MessageHandlerMetadata
{
    public Type MessageType { get; init; } = null!;
    public Type HandlerType { get; init; } = null!;
}