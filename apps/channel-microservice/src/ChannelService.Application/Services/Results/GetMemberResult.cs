using ChannelService.Application.Contracts.Members;

namespace ChannelService.Application.Services.Results;

public sealed class GetMemberResult
{
    public MemberResponse? Member { get; init; }
    public GetMemberResultType Type { get; init; }
}