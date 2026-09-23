using ChannelService.Application.Contracts.Channels;
using ChannelService.Application.Mappings;
using ChannelService.Application.Persistence.Repositories;
using ChannelService.Domain.Entities;
using ChannelService.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace ChannelService.Application.Services;

public sealed class ChannelService : IChannelService
{
    private readonly IChannelRepository _channelRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly ILogger<ChannelService> _logger;

    public ChannelService(IChannelRepository channelRepository, IMemberRepository memberRepository, ILogger<ChannelService> logger)
    {
        _channelRepository = channelRepository;
        _memberRepository = memberRepository;
        _logger = logger;
    }

    public async Task<ChannelResponse> CreateChannelAsync(CreateChannelRequest request, CancellationToken ct)
    {
        var channel = request.ToEntity();
        await _channelRepository.AddAsync(channel, ct);

        // Add user as owner of the channel
        var owner = new Member
        {
            ChannelId = channel.Id,
            UserId = request.UserId,
            Role = MemberRole.Owner
        };

        await _memberRepository.AddAsync(owner, ct);

        _logger.LogInformation("Channel {ChannelId} created with owner {OwnerId}.", channel.PublicId, owner.UserId);
        
        return channel.ToResponse();
    }

    public async Task<IEnumerable<ChannelResponse>> GetChannelsAsync(CancellationToken ct)
    {
        var channels = await _channelRepository.GetChannelsAsync(ct);

        return channels.Select(c => c.ToResponse());
    }

    public async Task<ChannelResponse?> GetChannelAsync(Guid id, CancellationToken ct)
    {
        var channel = await _channelRepository.GetByPublicIdAsync(id, ct);

        if (channel is null) return null;

        return channel.ToResponse();
    }

    public async Task<bool> UpdateChannelAsync(Guid id, UpdateChannelRequest request, CancellationToken ct)
    {
        var channel = await _channelRepository.GetByPublicIdAsync(id, ct);

        if (channel is null)
        {
            _logger.LogWarning("Channel update failed because channel {ChannelId} was not found.", id);
            return false;
        }

        var isUpdated = false;

        if (request.Name is not null)
        {
            channel.Name = request.Name;
            isUpdated = true;
        }

        if (request.Description is not null)
        {
            channel.Description = request.Description;
            isUpdated = true;
        }
        
        if (isUpdated)
        {
            channel.LastUpdatedAt = DateTime.UtcNow;
            await _channelRepository.UpdateAsync(channel, ct);
            _logger.LogInformation("Channel {ChannelId} updated.", channel.PublicId);
        }

        return true;
    }

    public async Task<bool> DeleteChannelAsync(Guid id, CancellationToken ct)
    {
        var channel = await _channelRepository.GetByPublicIdAsync(id, ct);

        if (channel is null)
        {
            _logger.LogWarning("Channel deletion failed because channel {ChannelId} was not found.", id);
            return false;
        }

        await _channelRepository.DeleteAsync(channel, ct);

        _logger.LogInformation("Channel {ChannelId} deleted.", channel.PublicId);

        return true;
    }
}