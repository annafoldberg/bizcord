using ChannelService.Application.Contracts.Channels;
using ChannelService.Application.Contracts.Members;
using ChannelService.Application.Services;
using ChannelService.Application.Services.Results;
using ChannelService.Presentation.GraphQL.Responses;

namespace ChannelService.Presentation.GraphQL;

public class ChannelQuery
{
    public async Task<IEnumerable<ChannelResponse>> GetChannels(
        [Service] IChannelService channelService,
        CancellationToken ct)
    {
        return await channelService.GetChannelsAsync(ct);
    }

    public async Task<ChannelResponse?> GetChannel(
        Guid id,
        [Service] IChannelService channelService,
        CancellationToken ct)
    {
        return await channelService.GetChannelAsync(id, ct);
    }

    public async Task<ChannelWithMembersResponse?> GetChannelWithMembers(
        Guid id,
        [Service] IChannelService channelService,
        [Service] IMemberService memberService,
        CancellationToken ct)
    {
        var channel = await channelService.GetChannelAsync(id, ct);

        if (channel is null)
            return null;

        var result = await memberService.GetChannelMembersAsync(id, ct);

        if (result.Type != GetChannelMembersResultType.Success)
            return null;

        return new ChannelWithMembersResponse
        {
            Channel = channel,
            Members = result.Members!
        };
    }

    public async Task<IEnumerable<MemberResponse>?> GetChannelMembers(
        Guid channelId,
        [Service] IMemberService memberService,
        CancellationToken ct)
    {
        var result = await memberService.GetChannelMembersAsync(channelId, ct);

        return result.Type == GetChannelMembersResultType.Success
            ? result.Members
            : null;
    }

    public async Task<MemberResponse?> GetMember(
        Guid channelId,
        Guid userId,
        [Service] IMemberService memberService,
        CancellationToken ct)
    {
        var result = await memberService.GetMemberAsync(channelId, userId, ct);

        return result.Type == GetMemberResultType.Success
            ? result.Member
            : null;
    }
}