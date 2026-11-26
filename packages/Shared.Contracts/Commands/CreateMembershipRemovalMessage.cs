namespace Shared.Contracts.Commands;

public sealed class CreateMembershipRemovalMessage
{
    public Guid SagaId { get; set; }
    public Guid MessageId { get; set; }
    public int ChannelId { get; set; }
    public Guid UserId { get; set; }
    public string Content { get; set; } = string.Empty;
}