using ChannelService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChannelService.Infrastructure.Persistence.Configurations;

public sealed class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.HasKey(m => m.Id);

        builder.HasIndex(m => new { m.ChannelId, m.UserId }).IsUnique();

        builder.Property(m => m.ChannelId).IsRequired();

        builder.Property(m => m.UserId).IsRequired();

        builder.Property(m => m.Role).IsRequired();

        builder.Property(m => m.JoinedAt).IsRequired();

        builder.HasOne<Channel>().WithMany().HasForeignKey(m => m.ChannelId);
    }
}