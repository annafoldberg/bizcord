using ChannelService.Domain.Entities;

namespace ChannelService.Application.Persistence.Repositories;

public interface IChannelRepository : IRepository<Channel>
{
    Task<IEnumerable<Channel>> GetChannelsAsync(CancellationToken ct = default);
    Task<Channel?> GetByPublicIdAsync(Guid id, CancellationToken ct = default);
}