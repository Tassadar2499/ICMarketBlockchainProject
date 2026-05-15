using ICMarkets.Blockchains.Application.Abstractions.External;
using ICMarkets.Blockchains.Application.Common;
using Microsoft.Extensions.Options;

namespace ICMarkets.Blockchains.Infrastructure.BlockCypher;

public sealed class BlockCypherSourceStrategy : IBlockchainSourceStrategy
{
    private static readonly (string Chain, string Network)[] SupportedSources =
    [
        ("eth", "main"),
        ("dash", "main"),
        ("btc", "main"),
        ("btc", "test3"),
        ("ltc", "main")
    ];

    private readonly IReadOnlyDictionary<string, BlockchainSource> _sources;

    public BlockCypherSourceStrategy(IOptions<BlockCypherOptions> options)
    {
        var baseUrl = string.IsNullOrWhiteSpace(options.Value.BaseUrl)
            ? "https://api.blockcypher.com/v1"
            : options.Value.BaseUrl.TrimEnd('/');

        _sources = SupportedSources
            .Select(source => new BlockchainSource(source.Chain, source.Network, $"{baseUrl}/{source.Chain}/{source.Network}"))
            .ToDictionary(source => source.Key, StringComparer.OrdinalIgnoreCase);
    }

    public Result<BlockchainSource> GetSource(string chain, string network)
    {
        if (string.IsNullOrWhiteSpace(chain) || string.IsNullOrWhiteSpace(network))
        {
            return Result<BlockchainSource>.Failure("Both chain and network are required.");
        }

        var key = BlockchainSource.CreateKey(chain, network);
        return _sources.TryGetValue(key, out var source)
            ? Result<BlockchainSource>.Success(source)
            : Result<BlockchainSource>.Failure($"Unsupported blockchain source '{chain}/{network}'.");
    }

    public IReadOnlyCollection<BlockchainSource> GetAllSources() => _sources.Values.ToArray();
}
