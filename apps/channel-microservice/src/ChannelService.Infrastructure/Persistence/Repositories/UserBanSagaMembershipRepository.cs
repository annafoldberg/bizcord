using ChannelService.Application.Persistence.Repositories;
using ChannelService.Domain.Entities;
using ChannelService.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ChannelService.Infrastructure.Persistence.Repositories;

public sealed class UserBanSagaMembershipRepository : IUserBanSagaMembershipRepository
{
    private readonly IChannelDbContext _context;

    public UserBanSagaMembershipRepository(IChannelDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<UserBanSagaMembership>> PersistOriginalMembershipsAsync(Guid sagaId, Guid userId, CancellationToken ct = default)
    {
        // Return existing original membership if already persisted
        var originalMemberships = await _context.UserBanSagaMemberships.Where(m => m.SagaId == sagaId).ToListAsync(ct);

        if (originalMemberships.Count > 0) return originalMemberships;

        // Retrieve current memberships
        var members = await _context.Members.Where(m => m.UserId == userId).ToListAsync(ct);

        // Define original memberships
        originalMemberships = members.Select(m => new UserBanSagaMembership
        {
            SagaId = sagaId,
            ChannelId = m.ChannelId,
            Role = m.Role,
            JoinedAt = m.JoinedAt,
            IsRemoved = false
        }).ToList();

        _context.UserBanSagaMemberships.AddRange(originalMemberships);

        // Persist original memberships
        await _context.SaveChangesAsync(ct);

        return originalMemberships;
    }

    public async Task RemoveMembershipAsync(Guid sagaId, int channelId, CancellationToken ct = default)
    {
        // Retrieve original membership
        var originalMembership = await _context.UserBanSagaMemberships.FirstOrDefaultAsync(m => m.SagaId == sagaId && m.ChannelId == channelId, ct);

        if (originalMembership is null)
            throw new InvalidOperationException($"Original membership for channel {channelId} was not found in saga {sagaId}.");

        // Return if already removed
        if (originalMembership.IsRemoved) return;

        // Retrieve current membership
        var state = await _context.UserBanSagaStates.FirstOrDefaultAsync(s => s.SagaId == sagaId, ct);

        if (state is null)
            throw new InvalidOperationException($"Saga {sagaId} was not found.");

        var member = await _context.Members.FirstOrDefaultAsync(m => m.ChannelId == channelId && m.UserId == state.UserId, ct);

        if (member is null)
            throw new InvalidOperationException($"Membership for user {state.UserId} in channel {channelId} was not found.");

        // Remove membership
        _context.Members.Remove(member);
        originalMembership.IsRemoved = true;

        await _context.SaveChangesAsync(ct);
    }

    public async Task RestoreMembershipAsync(Guid sagaId, int channelId, CancellationToken ct = default)
    {
        // Retrieve original membership
        var originalMembership = await _context.UserBanSagaMemberships
            .FirstOrDefaultAsync(m => m.SagaId == sagaId && m.ChannelId == channelId, ct);

        if (originalMembership is null)
            throw new InvalidOperationException($"Original membership for channel {channelId} was not found in saga {sagaId}.");

        // Return if not removed
        if (!originalMembership.IsRemoved) return;

        // Retrieve saga state
        var state = await _context.UserBanSagaStates.FirstOrDefaultAsync(s => s.SagaId == sagaId, ct);

        if (state is null) throw new InvalidOperationException($"Saga {sagaId} was not found.");

        // Check if membership already exists
        var exists = await _context.Members.AnyAsync(m => m.ChannelId == channelId && m.UserId == state.UserId, ct);

        if (exists) throw new InvalidOperationException($"Membership for user {state.UserId} in channel {channelId} already exists.");

        // Restore original membership
        _context.Members.Add(new Member
        {
            ChannelId = originalMembership.ChannelId,
            UserId = state.UserId,
            Role = originalMembership.Role,
            JoinedAt = originalMembership.JoinedAt
        });
        originalMembership.IsRemoved = false;

        await _context.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<UserBanSagaMembership>> GetRemovedMembershipsAsync(Guid sagaId, CancellationToken ct = default)
    {
        return await _context.UserBanSagaMemberships.Where(m => m.SagaId == sagaId && m.IsRemoved).ToListAsync(ct);
    }
}