using ICMarkets.Blockchains.Application.Common;

namespace ICMarkets.Blockchains.Application.Abstractions.External;

public interface IBlockCypherClient
{
    Task<string> FetchSnapshotJsonAsync(BlockchainSource source, CancellationToken cancellationToken = default);
}
