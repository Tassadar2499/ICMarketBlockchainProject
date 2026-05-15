namespace ICMarkets.Blockchains.Api.Models;

public sealed record CreateAllSnapshotsResponse(
    IReadOnlyCollection<BlockchainSnapshotResponse> Snapshots,
    IReadOnlyCollection<SnapshotFailureResponse> Failures);
