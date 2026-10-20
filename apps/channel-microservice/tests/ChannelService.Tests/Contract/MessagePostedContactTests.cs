using ChannelService.Application.Messaging.Handlers;
using ChannelService.Application.Services;
using ChannelService.Tests.Fakes;
using FluentAssertions;
using Moq;
using Shared.Contracts.Events;

namespace ChannelService.Tests.Contracts;

public class MessagePostedContractTests
{
    // If the event is changed and these tests fail, you know
    // the contract is broken before it reaches production
    [Fact]
    public async Task Handler_CanConsume_MinimumValidContract()
    {
        // Arrange
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
        Func<Task> act = () => handler.HandleAsync(new MessagePostedEvent
        {
            MessageId = Guid.NewGuid(),
            ChannelId = Guid.NewGuid(),
            AuthorId = Guid.NewGuid(),
            Content = "Hello world",
            PostedAt = DateTime.UtcNow
        }, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
        client.Published.Should().HaveCount(1);
    }
}