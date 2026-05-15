# Project Clarification Questions

1. Should the app be built from scratch in this folder, or is there an existing .NET solution you want me to extend?
A: Create new dotnet solution

2. Is `.NET 8` acceptable? The brief says `>= .NET 6`, so I would default to `.NET 8` unless `.NET 6` is required.
A: Use .net 10

3. For architecture, do you prefer `Clean Architecture` or `Vertical Slice Architecture`? I would lean `Clean Architecture` for this assignment because it visibly demonstrates SOLID, dependency injection, repositories/unit of work, mapping, and tests.
A: Clean Architecture


4. Which database should we use? I recommend `SQLite + EF Core` for simplicity, portability, and easy review, unless PostgreSQL or SQL Server in Docker is preferred.
A: SQLite + EF Core

5. Should API calls fetch live BlockCypher data only when an endpoint is called, or should the app also run a background scheduled job to periodically fetch and store snapshots?
A: run a background scheduled job to periodically fetch and store snapshots

6. Should each response be stored mostly as raw JSON, matching "stored as provided in the API's JSON responses," with metadata like `Id`, `Blockchain`, `Network`, and `CreatedAt`?
A: Yes

7. What API shape do you want? My default would be endpoints like:

   - `POST /api/blockchains/{chain}/snapshots` fetches live data and stores it
   - `GET /api/blockchains/{chain}/snapshots` returns history by `CreatedAt DESC`
   - `POST /api/blockchains/snapshots` fetches all configured chains in parallel
   - `GET /health`
   
A: Use this

8. Should history endpoints include pagination? I would add `page` and `pageSize` to avoid unbounded results.
A: yes, use pagination

9. Are unauthenticated BlockCypher requests acceptable, or should we support an optional API token via configuration?
A: Use optional API token

10. For design patterns, is `Repository + Unit of Work` enough, or should I also include lightweight `CQRS` using MediatR?
A: Do not use repository, do not use unit of work, use CQRS without MediatR

11. For tests, is `xUnit` acceptable? I would create Unit, Integration, and Functional test projects as requested.
A: Add integration tests, with local environment by docker compose
Add unit tests

12. Should the optional API Gateway be skipped? I would skip it unless specifically requested, because it adds complexity without much value for this assignment.
A: yse

13. Should I include Docker support with both `Dockerfile` and `docker-compose.yml`, even if the app uses SQLite?
A: yes

14. Do you want the final repo prepared with `main` and `development` branches, or only the application code and README?
A: only the application code and README

## Additional Clarification Questions

1. What should the background job interval be?

   Example: every `5 minutes`, `15 minutes`, or configurable with a default value.
   A: 5 minutes

2. Should the background job fetch all 5 BlockCypher endpoints on startup, or wait until the first scheduled interval?
A: fetch all

3. How should we model BTC mainnet and BTC testnet in routes?

   Recommended: use both `chain` and `network`, for example:

   - `POST /api/blockchains/btc/main/snapshots`
   - `POST /api/blockchains/btc/test3/snapshots`
   - `GET /api/blockchains/btc/main/snapshots`
   
   A: use both

4. Since Repository and Unit of Work should not be used, what second design pattern should we use besides CQRS?

   Recommended: `CQRS + Strategy`, where each blockchain endpoint/provider is resolved through a strategy/factory.
   A: CQRS + Strategy

5. The original brief asks for Integration, Functional, and Unit test projects. The current answer mentions integration and unit tests. Should I also create a separate Functional test project?
A: no Functional tests needed

6. For integration tests with Docker Compose, should tests call the real BlockCypher API, or should we use a mock HTTP server for stable repeatable tests?

   Recommended: mock HTTP server for tests, real API only for manual/local runtime.
A: mock HTTP server for tests, real API only for manual/local runtime.

7. Should API token configuration use an environment variable named `BlockCypher__ApiToken`?
A: yes

8. For API Gateway, answer says `yse`. I assume this means "yes, skip it." Is that correct?
A: yes
