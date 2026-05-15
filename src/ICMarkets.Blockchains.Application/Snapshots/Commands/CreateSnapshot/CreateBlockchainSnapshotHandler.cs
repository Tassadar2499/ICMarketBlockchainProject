using ICMarkets.Blockchains.Application.Abstractions.Clock;
using ICMarkets.Blockchains.Application.Abstractions.Data;
using ICMarkets.Blockchains.Application.Abstractions.External;
using ICMarkets.Blockchains.Application.Abstractions.Messaging;
using ICMarkets.Blockchains.Application.Common;
using ICMarkets.Blockchains.Domain.Snapshots;

namespace ICMarkets.Blockchains.Application.Snapshots.Commands.CreateSnapshot;

public sealed class CreateBlockchainSnapshotHandler(
    IBlockchainSourceStrategy sourceStrategy,
    IBlockCypherClient blockCypherClient,
    IDateTimeProvider dateTimeProvider,
    IApplicationDbContext dbContext)
    : ICommandHandler<CreateBlockchainSnapshotCommand, Result<BlockchainSnapshotDto>>
{
    public async Task<Result<BlockchainSnapshotDto>> HandleAsync(
        CreateBlockchainSnapshotCommand command,
        CancellationToken cancellationToken = default)
    {
        var sourceResult = sourceStrategy.GetSource(command.Chain, command.Network);
        if (!sourceResult.IsSuccess || sourceResult.Value is null)
        {
            return Result<BlockchainSnapshotDto>.Failure(sourceResult.Error ?? "Unsupported blockchain source.");
        }

        var source = sourceResult.Value;
        var requestedAt = dateTimeProvider.UtcNow;

        try
        {
            var rawJson = await blockCypherClient.FetchSnapshotJsonAsync(source, cancellationToken);
            var snapshot = BlockchainSnapshot.Create(source.Chain, source.Network, source.Url, rawJson, requestedAt);

            dbContext.BlockchainSnapshots.Add(snapshot);
            await dbContext.SaveChangesAsync(cancellationToken);

            return Result<BlockchainSnapshotDto>.Success(snapshot.ToDto());
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            return Result<BlockchainSnapshotDto>.Failure(
                $"Failed to fetch {source.Chain}/{source.Network} snapshot: {exception.Message}");
        }
    }
}
