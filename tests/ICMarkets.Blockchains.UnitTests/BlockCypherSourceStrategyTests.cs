using ICMarkets.Blockchains.Infrastructure.BlockCypher;
using Microsoft.Extensions.Options;

namespace ICMarkets.Blockchains.UnitTests;

public sealed class BlockCypherSourceStrategyTests
{
    [Fact]
    public void GetAllSources_Returns_Five_Configured_Sources()
    {
        var strategy = new BlockCypherSourceStrategy(Options.Create(new BlockCypherOptions()));

        var sources = strategy.GetAllSources();

        Assert.Equal(5, sources.Count);
        Assert.Contains(sources, source => source.Chain == "btc" && source.Network == "test3");
    }

    [Fact]
    public void GetSource_Resolves_CaseInsensitive_Source()
    {
        var strategy = new BlockCypherSourceStrategy(Options.Create(new BlockCypherOptions()));

        var result = strategy.GetSource("BTC", "MAIN");

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("https://api.blockcypher.com/v1/btc/main", result.Value!.Url);
    }

    [Fact]
    public void GetSource_Returns_Failure_For_Unsupported_Source()
    {
        var strategy = new BlockCypherSourceStrategy(Options.Create(new BlockCypherOptions()));

        var result = strategy.GetSource("doge", "main");

        Assert.False(result.IsSuccess);
        Assert.Contains("Unsupported", result.Error);
    }
}
