using ChannelService.Domain.Enums;
using ChannelService.Domain.Entities;

namespace ChannelService.Application.Persistence.Repositories;

public interface IMemberRepository : IRepository<Member>
{
    Task<IEnumerable<Member>> GetByChannelAsync(int channelId, CancellationToken ct = default);
    Task<Member?> GetByChannelAndUserAsync(int channelId, Guid userId, CancellationToken ct = default);
    Task<IEnumerable<Member>> GetByChannelAndRoleAsync(int channelId, MemberRole role, CancellationToken ct = default);
}