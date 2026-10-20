using ChannelService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChannelService.Infrastructure.Persistence.Configurations;

public sealed class ChannelConfiguration : IEntityTypeConfiguration<Channel>
{
    public void Configure(EntityTypeBuilder<Channel> builder)
    {
        builder.HasKey(c => c.Id);

        builder.HasIndex(c => c.PublicId).IsUnique();

        builder.HasIndex(c => c.Name).IsUnique();

        builder.Property(c => c.PublicId).IsRequired();

        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);

        builder.Property(c => c.Description).HasMaxLength(400);

        builder.Property(c => c.CreatedAt).IsRequired();
    }
}