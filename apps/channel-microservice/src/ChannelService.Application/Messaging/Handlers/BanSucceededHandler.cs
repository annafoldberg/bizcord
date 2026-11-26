using ChannelService.Application.Services;
using MessageClient.Interfaces;
using Shared.Contracts.Commands;
using Shared.Contracts.Events;

namespace ChannelService.Application.Messaging.Handlers;

public sealed class BanSucceededHandler : IMessageHandler<BanSucceeded>
{
    private readonly IUserBanSagaService _service;
    private readonly IMessageClient _messageClient;

    public BanSucceededHandler(
        IUserBanSagaService service,
        IMessageClient messageClient)
    {
        _service = service;
        _messageClient = messageClient;
    }

    public async Task HandleAsync(BanSucceeded message, CancellationToken ct)
    {
        // Initialize saga and retrieve original memberships
        var memberships = await _service.InitializeAsync(message.SagaId, message.UserId, ct);

        try
        {            
            // Remove memberships and publish system message
            foreach (var membership in memberships)
            {
                // Remove membership
                await _service.RemoveMembershipAsync(message.SagaId, membership.ChannelId, ct);

                // Request system message is published
                await PublishSystemMessageAsync(
                    "membership.removal.message.create",
                    new CreateMembershipRemovalMessage
                    {
                        SagaId = message.SagaId,
                        MessageId = GetMessageId(message.SagaId, membership.ChannelId),
                        ChannelId = membership.ChannelId,
                        UserId = message.UserId,
                        Content = $"User {message.UserId} was removed from the channel."
                    }, ct);
            }

            // Complete membership removals
            await _service.CompleteMembershipRemovalsAsync(message.SagaId, ct);
        }
        catch (Exception)
        {
            // Restore previously removed memberships
            var removedMemberships = await _service.GetRemovedMembershipsAsync(message.SagaId, ct);

            foreach (var membership in removedMemberships)
            {
                // Restore membership
                await _service.RestoreMembershipAsync(message.SagaId, membership.ChannelId, ct);

                // Request previously published system message is deleted
                await PublishSystemMessageAsync(
                    "membership.removal.message.delete",
                    new DeleteMembershipRemovalMessage
                    {
                        SagaId = message.SagaId,
                        MessageId = GetMessageId(message.SagaId, membership.ChannelId),
                        ChannelId = membership.ChannelId
                    }, ct);
            }

            // Complete compensation
            await _service.CompleteCompensationAsync(message.SagaId, ct);

            // Publish failed completion
            await _messageClient.PublishAsync(
                "membership.removal.failed",
                new MembershipRemovalFailed
                {
                    SagaId = message.SagaId,
                    UserId = message.UserId,
                    Reason = "Membership removal failed."
                }, ct);

            return;
        }

        // Publish successful completion
        await _messageClient.PublishAsync(
            "membership.removal.succeeded",
            new MembershipRemovalSucceeded
            {
                SagaId = message.SagaId,
                UserId = message.UserId
            }, ct);
    }

    private static Guid GetMessageId(Guid sagaId, int channelId)
    {
        var bytes = System.Security.Cryptography.SHA256.HashData(
            System.Text.Encoding.UTF8.GetBytes($"{sagaId}:{channelId}"));

        return new Guid(bytes[..16]);
    }

    private async Task PublishSystemMessageAsync<T>(string subscription, T command, CancellationToken ct)
    {
        try
        {
            await _messageClient.PublishAsync(subscription, command, ct);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // Ignore if system message publishing fails as it is non-critical
        }
    }
}