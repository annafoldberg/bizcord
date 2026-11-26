namespace Shared.Contracts.Events;

public class MembershipRemovalSucceeded
{
    public Guid SagaId { get; set; }
    public Guid UserId { get; set; }
}