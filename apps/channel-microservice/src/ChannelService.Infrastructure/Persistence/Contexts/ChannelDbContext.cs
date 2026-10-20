using ChannelService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChannelService.Infrastructure.Persistence.Contexts;

/// <summary>
/// Entity Framework Core database context for Channel Service.
/// </summary>
public sealed class ChannelDbContext : DbContext, IChannelDbContext
{
    public ChannelDbContext(DbContextOptions<ChannelDbContext> options) : base(options) { }

    public DbSet<Channel> Channels { get; set; }
    public DbSet<Member> Members { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ChannelDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}