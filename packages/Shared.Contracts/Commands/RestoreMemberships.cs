namespace Shared.Contracts.Commands;

public class RestoreMemberships
{
    public Guid SagaId { get; set; }
    public Guid UserId { get; set; }
}