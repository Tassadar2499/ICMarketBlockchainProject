namespace ICMarkets.Blockchains.Infrastructure.BlockCypher;

public sealed class BlockCypherOptions
{
    public const string SectionName = "BlockCypher";

    public string BaseUrl { get; init; } = "https://api.blockcypher.com/v1";

    public string? ApiToken { get; init; }
}
