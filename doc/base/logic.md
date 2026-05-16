# Logic

## Supported Sources

The application supports a fixed BlockCypher source set:

| Chain | Network | Path |
| --- | --- | --- |
| `eth` | `main` | `/eth/main` |
| `dash` | `main` | `/dash/main` |
| `btc` | `main` | `/btc/main` |
| `btc` | `test3` | `/btc/test3` |
| `ltc` | `main` | `/ltc/main` |

Source lookup trims input and compares normalized lowercase `{chain}:{network}` keys. Unsupported sources fail before any HTTP call is made.

## Single Snapshot Flow

Endpoint:

```http
POST /api/blockchains/{chain}/{network}/snapshots
```

Flow:

1. `BlockchainsController` creates a `CreateBlockchainSnapshotCommand`.
2. `CreateBlockchainSnapshotHandler` resolves the requested source.
3. The handler captures `IDateTimeProvider.UtcNow` as the snapshot timestamp.
4. `IBlockCypherClient` fetches the current source JSON.
5. `BlockchainSnapshot.Create` validates required fields and normalizes chain/network values.
6. The snapshot is added to `IApplicationDbContext.BlockchainSnapshots`.
7. `SaveChangesAsync` persists the row.
8. The API returns `201 Created` with the raw JSON exposed as the `data` object.

If source lookup, HTTP fetch, JSON parsing, or persistence fails, the handler returns a failure result and the API responds with `400 Bad Request`.

## All Snapshots Flow

Endpoint:

```http
POST /api/blockchains/snapshots
```

Flow:

1. `CreateAllBlockchainSnapshotsHandler` gets all configured sources.
2. It fetches all sources concurrently with `Task.WhenAll`.
3. Successful fetches become `BlockchainSnapshot` entities.
4. Failed fetches become `SnapshotFailureDto` entries and do not block other sources.
5. Successful snapshots are saved in a single `SaveChangesAsync` call.
6. The API returns `200 OK` with `snapshots` and `failures` arrays.

Cancellation is propagated. Other per-source exceptions are converted into failure items.

## Snapshot History Flow

Endpoint:

```http
GET /api/blockchains/{chain}/{network}/snapshots?page=1&pageSize=20
```

Rules:

- `page` must be at least `1`.
- `pageSize` must be between `1` and `100`.
- `chain` and `network` must resolve to a supported source.

The query filters snapshots by normalized chain/network, orders by `CreatedAt` descending, counts total rows, applies `Skip` and `Take`, and returns a `PagedResponse`.

The response includes:

- `items`;
- `page`;
- `pageSize`;
- `totalCount`;
- `totalPages`.

## Background Snapshot Worker

`BlockchainSnapshotBackgroundService` runs when `SnapshotWorker:Enabled` is `true`.

Behavior:

1. Run one all-source snapshot cycle on startup.
2. Wait for `SnapshotWorker:Interval`.
3. Run scheduled all-source cycles until shutdown.

Each cycle resolves `CreateAllBlockchainSnapshotsHandler` from a new service scope. Stored snapshot and failure counts are logged. Per-source failures are logged as warnings; unexpected cycle failures are logged as errors.

Default interval: `00:05:00`.

## BlockCypher Client Rules

`BlockCypherClient` builds an absolute URI from the resolved source URL. If `BlockCypher:ApiToken` is set, it appends the token as a query parameter.

The client reads the response body as a string. Non-success HTTP status codes throw `HttpRequestException` with the status and body. Success responses must parse as JSON; the raw JSON string is returned unchanged for storage.

## Persistence Rules

Snapshots are append-only. There is no deduplication or update path. Each successful fetch creates a new row with a new `Guid` and timestamp.

The API stores raw upstream JSON and parses it only when shaping API responses. This preserves the original BlockCypher payload while still returning `data` as a JSON object.
