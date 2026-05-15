using ICMarkets.Blockchains.Application.Abstractions.Messaging;
using ICMarkets.Blockchains.Application.Snapshots.Commands.CreateAllSnapshots;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ICMarkets.Blockchains.Infrastructure.BackgroundJobs;

public sealed class BlockchainSnapshotBackgroundService(
    IServiceScopeFactory scopeFactory,
    IOptions<SnapshotWorkerOptions> options,
    ILogger<BlockchainSnapshotBackgroundService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!options.Value.Enabled)
        {
            logger.LogInformation("Blockchain snapshot background worker is disabled.");
            return;
        }

        await RunCycleAsync("startup", stoppingToken);

        using var timer = new PeriodicTimer(options.Value.Interval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await RunCycleAsync("scheduled", stoppingToken);
        }
    }

    private async Task RunCycleAsync(string reason, CancellationToken cancellationToken)
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<
                ICommandHandler<CreateAllBlockchainSnapshotsCommand, CreateAllBlockchainSnapshotsResult>>();

            var result = await handler.HandleAsync(new CreateAllBlockchainSnapshotsCommand(), cancellationToken);

            logger.LogInformation(
                "Blockchain snapshot cycle completed ({Reason}). Stored {SnapshotCount} snapshots with {FailureCount} failures.",
                reason,
                result.Snapshots.Count,
                result.Failures.Count);

            foreach (var failure in result.Failures)
            {
                logger.LogWarning(
                    "Failed to store {Chain}/{Network} snapshot from {SourceUrl}: {Error}",
                    failure.Chain,
                    failure.Network,
                    failure.SourceUrl,
                    failure.Error);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Blockchain snapshot cycle failed ({Reason}).", reason);
        }
    }
}
