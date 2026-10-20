using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ChannelService.Application.Messaging;

public class MessageBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public MessageBackgroundService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();

        var subscriptionManager = scope.ServiceProvider.GetRequiredService<IMessageHandlerSubscriptionManager>();

        await subscriptionManager.SubscribeAsync(stoppingToken);

        try
        {
            // Application is running
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            // Application is stopping
        }
        finally
        {
            await subscriptionManager.UnsubscribeAsync();
        }
    }
}