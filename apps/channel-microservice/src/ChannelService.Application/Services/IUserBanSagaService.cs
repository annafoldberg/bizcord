using ChannelService.Domain.Entities;

namespace ChannelService.Application.Services;

public interface IUserBanSagaService
{
    // Create or retrieve saga state and persist original memberships
    Task<IReadOnlyList<UserBanSagaMembership>> InitializeAsync(Guid sagaId, Guid userId, CancellationToken ct = default);

    // Remove one membership
    Task RemoveMembershipAsync(Guid sagaId, int channelId, CancellationToken ct = default);

    // Mark the saga's membership removal step as successful
    Task CompleteMembershipRemovalsAsync(Guid sagaId, CancellationToken ct = default);

    // Get all removed memberships
    Task<IReadOnlyList<UserBanSagaMembership>> GetRemovedMembershipsAsync(Guid sagaId, CancellationToken ct = default);

    // Restore one removed membership
    Task RestoreMembershipAsync(Guid sagaId, int channelId, CancellationToken ct = default);

    // Mark the saga's compensation as successful
    Task CompleteCompensationAsync(Guid sagaId, CancellationToken ct = default);
}