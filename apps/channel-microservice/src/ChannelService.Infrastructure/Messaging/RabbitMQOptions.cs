namespace ChannelService.Infrastructure.Messaging;

/// <summary>
/// Configuration options for RabbitMQ connection.
/// </summary>
public sealed class RabbitMQOptions
{
    public const string SectionName = "RabbitMQ";
    public string Host { get; init; } = string.Empty;
}