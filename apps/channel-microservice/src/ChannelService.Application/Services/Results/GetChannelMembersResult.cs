using ChannelService.Application.Contracts.Members;

namespace ChannelService.Application.Services.Results;

public sealed class GetChannelMembersResult
{
    public IEnumerable<MemberResponse>? Members { get; init; }
    public GetChannelMembersResultType Type { get; init; }
}