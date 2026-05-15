namespace ICMarkets.Blockchains.Api.Models;

public sealed record SnapshotFailureResponse(
    string Chain,
    string Network,
    string SourceUrl,
    string Error);
