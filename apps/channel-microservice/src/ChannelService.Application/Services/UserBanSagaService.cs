using ChannelService.Application.Persistence.Repositories;
using ChannelService.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace ChannelService.Application.Services;

public sealed class UserBanSagaService : IUserBanSagaService
{
    private readonly IUserBanSagaStateRepository _stateRepository;
    private readonly IUserBanSagaMembershipRepository _membershipRepository;
    private readonly ILogger<UserBanSagaService> _logger;

    public UserBanSagaService(
        IUserBanSagaStateRepository stateRepository,
        IUserBanSagaMembershipRepository membershipRepository,
        ILogger<UserBanSagaService> logger)
    {
        _stateRepository = stateRepository;
        _membershipRepository = membershipRepository;
        _logger = logger;
    }

    public async Task<IReadOnlyList<UserBanSagaMembership>> InitializeAsync(Guid sagaId, Guid userId, CancellationToken ct = default)
    {
        // Retrieve saga state
        var state = await _stateRepository.GetByIdAsync(sagaId, ct);

        // If saga state doesn't exist, create it
        if (state is null)
        {
            state = new UserBanSagaState
            {
                SagaId = sagaId,
                UserId = userId,
                BanSucceeded = true
            };

            await _stateRepository.SaveAsync(state, ct);
        }
        // If saga state exists, ensure that it doesn't belong to another user
        else if (state.UserId != userId)
            throw new InvalidOperationException($"Saga {sagaId} belongs to another user.");

        // Prevent restarting a compensated saga
        if (state.MembershipsRestored || state.IsFailed)
            throw new InvalidOperationException($"Saga {sagaId} has already been compensated.");

        // Persist original memberships
        var originalMemberships = await _membershipRepository.PersistOriginalMembershipsAsync(sagaId, userId, ct);

        _logger.LogInformation(
            "Initialized membership removal for saga {SagaId} with {Count} memberships.",
            sagaId,
            originalMemberships.Count);

        return originalMemberships;
    }

    public async Task RemoveMembershipAsync(Guid sagaId, int channelId, CancellationToken ct = default)
    {
        // Retrieve saga state
        var state = await _stateRepository.GetByIdAsync(sagaId, ct);

        if (state is null) throw new InvalidOperationException($"Saga {sagaId} was not found.");

        // Prevent removal after completion or compensation
        if (state.MembershipsRemoved || state.MembershipsRestored || state.IsFailed)
            throw new InvalidOperationException($"Saga {sagaId} is no longer eligible for membership removal.");

        // Remove membership
        await _membershipRepository.RemoveMembershipAsync(sagaId, channelId, ct);

        _logger.LogInformation("Removed membership for channel {ChannelId} in saga {SagaId}.", channelId, sagaId);
    }

    public async Task CompleteMembershipRemovalsAsync(Guid sagaId, CancellationToken ct = default)
    {
        // Retrieve saga state
        var state = await _stateRepository.GetByIdAsync(sagaId, ct);

        if (state is null) throw new InvalidOperationException($"Saga {sagaId} was not found.");

        // Prevent completion after compensation or failure
        if (state.MembershipsRestored || state.IsFailed)
            throw new InvalidOperationException($"Saga {sagaId} cannot complete membership removals.");

        // Return if already completed
        if (state.MembershipsRemoved) return;

        // Mark membership removals as completed
        await _stateRepository.CompleteMembershipRemovalsAsync(sagaId, ct);

        _logger.LogInformation("Completed membership removals for saga {SagaId}.", sagaId);
    }

    public async Task<IReadOnlyList<UserBanSagaMembership>> GetRemovedMembershipsAsync(Guid sagaId, CancellationToken ct = default)
    {
        // Retrieve saga state
        var state = await _stateRepository.GetByIdAsync(sagaId, ct);

        if (state is null) throw new InvalidOperationException($"Saga {sagaId} was not found.");

        // Retrieve removed memberships
        return await _membershipRepository.GetRemovedMembershipsAsync(sagaId, ct);
    }

    public async Task RestoreMembershipAsync(Guid sagaId, int channelId, CancellationToken ct = default)
    {
        // Retrieve saga state
        var state = await _stateRepository.GetByIdAsync(sagaId, ct);

        if (state is null) throw new InvalidOperationException($"Saga {sagaId} was not found.");

        // Prevent restoration after successful completion
        if (state.MembershipsRemoved) throw new InvalidOperationException($"Saga {sagaId} has already completed membership removals.");

        // Return if compensation is already completed
        if (state.MembershipsRestored) return;

        // Restore membership
        await _membershipRepository.RestoreMembershipAsync(sagaId, channelId, ct);

        _logger.LogInformation("Restored membership for channel {ChannelId} in saga {SagaId}.", channelId, sagaId);
    }

    public async Task CompleteCompensationAsync(Guid sagaId, CancellationToken ct = default)
    {
        // Retrieve saga state
        var state = await _stateRepository.GetByIdAsync(sagaId, ct);

        if (state is null) throw new InvalidOperationException($"Saga {sagaId} was not found.");

        // Return if compensation is already completed
        if (state.MembershipsRestored) return;

        // Prevent compensation after successful completion
        if (state.MembershipsRemoved) throw new InvalidOperationException($"Saga {sagaId} has already completed membership removals.");

        // Ensure all memberships have been restored
        var remainingMemberships = await _membershipRepository.GetRemovedMembershipsAsync(sagaId, ct);

        if (remainingMemberships.Count > 0)
            throw new InvalidOperationException($"Saga {sagaId} still has memberships that have not been restored.");

        // Mark compensation as completed
        state.MembershipsRestored = true;
        state.MembershipsRemoved = false;
        state.IsFailed = true;
        state.CompletedAt = DateTime.UtcNow;

        await _stateRepository.SaveAsync(state, ct);

        _logger.LogInformation("Membership compensation completed for saga {SagaId}.", sagaId);
    }
}