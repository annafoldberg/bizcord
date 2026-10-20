using Microsoft.Extensions.DependencyInjection;

namespace ChannelService.Application.Messaging;

public static class MessagingServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationMessaging(this IServiceCollection services)
    {
        services.AddScoped<IMessageHandlerSubscriptionManager, MessageHandlerSubscriptionManager>();

        services.AddMessageHandlers();

        services.AddHostedService<MessageBackgroundService>();

        return services;
    }
}