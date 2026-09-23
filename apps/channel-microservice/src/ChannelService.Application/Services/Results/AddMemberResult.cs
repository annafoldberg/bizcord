using ChannelService.Application.Contracts.Members;

namespace ChannelService.Application.Services.Results;

public sealed class AddMemberResult
{
    public MemberResponse? Member { get; init; }
    public AddMemberResultType Type { get; init; }
}