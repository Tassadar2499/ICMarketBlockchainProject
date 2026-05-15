using ICMarkets.Blockchains.Application.Common;

namespace ICMarkets.Blockchains.Application.Abstractions.External;

public interface IBlockchainSourceStrategy
{
    Result<BlockchainSource> GetSource(string chain, string network);

    IReadOnlyCollection<BlockchainSource> GetAllSources();
}
