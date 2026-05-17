using ICMarkets.Blockchains.Application.Abstractions.Data;
using ICMarkets.Blockchains.Infrastructure.BackgroundJobs;
using ICMarkets.Blockchains.Infrastructure.BlockCypher;
using ICMarkets.Blockchains.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace ICMarkets.Blockchains.IntegrationTests.Support;

public sealed class ApiTestFactory(BlockCypherMockServer mockServer) : WebApplicationFactory<Program>
{
    private readonly string _databaseDirectory = Path.Combine(Path.GetTempPath(), $"icmarkets-{Guid.NewGuid():N}");
    private string DatabaseConnectionString => $"Data Source={Path.Combine(_databaseDirectory, "blockchains.db")}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Directory.CreateDirectory(_databaseDirectory);

        builder.UseEnvironment("IntegrationTests");

        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:Blockchains"] = DatabaseConnectionString,
                ["BlockCypher:BaseUrl"] = $"{mockServer.BaseAddress}/v1",
                ["SnapshotWorker:Enabled"] = "false"
            });
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<BlockchainDbContext>>();
            services.RemoveAll<BlockchainDbContext>();
            services.RemoveAll<IApplicationDbContext>();

            services.AddDbContext<BlockchainDbContext>(options => options.UseSqlite(DatabaseConnectionString));
            services.AddScoped<IApplicationDbContext>(serviceProvider =>
                serviceProvider.GetRequiredService<BlockchainDbContext>());

            services.RemoveAll<IOptions<BlockCypherOptions>>();
            services.AddSingleton<IOptions<BlockCypherOptions>>(_ =>
                Options.Create(new BlockCypherOptions { BaseUrl = $"{mockServer.BaseAddress}/v1" }));

            services.RemoveAll<IOptions<SnapshotWorkerOptions>>();
            services.AddSingleton<IOptions<SnapshotWorkerOptions>>(_ =>
                Options.Create(new SnapshotWorkerOptions { Enabled = false }));
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (Directory.Exists(_databaseDirectory))
        {
            Directory.Delete(_databaseDirectory, recursive: true);
        }
    }
}
