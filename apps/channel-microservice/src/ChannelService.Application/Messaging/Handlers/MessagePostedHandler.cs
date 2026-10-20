using ChannelService.Application.Services;
using MessageClient.Interfaces;
using Shared.Contracts.Events;

namespace ChannelService.Application.Messaging.Handlers;

public sealed class MessagePostedHandler : IMessageHandler<MessagePostedEvent>
{
    private readonly IMessageClient _messageClient;
    private readonly IChannelService _service;

    public MessagePostedHandler(IMessageClient messageClient, IChannelService service)
    {
        _messageClient = messageClient;
        _service = service;
    }

    public async Task HandleAsync(MessagePostedEvent message, CancellationToken ct)
    {
        var updated = await _service.UpdateLastActivityAsync(message.ChannelId, message.PostedAt, ct);

        if (!updated) return;

        await _messageClient.PublishAsync(
            "activity.processed",
            new ActivityProcessedEvent
            {
                MessageId = message.MessageId,
                ChannelId = message.ChannelId,
                LastActivityAt = message.PostedAt,
                ProcessedAt = DateTime.UtcNow
            },
            ct);
    }
}