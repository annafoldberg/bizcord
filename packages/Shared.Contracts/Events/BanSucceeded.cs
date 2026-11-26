namespace Shared.Contracts.Events;

public class BanSucceeded
{
    public Guid SagaId { get; set; }
    public Guid UserId { get; set; }
}