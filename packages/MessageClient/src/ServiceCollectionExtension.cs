using Microsoft.Extensions.DependencyInjection;
using MessageClient.Configuration;
using MessageClient.Factories;
using MessageClient.Interfaces;
using MessageClient.Implementation;

namespace MessageClient.Extensions;

public static class ServiceCollectionExtension {
    
    public static IServiceCollection AddMessageClient(
        this IServiceCollection services,
        Action<MessageClientOptions> configureOptions
    ){
        MessageClientOptions options = new MessageClientOptions();
        configureOptions(options);

        IMessageClient? messagingClient;
        switch(options.MessagingProvider){
            case MessagingProvider.RabbitMQ:
                messagingClient = RabbitMQFactory.Create(options);
                services.AddSingleton<IMessageClient>(
                    sp => messagingClient
                );
                break;
            default:
                throw new ArgumentException("");
        }
        return services;
    }
    
    public static IServiceCollection AddMessageHandler(
        this IServiceCollection services,
        Action<MessageClientOptions> configureClientOptions
    ){
        services.AddMessageClient(configureClientOptions);
        
        return services;
    }
}