using ChannelService.Application.Persistence.Repositories;
using ChannelService.Domain.Entities;
using ChannelService.Domain.Enums;
using ChannelService.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ChannelService.Infrastructure.Persistence.Repositories;

public sealed class MemberRepository : IMemberRepository
{
    private readonly IChannelDbContext _context;

    public MemberRepository(IChannelDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Member member, CancellationToken ct = default)
    {
        _context.Members.Add(member);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<Member>> GetByChannelAsync(int channelId, CancellationToken ct = default)
    {
        return await _context.Members.Where(m => m.ChannelId == channelId).ToListAsync(ct);
    }

    public async Task<Member?> GetByChannelAndUserAsync(int channelId, Guid userId, CancellationToken ct = default)
    {
        return await _context.Members.FirstOrDefaultAsync(m => m.ChannelId == channelId && m.UserId == userId, ct);
    }

    public async Task<IEnumerable<Member>> GetByChannelAndRoleAsync(int channelId, MemberRole role, CancellationToken ct = default)
    {
        return await _context.Members
            .Where(m => m.ChannelId == channelId &&
                        m.Role == role)
            .ToListAsync(ct);
    }

    public async Task UpdateAsync(Member member, CancellationToken ct = default)
    {
        _context.Members.Update(member);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Member member, CancellationToken ct = default)
    {
        _context.Members.Remove(member);
        await _context.SaveChangesAsync(ct);
    }

}