using ChannelService.Application.Contracts.Channels;
using ChannelService.Application.Contracts.Members;
using ChannelService.Application.Services;
using ChannelService.Application.Services.Results;
using ChannelService.Domain.Enums;

namespace ChannelService.Presentation.GraphQL;

public class ChannelMutation
{
    public async Task<ChannelResponse> CreateChannel(
        CreateChannelRequest request,
        [Service] IChannelService channelService,
        CancellationToken ct)
    {
        return await channelService.CreateChannelAsync(request, ct);
    }

    public async Task<bool> UpdateChannel(
        Guid id,
        UpdateChannelRequest request,
        [Service] IChannelService channelService,
        CancellationToken ct)
    {
        return await channelService.UpdateChannelAsync(id, request, ct);
    }

    public async Task<bool> DeleteChannel(
        Guid id,
        [Service] IChannelService channelService,
        CancellationToken ct)
    {
        return await channelService.DeleteChannelAsync(id, ct);
    }

    public async Task<MemberResponse> AddMember(
        Guid channelId,
        AddMemberRequest request,
        [Service] IMemberService memberService,
        CancellationToken ct)
    {
        var result = await memberService.AddMemberAsync(channelId, request, ct);

        return result.Type switch
        {
            AddMemberResultType.Success => result.Member!,
            AddMemberResultType.ChannelNotFound =>
                throw new GraphQLException("Channel was not found."),
            AddMemberResultType.AlreadyMember =>
                throw new GraphQLException("User is already a member of this channel."),
            _ => throw new GraphQLException("Failed to add member.")
        };
    }

    public async Task<MemberResponse> AddMemberWithRole(
        Guid channelId,
        AddMemberRequest request,
        MemberRole role,
        [Service] IMemberService memberService,
        CancellationToken ct)
    {
        var addResult = await memberService.AddMemberAsync(channelId, request, ct);

        switch (addResult.Type)
        {
            case AddMemberResultType.ChannelNotFound:
                throw new GraphQLException("Channel was not found.");

            case AddMemberResultType.AlreadyMember:
                throw new GraphQLException("User is already a member of this channel.");

            case AddMemberResultType.Success:
                break;

            default:
                throw new GraphQLException("Failed to add member.");
        }

        var updateResult = await memberService.UpdateMemberRoleAsync(
            channelId,
            request.UserId,
            new UpdateMemberRoleRequest { Role = role },
            ct);

        if (updateResult != UpdateMemberRoleResult.Success)
            throw new GraphQLException(
                $"Member was added, but role update failed: {updateResult}.");

        var memberResult = await memberService.GetMemberAsync(
            channelId,
            request.UserId,
            ct);

        if (memberResult.Type != GetMemberResultType.Success ||
            memberResult.Member is null)
        {
            throw new GraphQLException(
                "Member was added and role updated, but could not be retrieved.");
        }

        return memberResult.Member;
    }

    public async Task<bool> UpdateMemberRole(
        Guid channelId,
        Guid userId,
        UpdateMemberRoleRequest request,
        [Service] IMemberService memberService,
        CancellationToken ct)
    {
        var result = await memberService.UpdateMemberRoleAsync(
            channelId,
            userId,
            request,
            ct);

        return result switch
        {
            UpdateMemberRoleResult.Success => true,
            UpdateMemberRoleResult.ChannelNotFound =>
                throw new GraphQLException("Channel was not found."),
            UpdateMemberRoleResult.MemberNotFound =>
                throw new GraphQLException("Member was not found."),
            UpdateMemberRoleResult.CannotDemoteOnlyOwner =>
                throw new GraphQLException("Cannot demote only channel owner."),
            _ => throw new GraphQLException("Failed to update member role.")
        };
    }

    public async Task<bool> RemoveMember(
        Guid channelId,
        Guid userId,
        [Service] IMemberService memberService,
        CancellationToken ct)
    {
        var result = await memberService.RemoveMemberAsync(
            channelId,
            userId,
            ct);

        return result switch
        {
            RemoveMemberResult.Success => true,
            RemoveMemberResult.ChannelNotFound =>
                throw new GraphQLException("Channel was not found."),
            RemoveMemberResult.MemberNotFound =>
                throw new GraphQLException("Member was not found."),
            RemoveMemberResult.CannotRemoveOnlyOwner =>
                throw new GraphQLException("Cannot remove only channel owner."),
            _ => throw new GraphQLException("Failed to remove member.")
        };
    }
}