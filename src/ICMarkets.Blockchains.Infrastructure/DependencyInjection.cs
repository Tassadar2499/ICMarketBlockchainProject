using ICMarkets.Blockchains.Application.Abstractions.Clock;
using ICMarkets.Blockchains.Application.Abstractions.Data;
using ICMarkets.Blockchains.Application.Abstractions.External;
using ICMarkets.Blockchains.Infrastructure.BackgroundJobs;
using ICMarkets.Blockchains.Infrastructure.BlockCypher;
using ICMarkets.Blockchains.Infrastructure.Clock;
using ICMarkets.Blockchains.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ICMarkets.Blockchains.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Blockchains")
            ?? configuration["Database:ConnectionString"]
            ?? "Data Source=./data/blockchains.db";

        SqliteFileSystem.EnsureDatabaseDirectoryExists(connectionString);

        services.Configure<BlockCypherOptions>(configuration.GetSection(BlockCypherOptions.SectionName));
        services.Configure<SnapshotWorkerOptions>(configuration.GetSection(SnapshotWorkerOptions.SectionName));

        services.AddDbContext<BlockchainDbContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<IApplicationDbContext>(serviceProvider => serviceProvider.GetRequiredService<BlockchainDbContext>());

        services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();
        services.AddSingleton<IBlockchainSourceStrategy, BlockCypherSourceStrategy>();

        services.AddHttpClient<IBlockCypherClient, BlockCypherClient>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(20);
        });

        services.AddHostedService<BlockchainSnapshotBackgroundService>();

        return services;
    }
}
