# ICMarkets Blockchains API

.NET 10 Web API for fetching BlockCypher blockchain data, storing snapshots in SQLite, and exposing paginated blockchain history.

## Architecture

The solution uses Clean Architecture:

| Project | Purpose |
|---|---|
| `ICMarkets.Blockchains.Domain` | Snapshot entity and domain validation |
| `ICMarkets.Blockchains.Application` | CQRS commands, queries, handlers, DTOs, contracts |
| `ICMarkets.Blockchains.Infrastructure` | EF Core SQLite, BlockCypher HTTP client, source strategy, background worker |
| `ICMarkets.Blockchains.Api` | Controllers, Swagger, CORS, health checks, JSON serialization |
| `ICMarkets.Blockchains.UnitTests` | Domain, strategy, and client unit tests |
| `ICMarkets.Blockchains.IntegrationTests` | API tests with SQLite and mocked BlockCypher HTTP server |

Design patterns used:

- CQRS without MediatR: explicit command/query handlers are registered in DI.
- Strategy: supported BlockCypher sources are resolved by `IBlockchainSourceStrategy`.

Repository and Unit of Work patterns are intentionally not used. EF Core `DbContext` is exposed through an application DbContext contract.

## Supported Sources

| Chain | Network | Source |
|---|---|---|
| `eth` | `main` | `https://api.blockcypher.com/v1/eth/main` |
| `dash` | `main` | `https://api.blockcypher.com/v1/dash/main` |
| `btc` | `main` | `https://api.blockcypher.com/v1/btc/main` |
| `btc` | `test3` | `https://api.blockcypher.com/v1/btc/test3` |
| `ltc` | `main` | `https://api.blockcypher.com/v1/ltc/main` |

## API Endpoints

```http
POST /api/blockchains/{chain}/{network}/snapshots
```

Fetches one live BlockCypher snapshot and stores the raw JSON response with `CreatedAt`.

```http
POST /api/blockchains/snapshots
```

Fetches all configured BlockCypher sources in parallel and stores all successful snapshots.

```http
GET /api/blockchains/{chain}/{network}/snapshots?page=1&pageSize=20
```

Returns paginated history ordered by `CreatedAt` descending.

```http
GET /health
```

Health check endpoint.

Swagger UI is available at `/swagger`.

## Configuration

Main settings are in `src/ICMarkets.Blockchains.Api/appsettings.json`.

| Setting | Default | Description |
|---|---|---|
| `ConnectionStrings__Blockchains` | `Data Source=./data/blockchains.db` | SQLite database path |
| `BlockCypher__BaseUrl` | `https://api.blockcypher.com/v1` | BlockCypher API base URL |
| `BlockCypher__ApiToken` | empty | Optional BlockCypher token |
| `SnapshotWorker__Enabled` | `true` | Enables the background worker |
| `SnapshotWorker__Interval` | `00:05:00` | Snapshot worker interval |

The background worker fetches all five sources on startup and then every five minutes.

## Run Locally

Prerequisite: .NET 10 SDK.

```bash
dotnet restore
dotnet run --project src/ICMarkets.Blockchains.Api
```

Open:

```text
http://localhost:5080/swagger
```

Optional API token:

```bash
BlockCypher__ApiToken=your-token dotnet run --project src/ICMarkets.Blockchains.Api
```

## Run With Docker

Prerequisite: Docker or Docker Compose.

```bash
docker compose up --build
```

Open:

```text
http://localhost:8080/swagger
```

Optional API token:

```bash
BLOCKCYPHER_API_TOKEN=your-token docker compose up --build
```

SQLite data is persisted in the `blockchains-data` Docker volume.

## Tests

```bash
dotnet test
```

Integration tests run the API in memory, use SQLite, and point BlockCypher traffic to a local mocked HTTP server. They do not call the real BlockCypher API.

Tests can also be run through Docker Compose:

```bash
docker compose --profile tests run --rm tests
```

## Notes

- Raw BlockCypher JSON is stored as provided in `BlockchainSnapshots.RawJson`.
- API responses expose the stored raw JSON as the `data` object.
- API Gateway is omitted based on clarified requirements.
- Functional tests are omitted based on clarified requirements.
