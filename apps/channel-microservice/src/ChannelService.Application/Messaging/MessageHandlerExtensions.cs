using Microsoft.Extensions.DependencyInjection;

namespace ChannelService.Application.Messaging;

public static class MessageHandlerExtensions
{
    public static IServiceCollection AddMessageHandlers(this IServiceCollection services)
    {
        // Get the assembly containing IMessageHandler
        var assembly = typeof(IMessageHandler<>).Assembly;

        // Get all concrete classes in the assembly
        var concreteTypes = assembly.GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false });

        foreach (var concreteType in concreteTypes)
        {
            // Check if the class implements IMessageHandler<T>
            var handlerInterfaces = concreteType.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMessageHandler<>));
            
            foreach (var handlerInterface in handlerInterfaces)
            {
                // Register IMessageHandler<T> with its implementation
                services.AddScoped(handlerInterface, concreteType);

                // Register MessageHandler information for background service
                services.AddSingleton(new MessageHandlerMetadata
                {
                    MessageType = handlerInterface.GetGenericArguments()[0],
                    HandlerType = concreteType
                });
            }
        }

        return services;
    }
}