using EasyNetQ;

namespace Messaging;

public static class MessagingServiceCollectionExtensions
{
    public static IServiceCollection AddMessaging(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddEasyNetQ($"host={connectionString}");
        services.AddSingleton<IMessageClient, MessageClient>();

        return services;
    }
}