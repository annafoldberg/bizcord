namespace Shared.Contracts.Commands;

public class BanUser
{
    public Guid SagaId { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
}