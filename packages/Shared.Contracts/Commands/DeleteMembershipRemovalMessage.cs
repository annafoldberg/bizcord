namespace Shared.Contracts.Commands;

public sealed class DeleteMembershipRemovalMessage
{
    public Guid SagaId { get; set; }
    public Guid MessageId { get; set; }
    public int ChannelId { get; set; }
}