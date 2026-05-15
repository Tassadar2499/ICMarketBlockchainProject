namespace ICMarkets.Blockchains.Application.Common;

public sealed record SnapshotFailureDto(
    string Chain,
    string Network,
    string SourceUrl,
    string Error);
