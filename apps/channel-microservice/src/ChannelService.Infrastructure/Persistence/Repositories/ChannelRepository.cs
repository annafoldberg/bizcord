using Microsoft.EntityFrameworkCore;
using ChannelService.Application.Persistence.Repositories;
using ChannelService.Infrastructure.Persistence.Contexts;
using ChannelService.Domain.Entities;

namespace ChannelService.Infrastructure.Persistence.Repositories;

public sealed class ChannelRepository : IChannelRepository
{
    private readonly IChannelDbContext _context;

    public ChannelRepository(IChannelDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Channel channel, CancellationToken ct = default)
    {
        _context.Channels.Add(channel);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<Channel>> GetChannelsAsync(CancellationToken ct = default)
    {
        return await _context.Channels.ToListAsync(ct);
    }

    public async Task<Channel?> GetByPublicIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Channels.FirstOrDefaultAsync(c => c.PublicId == id, ct);
    }

    public async Task UpdateAsync(Channel channel, CancellationToken ct = default)
    {
        _context.Channels.Update(channel);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Channel channel, CancellationToken ct = default)
    {
        _context.Channels.Remove(channel);
        await _context.SaveChangesAsync(ct);
    }
}