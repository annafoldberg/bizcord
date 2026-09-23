using ChannelService.Application.Contracts.Members;
using ChannelService.Application.Services.Results;

namespace ChannelService.Application.Services;

public interface IMemberService
{
    Task<AddMemberResult> AddMemberAsync(Guid channelId, AddMemberRequest request, CancellationToken ct);
    Task<GetChannelMembersResult> GetChannelMembersAsync(Guid channelId, CancellationToken ct);
    Task<GetMemberResult> GetMemberAsync(Guid channelId, Guid memberId, CancellationToken ct);
    Task<UpdateMemberRoleResult> UpdateMemberRoleAsync(
        Guid channelId,
        Guid memberId,
        UpdateMemberRoleRequest request,
        CancellationToken ct);
    Task<RemoveMemberResult> RemoveMemberAsync(Guid channelId, Guid memberId, CancellationToken ct);
}