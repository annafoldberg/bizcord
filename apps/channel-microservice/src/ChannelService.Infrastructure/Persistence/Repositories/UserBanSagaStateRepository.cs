using ChannelService.Application.Persistence.Repositories;
using ChannelService.Domain.Entities;
using ChannelService.Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ChannelService.Infrastructure.Persistence.Repositories;

public sealed class UserBanSagaStateRepository : IUserBanSagaStateRepository
{
    private readonly IChannelDbContext _context;

    public UserBanSagaStateRepository(IChannelDbContext context)
    {
        _context = context;
    }

    public async Task<UserBanSagaState?> GetByIdAsync(Guid sagaId, CancellationToken ct = default)
    {
        return await _context.UserBanSagaStates.FirstOrDefaultAsync(s => s.SagaId == sagaId, ct);
    }

    public async Task SaveAsync(UserBanSagaState state, CancellationToken ct = default)
    {
        var existingState = await _context.UserBanSagaStates.FirstOrDefaultAsync(s => s.SagaId == state.SagaId, ct);

        if (existingState is null) _context.UserBanSagaStates.Add(state);

        else if (!ReferenceEquals(existingState, state))
        {
            existingState.UserId = state.UserId;
            existingState.BanSucceeded = state.BanSucceeded;
            existingState.MembershipsRemoved = state.MembershipsRemoved;
            existingState.MembershipsRestored = state.MembershipsRestored;
            existingState.IsCompleted = state.IsCompleted;
            existingState.IsFailed = state.IsFailed;
            existingState.StartedAt = state.StartedAt;
            existingState.CompletedAt = state.CompletedAt;
        }

        await _context.SaveChangesAsync(ct);
    }

    public async Task CompleteMembershipRemovalsAsync(Guid sagaId, CancellationToken ct = default)
    {
        var state = await _context.UserBanSagaStates.FirstOrDefaultAsync(s => s.SagaId == sagaId, ct);

        if (state is null) throw new InvalidOperationException($"Saga {sagaId} was not found.");

        if (state.MembershipsRemoved) return;

        var hasRemainingMemberships = await _context.UserBanSagaMemberships.AnyAsync(m => m.SagaId == sagaId && !m.IsRemoved, ct);

        if (hasRemainingMemberships)
            throw new InvalidOperationException($"Saga {sagaId} still has memberships that have not been removed.");

        state.MembershipsRemoved = true;
        
        await _context.SaveChangesAsync(ct);
    }
}