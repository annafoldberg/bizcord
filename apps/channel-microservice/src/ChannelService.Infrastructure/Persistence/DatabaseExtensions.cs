using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using ChannelService.Infrastructure.Persistence.Contexts;

namespace ChannelService.Infrastructure.Persistence;

public static class DatabaseExtensions
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<DatabaseOptions>()
            .Bind(configuration.GetSection(DatabaseOptions.SectionName))
            .Validate(options =>
                !string.IsNullOrWhiteSpace(options.Host) &&
                !string.IsNullOrWhiteSpace(options.Name) &&
                !string.IsNullOrWhiteSpace(options.User) &&
                !string.IsNullOrWhiteSpace(options.Password),
                "Database configuration is incomplete.")
            .ValidateOnStart();

        services.AddDbContext<ChannelDbContext>((serviceProvider, options) =>
        {
            var databaseOptions = serviceProvider
                .GetRequiredService<IOptions<DatabaseOptions>>()
                .Value;

            var connectionString =
                $"Host={databaseOptions.Host};" +
                $"Database={databaseOptions.Name};" +
                $"Username={databaseOptions.User};" +
                $"Password={databaseOptions.Password}";
                
            options.UseNpgsql(connectionString);
        });

        return services;
    }

    public static async Task ApplyMigrationsAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<ChannelDbContext>();

        await dbContext.Database.MigrateAsync();
    }
}