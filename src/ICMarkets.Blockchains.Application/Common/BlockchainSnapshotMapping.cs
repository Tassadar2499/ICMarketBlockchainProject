using ICMarkets.Blockchains.Domain.Snapshots;

namespace ICMarkets.Blockchains.Application.Common;

internal static class BlockchainSnapshotMapping
{
    public static BlockchainSnapshotDto ToDto(this BlockchainSnapshot snapshot) =>
        new(
            snapshot.Id,
            snapshot.Chain,
            snapshot.Network,
            snapshot.SourceUrl,
            snapshot.RawJson,
            snapshot.CreatedAt);
}
