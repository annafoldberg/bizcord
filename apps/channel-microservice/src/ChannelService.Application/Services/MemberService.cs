using ChannelService.Application.Contracts.Members;
using ChannelService.Application.Mappings;
using ChannelService.Application.Persistence.Repositories;
using ChannelService.Application.Services.Results;
using ChannelService.Domain.Entities;
using ChannelService.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace ChannelService.Application.Services;

public sealed class MemberService : IMemberService
{
    private readonly IChannelRepository _channelRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly ILogger<MemberService> _logger;

    public MemberService(IChannelRepository channelRepository, IMemberRepository memberRepository, ILogger<MemberService> logger)
    {
        _channelRepository = channelRepository;
        _memberRepository = memberRepository;
        _logger = logger;
    }

    public async Task<AddMemberResult> AddMemberAsync(Guid channelId, AddMemberRequest request, CancellationToken ct)
    {
        // Retrieve channel
        var channel = await _channelRepository.GetByPublicIdAsync(channelId, ct);

        if (channel is null)
        {
            _logger.LogWarning("Member addition failed because channel {ChannelId} was not found.", channelId);
            return new AddMemberResult { Type = AddMemberResultType.ChannelNotFound };
        }

        // Check if user is already member
        var alreadyMember =
            await _memberRepository.GetByChannelAndUserAsync(channel.Id, request.UserId, ct)
            is not null;

        if (alreadyMember)
        {
            _logger.LogWarning("Member addition failed because user {UserId} is already a member of channel {ChannelId}.", request.UserId, channelId);
            return new AddMemberResult { Type = AddMemberResultType.AlreadyMember };
        }

        // Create and persist member
        var member = new Member
        {
            ChannelId = channel.Id,
            UserId = request.UserId
        };

        await _memberRepository.AddAsync(member, ct);

        _logger.LogInformation("User {UserId} added to channel {ChannelId}.", member.UserId, channelId);

        return new AddMemberResult
        {
            Member = member.ToResponse(channel.PublicId),
            Type = AddMemberResultType.Success
        };
    }

    public async Task<GetChannelMembersResult> GetChannelMembersAsync(Guid channelId, CancellationToken ct)
    {
        var channel = await _channelRepository.GetByPublicIdAsync(channelId, ct);

        if (channel is null)
            return new GetChannelMembersResult { Type = GetChannelMembersResultType.ChannelNotFound };

        var members = await _memberRepository.GetByChannelAsync(channel.Id, ct);
        
        return new GetChannelMembersResult
        {
            Members = members.Select(m => m.ToResponse(channel.PublicId)),
            Type = GetChannelMembersResultType.Success
        };
    }

    public async Task<GetMemberResult> GetMemberAsync(
        Guid channelId,
        Guid userId,
        CancellationToken ct)
    {
        var channel = await _channelRepository.GetByPublicIdAsync(channelId, ct);

        if (channel is null)
            return new GetMemberResult { Type = GetMemberResultType.ChannelNotFound };

        var member = await _memberRepository.GetByChannelAndUserAsync(channel.Id, userId, ct);

        if (member is null)
            return new GetMemberResult { Type = GetMemberResultType.MemberNotFound };

        return new GetMemberResult
        {
            Member = member.ToResponse(channel.PublicId),
            Type = GetMemberResultType.Success
        };
    }

    public async Task<UpdateMemberRoleResult> UpdateMemberRoleAsync(
        Guid channelId,
        Guid userId,
        UpdateMemberRoleRequest request,
        CancellationToken ct)
    {
        // Retrieve channel
        var channel = await _channelRepository.GetByPublicIdAsync(channelId, ct);

        if (channel is null)
        {
            _logger.LogWarning("Member role update failed because channel {ChannelId} was not found.", channelId);
            return UpdateMemberRoleResult.ChannelNotFound;
        }

        // Retrieve member
        var member = await _memberRepository.GetByChannelAndUserAsync(channel.Id, userId, ct);

        if (member is null)
        {
            _logger.LogWarning("Member role update failed because user {UserId} is not a member of channel {ChannelId}.", userId, channelId);
            return UpdateMemberRoleResult.MemberNotFound;
        }

        // Do nothing if role is unchanged
        if (member.Role == request.Role)
            return UpdateMemberRoleResult.Success;

        // If member's current role is owner, check if other owners exist
        if (member.Role == MemberRole.Owner)
        {
            var owners = await _memberRepository.GetByChannelAndRoleAsync(channel.Id, MemberRole.Owner, ct);
            if (!owners.Any(o => o.Id != member.Id))
            {
                _logger.LogWarning("Member role update failed because user {UserId} is the only owner of channel {ChannelId}.", userId, channelId);
                return UpdateMemberRoleResult.CannotDemoteOnlyOwner;
            }
        }

        // Update member role
        member.Role = request.Role;
        member.LastUpdatedAt = DateTime.UtcNow;

        await _memberRepository.UpdateAsync(member, ct);

        _logger.LogInformation("Role for user {UserId} in channel {ChannelId} updated to {Role}.", userId, channelId, member.Role);

        return UpdateMemberRoleResult.Success;
    }

    public async Task<RemoveMemberResult> RemoveMemberAsync(
        Guid channelId,
        Guid userId,
        CancellationToken ct)
    {
        var channel = await _channelRepository.GetByPublicIdAsync(channelId, ct);

        if (channel is null)
        {
            _logger.LogWarning("Member removal failed because channel {ChannelId} was not found.", channelId);
            return RemoveMemberResult.ChannelNotFound;
        }

        var member = await _memberRepository.GetByChannelAndUserAsync(channel.Id, userId, ct);

        if (member is null)
        {
            _logger.LogWarning("Member removal failed because user {UserId} is not a member of channel {ChannelId}.", userId, channelId);
            return RemoveMemberResult.MemberNotFound;
        }

        // If member's role is owner, check if other owners exist
        if (member.Role == MemberRole.Owner)
        {
            var owners = await _memberRepository.GetByChannelAndRoleAsync(channel.Id, MemberRole.Owner, ct);
            if (!owners.Any(o => o.Id != member.Id))
            {
                _logger.LogWarning("Member removal failed because user {UserId} is the only owner of channel {ChannelId}.", userId, channelId);
                return RemoveMemberResult.CannotRemoveOnlyOwner;
            }
        }

        // Remove member
        await _memberRepository.DeleteAsync(member, ct);

        _logger.LogInformation("User {UserId} removed from channel {ChannelId}.", userId, channelId);

        return RemoveMemberResult.Success;
    }
}