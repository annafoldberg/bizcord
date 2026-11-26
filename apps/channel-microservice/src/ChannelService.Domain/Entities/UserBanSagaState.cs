namespace ChannelService.Domain.Entities;

public class UserBanSagaState
{
    public Guid SagaId { get; set; }
    public Guid UserId { get; set; }

    public bool BanSucceeded { get; set; }
    public bool MembershipsRemoved { get; set; }
    public bool MembershipsRestored { get; set; }

    public bool IsCompleted { get; set; }
    public bool IsFailed { get; set; }

    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }
}