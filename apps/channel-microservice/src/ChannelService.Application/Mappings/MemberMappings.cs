using ChannelService.Application.Contracts.Members;
using ChannelService.Domain.Entities;

namespace ChannelService.Application.Mappings;

public static class MemberMappings
{
    public static MemberResponse ToResponse(this Member member, Guid publicChannelId)
    {
        return new MemberResponse
        {
            ChannelId = publicChannelId,
            UserId = member.UserId,
            Role = member.Role,
            JoinedAt = member.JoinedAt,
            LastUpdatedAt = member.LastUpdatedAt
        };
    }
}