using ChannelService.Domain.Entities;

namespace ChannelService.Application.Persistence.Repositories;

public interface IUserBanSagaStateRepository
{
    // Retrieve saga state
    Task<UserBanSagaState?> GetByIdAsync(Guid sagaId, CancellationToken ct = default);

    // Persist saga state
    Task SaveAsync(UserBanSagaState state, CancellationToken ct = default);

    // Mark membership removal as completed
    Task CompleteMembershipRemovalsAsync(Guid sagaId, CancellationToken ct = default);
}