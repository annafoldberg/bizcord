using ChannelService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChannelService.Infrastructure.Persistence.Configurations;

public sealed class UserBanSagaStateConfiguration : IEntityTypeConfiguration<UserBanSagaState>
{
    public void Configure(EntityTypeBuilder<UserBanSagaState> builder)
    {
        builder.HasKey(s => s.SagaId);

        builder.Property(s => s.UserId).IsRequired();

        builder.Property(s => s.BanSucceeded).IsRequired();

        builder.Property(s => s.MembershipsRemoved).IsRequired();

        builder.Property(s => s.MembershipsRestored).IsRequired();

        builder.Property(s => s.IsCompleted).IsRequired();

        builder.Property(s => s.IsFailed).IsRequired();

        builder.Property(s => s.StartedAt).IsRequired();
    }
}