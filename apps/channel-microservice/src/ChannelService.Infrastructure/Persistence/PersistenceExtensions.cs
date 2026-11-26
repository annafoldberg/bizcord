using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using ChannelService.Infrastructure.Persistence.Contexts;
using ChannelService.Application.Persistence.Repositories;
using ChannelService.Infrastructure.Persistence.Repositories;
using ChannelService.Infrastructure.Vault;

namespace ChannelService.Infrastructure.Persistence;

public static class PersistenceExtensions
{
    public static async Task<IServiceCollection> AddPersistenceAsync(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var databasePassword = await VaultSecretReader.GetDatabasePasswordAsync();

        var databaseConfiguration = new ConfigurationBuilder()
            .AddConfiguration(configuration)
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"{DatabaseOptions.SectionName}:Password"] = databasePassword
            })
            .Build();

        services
            .AddOptions<DatabaseOptions>()
            .Bind(databaseConfiguration.GetSection(DatabaseOptions.SectionName))
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
                
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorCodesToAdd: null);

                npgsqlOptions.CommandTimeout(10);
            });
        });

        services.AddScoped<IChannelDbContext>(
            provider => provider.GetRequiredService<ChannelDbContext>());

        services.AddScoped<IChannelRepository, ChannelRepository>();
        services.AddScoped<IMemberRepository, MemberRepository>();
        services.AddScoped<IUserBanSagaStateRepository, UserBanSagaStateRepository>();
        services.AddScoped<IUserBanSagaMembershipRepository, UserBanSagaMembershipRepository>();

        return services;
    }

    public static async Task ApplyMigrationsAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ChannelDbContext>();

        await dbContext.Database.MigrateAsync();
    }
}