# Repository Guidelines

## Project Structure & Module Organization

This is a .NET 10 Web API solution organized around Clean Architecture.

- `src/ICMarkets.Blockchains.Domain`: snapshot entity and domain validation.
- `src/ICMarkets.Blockchains.Application`: CQRS commands, queries, handlers, DTOs, and contracts.
- `src/ICMarkets.Blockchains.Infrastructure`: EF Core SQLite persistence, BlockCypher client, source strategy, and background worker.
- `src/ICMarkets.Blockchains.Api`: controllers, Swagger, health checks, configuration, and HTTP response models.
- `tests/ICMarkets.Blockchains.UnitTests`: domain, strategy, and client unit tests.
- `tests/ICMarkets.Blockchains.IntegrationTests`: in-memory API tests using SQLite and a mocked BlockCypher server.
- `doc/base`: reference docs for [architecture](doc/base/architecture.md) and [runtime logic](doc/base/logic.md).

Keep dependencies flowing inward: API and Infrastructure can reference Application; Application should depend on abstractions and Domain behavior, not infrastructure details.

## Build, Test, and Development Commands

- `dotnet restore`: restore NuGet packages for `ICMarkets.Blockchains.sln`.
- `dotnet build ICMarkets.Blockchains.sln`: compile all projects.
- `dotnet test`: run unit and integration tests.
- `dotnet run --project src/ICMarkets.Blockchains.Api`: run the API locally; Swagger is available at `http://localhost:5080/swagger`.
- `docker compose up --build`: build and run the API container on `http://localhost:8080/swagger`.
- `docker compose --profile tests run --rm tests`: run the test suite inside the .NET SDK container.

## Coding Style & Naming Conventions

Use C# defaults with 4-space indentation, nullable reference types, and implicit usings enabled. Prefer `sealed` classes where inheritance is not intended. Use PascalCase for public types and members, camelCase for locals and parameters, and `Async` suffixes for asynchronous methods. Keep commands, queries, and handlers in feature folders such as `Snapshots/Commands/CreateSnapshot`.

## Testing Guidelines

Tests use xUnit with `Microsoft.NET.Test.Sdk` and `coverlet.collector` available. Name tests with descriptive underscore-separated behavior, for example `Create_Throws_When_RawJson_Is_Empty`. Put fast domain and strategy coverage in unit tests; put API behavior, persistence, and external HTTP mocking in integration tests. Integration tests must use `BlockCypherMockServer`, not the real BlockCypher API.

## Commit & Pull Request Guidelines

Git history currently only shows `Initial implementation`, so no strict convention is established. Use short imperative commit subjects, for example `Add snapshot pagination validation`. Pull requests should include a focused summary, test commands run, linked issues when applicable, and API examples or screenshots for visible endpoint or Swagger changes.

## Security & Configuration Tips

Do not commit API tokens or local database files. Configure secrets through environment variables such as `BlockCypher__ApiToken` locally or `BLOCKCYPHER_API_TOKEN` for Docker Compose. SQLite data lives under `./data` locally and in the `blockchains-data` Docker volume.
