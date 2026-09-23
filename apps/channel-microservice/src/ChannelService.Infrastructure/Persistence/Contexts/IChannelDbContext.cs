using ChannelService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChannelService.Infrastructure.Persistence.Contexts;

public interface IChannelDbContext
{
    DbSet<Channel> Channels { get; }
    DbSet<Member> Members { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default );
}