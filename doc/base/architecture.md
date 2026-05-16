# Architecture

## Overview

ICMarkets Blockchains is a .NET 10 Web API that fetches blockchain snapshots from BlockCypher, stores the raw JSON payloads in SQLite, and exposes paginated snapshot history over HTTP.

The solution follows Clean Architecture. The API owns transport concerns, Application owns use cases, Domain owns snapshot invariants, and Infrastructure owns persistence, external HTTP calls, clocks, and hosted background work.

## Project Layout

| Project | Responsibility |
| --- | --- |
| `src/ICMarkets.Blockchains.Domain` | Domain entities and validation rules. |
| `src/ICMarkets.Blockchains.Application` | CQRS commands, queries, handlers, DTOs, results, and abstractions. |
| `src/ICMarkets.Blockchains.Infrastructure` | EF Core SQLite, BlockCypher HTTP client, source strategy, system clock, and background worker. |
| `src/ICMarkets.Blockchains.Api` | Controllers, route model binding, response models, Swagger, CORS, health checks, and startup wiring. |
| `tests/ICMarkets.Blockchains.UnitTests` | Fast tests for domain behavior, source strategy, and isolated client logic. |
| `tests/ICMarkets.Blockchains.IntegrationTests` | In-memory API tests using SQLite and a mocked BlockCypher HTTP server. |

## Dependency Direction

Dependencies flow inward:

```text
Api -> Application <- Domain
Api -> Infrastructure -> Application
Infrastructure -> Domain
```

Application defines interfaces such as `IApplicationDbContext`, `IBlockCypherClient`, `IBlockchainSourceStrategy`, and `IDateTimeProvider`. Infrastructure implements those interfaces and registers them through dependency injection.

The repository intentionally does not use Repository or Unit of Work patterns. EF Core is exposed to Application through `IApplicationDbContext`.

## Runtime Composition

`src/ICMarkets.Blockchains.Api/Program.cs` is the composition root:

- registers Application handlers with `AddApplication()`;
- registers Infrastructure services with `AddInfrastructure(configuration)`;
- enables controllers, camelCase JSON, Swagger, CORS, and health checks;
- creates the SQLite database on startup with `Database.EnsureCreatedAsync()`;
- maps controllers and `/health`.

Infrastructure configures:

- `BlockchainDbContext` with SQLite;
- `IApplicationDbContext` as a scoped alias for the DbContext;
- `BlockCypherSourceStrategy` as a singleton;
- `BlockCypherClient` through `HttpClientFactory`;
- `BlockchainSnapshotBackgroundService` as a hosted service.

## Data Model

The only persisted aggregate is `BlockchainSnapshot`.

Fields:

- `Id`: generated `Guid`.
- `Chain`: normalized lowercase chain code.
- `Network`: normalized lowercase network code.
- `SourceUrl`: BlockCypher source URL used for the fetch.
- `RawJson`: raw BlockCypher JSON response.
- `CreatedAt`: UTC timestamp stored as Unix milliseconds.

SQLite table: `BlockchainSnapshots`.

Index: `(Chain, Network, CreatedAt)` supports filtered history reads ordered by newest snapshot first.

## External Integration

BlockCypher is accessed only through `IBlockCypherClient`. Supported sources are resolved by `IBlockchainSourceStrategy`, which builds source URLs from `BlockCypher:BaseUrl`.

The HTTP client:

- sends `Accept: application/json`;
- appends `token=<ApiToken>` when configured;
- validates the response status code;
- parses the body as JSON before returning the raw payload.

## Testing Architecture

Unit tests cover isolated behavior and avoid real network or database dependencies. Integration tests run the API in memory, use SQLite, and redirect BlockCypher calls to `BlockCypherMockServer`.

All test traffic to BlockCypher must remain mocked.
