using ChannelService.Application.Messaging;
using ChannelService.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ChannelService.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IChannelService, ChannelService.Application.Services.ChannelService>();
        services.AddScoped<IMemberService, MemberService>();

        services.AddApplicationMessaging();

        return services;
    }
}