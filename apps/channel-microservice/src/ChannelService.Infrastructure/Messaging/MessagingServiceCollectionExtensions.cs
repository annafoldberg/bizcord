using MessageClient.Configuration;
using MessageClient.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChannelService.Infrastructure.Messaging;

public static class MessagingServiceCollectionExtensions
{
    public static IServiceCollection AddRabbitMQMessaging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<RabbitMQOptions>()
            .Bind(configuration.GetSection(RabbitMQOptions.SectionName))
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.Host),
                "RabbitMQ configuration is incomplete.")
            .ValidateOnStart();

        var rabbitMQOptions = configuration
            .GetSection(RabbitMQOptions.SectionName)
            .Get<RabbitMQOptions>()
            ?? throw new InvalidOperationException(
                "RabbitMQ configuration is missing.");

        services.AddMessageClient(options =>
        {
            options.MessagingProvider = MessagingProvider.RabbitMQ;
            options.ConnectionString = $"host={rabbitMQOptions.Host}";
        });

        return services;
    }
}