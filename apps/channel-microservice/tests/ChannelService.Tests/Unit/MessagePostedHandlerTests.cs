using ChannelService.Application.Messaging.Handlers;
using ChannelService.Application.Services;
using ChannelService.Tests.Fakes;
using FluentAssertions;
using Moq;
using Shared.Contracts.Events;

namespace ChannelService.Tests.Unit;

public class MessagePostedHandlerTests
{
    [Fact]
    public async Task Handle_PublishesResultEvent_WithCorrectMessageId()
    {
        // Arrange
        var messageId = Guid.NewGuid();

        var mockService = new Mock<IChannelService>();

        mockService
            .Setup(s => s.UpdateLastActivityAsync(
                It.IsAny<Guid>(),
                It.IsAny<DateTime>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var client = new FakeMessageClient();
        
        var handler = new MessagePostedHandler(client, mockService.Object);

        // Act
        await handler.HandleAsync(new MessagePostedEvent
        {
            MessageId = messageId,
            ChannelId = Guid.NewGuid(),
            AuthorId = Guid.NewGuid(),
            Content = "Hello world",
            PostedAt = DateTime.UtcNow
        }, CancellationToken.None);

        var published = client.SinglePublished<ActivityProcessedEvent>();

        // Assert
        published.Should().NotBeNull();
        published!.MessageId.Should().Be(messageId);
    }
}