namespace ChannelService.Application.Services.Results;

public enum RemoveMemberResult
{
    Success,
    ChannelNotFound,
    MemberNotFound,
    CannotRemoveOnlyOwner
}