namespace ICMarkets.Blockchains.Domain.Snapshots;

public sealed class BlockchainSnapshot
{
    private BlockchainSnapshot()
    {
    }

    private BlockchainSnapshot(
        Guid id,
        string chain,
        string network,
        string sourceUrl,
        string rawJson,
        DateTimeOffset createdAt)
    {
        Id = id;
        Chain = chain;
        Network = network;
        SourceUrl = sourceUrl;
        RawJson = rawJson;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public string Chain { get; private set; } = string.Empty;

    public string Network { get; private set; } = string.Empty;

    public string SourceUrl { get; private set; } = string.Empty;

    public string RawJson { get; private set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; private set; }

    public static BlockchainSnapshot Create(
        string chain,
        string network,
        string sourceUrl,
        string rawJson,
        DateTimeOffset createdAt)
    {
        if (string.IsNullOrWhiteSpace(chain))
        {
            throw new ArgumentException("Chain is required.", nameof(chain));
        }

        if (string.IsNullOrWhiteSpace(network))
        {
            throw new ArgumentException("Network is required.", nameof(network));
        }

        if (string.IsNullOrWhiteSpace(sourceUrl))
        {
            throw new ArgumentException("Source URL is required.", nameof(sourceUrl));
        }

        if (string.IsNullOrWhiteSpace(rawJson))
        {
            throw new ArgumentException("Raw JSON is required.", nameof(rawJson));
        }

        return new BlockchainSnapshot(
            Guid.NewGuid(),
            chain.Trim().ToLowerInvariant(),
            network.Trim().ToLowerInvariant(),
            sourceUrl.Trim(),
            rawJson,
            createdAt);
    }
}
