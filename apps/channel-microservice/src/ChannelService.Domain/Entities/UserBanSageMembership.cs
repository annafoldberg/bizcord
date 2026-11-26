using ChannelService.Domain.Enums;

namespace ChannelService.Domain.Entities;

public class UserBanSagaMembership
{
    public int Id { get; set; }
    public Guid SagaId { get; set; }

    public int ChannelId { get; set; }
    public MemberRole Role { get; set; }
    public DateTime JoinedAt { get; set; }
    
    public bool IsRemoved { get; set; }
}