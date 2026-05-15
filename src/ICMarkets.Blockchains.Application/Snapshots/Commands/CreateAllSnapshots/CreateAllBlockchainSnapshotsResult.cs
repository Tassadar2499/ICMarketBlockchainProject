using ICMarkets.Blockchains.Application.Common;

namespace ICMarkets.Blockchains.Application.Snapshots.Commands.CreateAllSnapshots;

public sealed record CreateAllBlockchainSnapshotsResult(
    IReadOnlyCollection<BlockchainSnapshotDto> Snapshots,
    IReadOnlyCollection<SnapshotFailureDto> Failures);
