using ChannelService.Domain.Enums;

namespace ChannelService.Application.Contracts.Members;

public class MemberResponse
{
    public Guid ChannelId { get; set; } // Public ChannelId
    public Guid UserId { get; set; } // Public UserId
    public MemberRole Role { get; set; }
    public DateTime JoinedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
}