namespace Shared.Contracts.Commands;

public class TerminateStreaming
{
    public Guid SagaId { get; set; }
    public Guid UserId { get; set; }
}