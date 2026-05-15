namespace ICMarkets.Blockchains.Application.Snapshots.Queries.GetSnapshotHistory;

public sealed record GetBlockchainSnapshotHistoryQuery(
    string Chain,
    string Network,
    int Page,
    int PageSize);
