using ChannelService.Application.Contracts.Channels;
using ChannelService.Domain.Entities;
using ChannelService.Infrastructure.Persistence.Contexts;
using ChannelService.Infrastructure.Persistence.Repositories;
using ChannelService.Tests.Helpers;
using FluentAssertions;
using MessageClient.Configuration;
using MessageClient.Factories;
using MessageClient.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.Events;

namespace ChannelService.Tests.Integration;

public class MessagePostedIntegrationTests
{
    [Fact]
    public async Task MessagePostedEvent_IsConsumed_AndResultEventPublished()
    {
        // Arrange
        var messageClientOptions = new MessageClientOptions
        {
            ConnectionString = "host=localhost",
            MessagingProvider = MessagingProvider.RabbitMQ
        };

        IMessageClient client = RabbitMQFactory.Create(messageClientOptions);

        var databaseOptions = new DbContextOptionsBuilder<ChannelDbContext>()
            .UseNpgsql("Host=localhost;Database=channels;Username=postgres;Password=postgres")
            .Options;

        await using var dbContext = new ChannelDbContext(databaseOptions);

        var repository = new ChannelRepository(dbContext);

        var messageId = Guid.NewGuid();
        var channelId = Guid.NewGuid();

        await repository.AddAsync(new Channel
        {
            PublicId = channelId,
            Name = "Integration Test Channel"
        }, CancellationToken.None);

        var capture = new MessageCapture<ActivityProcessedEvent>(client);

        try
        {
            // Act
            await client.PublishAsync(
                "message-posted",
                new MessagePostedEvent
                {
                    MessageId = messageId,
                    ChannelId = channelId,
                    AuthorId = Guid.NewGuid(),
                    Content = "Hello world",
                    PostedAt = DateTime.UtcNow
                });

            var result = await capture.WaitForMessageAsync(TimeSpan.FromSeconds(5));

            // Assert
            result.MessageId.Should().Be(messageId);
        }
        finally
        {
            dbContext.Channels.Remove(await dbContext.Channels.SingleAsync(c => c.PublicId == channelId));

            await dbContext.SaveChangesAsync();
        }
    }
}