using ChannelService.Domain.Enums;

namespace ChannelService.Domain.Entities;

public class Member
{
    public int Id { get; set; }
    public int ChannelId { get; set; } // Channel's internal database ID
    public Guid UserId { get; set; } // Public UserId
    public MemberRole Role { get; set; }
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastUpdatedAt { get; set; } // In case role is updated
}