using ChannelService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChannelService.Infrastructure.Persistence.Configurations;

public sealed class UserBanSagaMembershipConfiguration
    : IEntityTypeConfiguration<UserBanSagaMembership>
{
    public void Configure(EntityTypeBuilder<UserBanSagaMembership> builder)
    {
        builder.HasKey(m => m.Id);

        builder.HasIndex(m => new { m.SagaId, m.ChannelId }).IsUnique();

        builder.Property(m => m.SagaId).IsRequired();

        builder.Property(m => m.ChannelId).IsRequired();

        builder.Property(m => m.Role).IsRequired();

        builder.Property(m => m.JoinedAt).IsRequired();

        builder.HasOne<UserBanSagaState>().WithMany().HasForeignKey(m => m.SagaId);
    }
}