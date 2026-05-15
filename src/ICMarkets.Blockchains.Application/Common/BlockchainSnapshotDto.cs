namespace ICMarkets.Blockchains.Application.Common;

public sealed record BlockchainSnapshotDto(
    Guid Id,
    string Chain,
    string Network,
    string SourceUrl,
    string RawJson,
    DateTimeOffset CreatedAt);
