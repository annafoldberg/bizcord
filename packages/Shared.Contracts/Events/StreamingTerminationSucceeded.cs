namespace Shared.Contracts.Events;

public class StreamingTerminationSucceeded
{
    public Guid SagaId { get; set; }
    public Guid UserId { get; set; }
}