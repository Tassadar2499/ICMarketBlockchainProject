using System.Text.Json;

namespace ICMarkets.Blockchains.Api.Models;

public sealed record BlockchainSnapshotResponse(
    Guid Id,
    string Chain,
    string Network,
    string SourceUrl,
    DateTimeOffset CreatedAt,
    JsonElement Data);
