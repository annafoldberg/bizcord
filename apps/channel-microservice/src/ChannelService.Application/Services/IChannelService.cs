using ChannelService.Application.Contracts.Channels;

namespace ChannelService.Application.Services;

public interface IChannelService
{
    Task<ChannelResponse> CreateChannelAsync(CreateChannelRequest request, CancellationToken ct);
    Task<IEnumerable<ChannelResponse>> GetChannelsAsync(CancellationToken ct);
    Task<ChannelResponse?> GetChannelAsync(Guid id, CancellationToken ct);
    Task<bool> UpdateChannelAsync(Guid id, UpdateChannelRequest request, CancellationToken ct);
    Task<bool> DeleteChannelAsync(Guid id, CancellationToken ct);
}