using ChannelService.Application.Contracts.Channels;
using ChannelService.Application.Contracts.Members;

namespace ChannelService.Presentation.GraphQL.Responses;

public class ChannelWithMembersResponse
{
    public required ChannelResponse Channel { get; set; }
    public required IEnumerable<MemberResponse> Members { get; set; }
}