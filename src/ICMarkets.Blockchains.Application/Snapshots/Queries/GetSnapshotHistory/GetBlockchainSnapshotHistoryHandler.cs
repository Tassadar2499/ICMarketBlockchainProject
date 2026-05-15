using ICMarkets.Blockchains.Application.Abstractions.Data;
using ICMarkets.Blockchains.Application.Abstractions.External;
using ICMarkets.Blockchains.Application.Abstractions.Messaging;
using ICMarkets.Blockchains.Application.Common;
using Microsoft.EntityFrameworkCore;

namespace ICMarkets.Blockchains.Application.Snapshots.Queries.GetSnapshotHistory;

public sealed class GetBlockchainSnapshotHistoryHandler(
    IBlockchainSourceStrategy sourceStrategy,
    IApplicationDbContext dbContext)
    : IQueryHandler<GetBlockchainSnapshotHistoryQuery, Result<PagedResult<BlockchainSnapshotDto>>>
{
    private const int MaxPageSize = 100;

    public async Task<Result<PagedResult<BlockchainSnapshotDto>>> HandleAsync(
        GetBlockchainSnapshotHistoryQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query.Page < 1)
        {
            return Result<PagedResult<BlockchainSnapshotDto>>.Failure("Page must be greater than or equal to 1.");
        }

        if (query.PageSize < 1 || query.PageSize > MaxPageSize)
        {
            return Result<PagedResult<BlockchainSnapshotDto>>.Failure(
                $"Page size must be between 1 and {MaxPageSize}.");
        }

        var sourceResult = sourceStrategy.GetSource(query.Chain, query.Network);
        if (!sourceResult.IsSuccess || sourceResult.Value is null)
        {
            return Result<PagedResult<BlockchainSnapshotDto>>.Failure(sourceResult.Error ?? "Unsupported blockchain source.");
        }

        var source = sourceResult.Value;
        var snapshotsQuery = dbContext.BlockchainSnapshots
            .AsNoTracking()
            .Where(snapshot => snapshot.Chain == source.Chain && snapshot.Network == source.Network)
            .OrderByDescending(snapshot => snapshot.CreatedAt);

        var totalCount = await snapshotsQuery.CountAsync(cancellationToken);
        var snapshots = await snapshotsQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(snapshot => new BlockchainSnapshotDto(
                snapshot.Id,
                snapshot.Chain,
                snapshot.Network,
                snapshot.SourceUrl,
                snapshot.RawJson,
                snapshot.CreatedAt))
            .ToListAsync(cancellationToken);

        return Result<PagedResult<BlockchainSnapshotDto>>.Success(
            new PagedResult<BlockchainSnapshotDto>(snapshots, query.Page, query.PageSize, totalCount));
    }
}
