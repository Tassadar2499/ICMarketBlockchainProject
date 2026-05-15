namespace ICMarkets.Blockchains.Application.Common;

public sealed record BlockchainSource(string Chain, string Network, string Url)
{
    public string Key => CreateKey(Chain, Network);

    public static string CreateKey(string chain, string network) =>
        $"{chain.Trim().ToLowerInvariant()}:{network.Trim().ToLowerInvariant()}";
}
