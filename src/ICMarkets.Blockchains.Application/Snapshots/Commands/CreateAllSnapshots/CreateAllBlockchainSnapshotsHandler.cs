using ICMarkets.Blockchains.Application.Abstractions.Clock;
using ICMarkets.Blockchains.Application.Abstractions.Data;
using ICMarkets.Blockchains.Application.Abstractions.External;
using ICMarkets.Blockchains.Application.Abstractions.Messaging;
using ICMarkets.Blockchains.Application.Common;
using ICMarkets.Blockchains.Domain.Snapshots;

namespace ICMarkets.Blockchains.Application.Snapshots.Commands.CreateAllSnapshots;

public sealed class CreateAllBlockchainSnapshotsHandler(
    IBlockchainSourceStrategy sourceStrategy,
    IBlockCypherClient blockCypherClient,
    IDateTimeProvider dateTimeProvider,
    IApplicationDbContext dbContext)
    : ICommandHandler<CreateAllBlockchainSnapshotsCommand, CreateAllBlockchainSnapshotsResult>
{
    public async Task<CreateAllBlockchainSnapshotsResult> HandleAsync(
        CreateAllBlockchainSnapshotsCommand command,
        CancellationToken cancellationToken = default)
    {
        var sources = sourceStrategy.GetAllSources();
        var fetchTasks = sources.Select(source => FetchSourceAsync(source, cancellationToken));
        var results = await Task.WhenAll(fetchTasks);

        var snapshots = new List<BlockchainSnapshotDto>();
        var failures = new List<SnapshotFailureDto>();

        foreach (var result in results)
        {
            if (result.Snapshot is not null)
            {
                dbContext.BlockchainSnapshots.Add(result.Snapshot);
                snapshots.Add(result.Snapshot.ToDto());
                continue;
            }

            if (result.Failure is not null)
            {
                failures.Add(result.Failure);
            }
        }

        if (snapshots.Count > 0)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        return new CreateAllBlockchainSnapshotsResult(snapshots, failures);
    }

    private async Task<FetchResult> FetchSourceAsync(BlockchainSource source, CancellationToken cancellationToken)
    {
        var requestedAt = dateTimeProvider.UtcNow;

        try
        {
            var rawJson = await blockCypherClient.FetchSnapshotJsonAsync(source, cancellationToken);
            var snapshot = BlockchainSnapshot.Create(source.Chain, source.Network, source.Url, rawJson, requestedAt);
            return new FetchResult(snapshot, null);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            return new FetchResult(
                null,
                new SnapshotFailureDto(source.Chain, source.Network, source.Url, exception.Message));
        }
    }

    private sealed record FetchResult(BlockchainSnapshot? Snapshot, SnapshotFailureDto? Failure);
}
