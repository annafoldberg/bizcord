using ChannelService.Domain.Entities;

namespace ChannelService.Application.Persistence.Repositories;

public interface IUserBanSagaMembershipRepository
{
    // Persist original memberships in case of compensation
    Task<IReadOnlyList<UserBanSagaMembership>> PersistOriginalMembershipsAsync(Guid sagaId, Guid userId, CancellationToken ct = default);

    // Remove one membership
    Task RemoveMembershipAsync(Guid sagaId, int channelId, CancellationToken ct = default);

    // Get all removed memberships
    Task<IReadOnlyList<UserBanSagaMembership>> GetRemovedMembershipsAsync(Guid sagaId, CancellationToken ct = default);

    // Restore one previously removed membership
    Task RestoreMembershipAsync(Guid sagaId, int channelId, CancellationToken ct = default);
}