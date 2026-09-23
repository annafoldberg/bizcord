namespace ChannelService.Application.Services.Results;

public enum UpdateMemberRoleResult
{
    Success,
    ChannelNotFound,
    MemberNotFound,
    CannotDemoteOnlyOwner
}