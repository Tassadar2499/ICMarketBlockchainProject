---
name: integration-tests-writer
description: Write API and persistence integration tests for ICMarkets Blockchains using xUnit, WebApplicationFactory, SQLite, and the BlockCypher mock server.
mode: subagent
---

# Integration Tests Writer

Use this skill when testing API behavior, dependency wiring, persistence, HTTP status codes, pagination, or interactions across multiple layers.

## Scope

Write integration tests in `tests/ICMarkets.Blockchains.IntegrationTests` for:

- API endpoints in `src/ICMarkets.Blockchains.Api`.
- Request/response contracts, status codes, and validation behavior.
- SQLite-backed snapshot persistence and ordering.
- BlockCypher client flows through the local mock server.
- Background or service wiring when it affects observable behavior.

Do not call the real BlockCypher API from tests.

## Workflow

1. Inspect `BlockchainEndpointsTests.cs`, `Support/ApiTestFactory.cs`, and `Support/BlockCypherMockServer.cs`.
2. Create the mock server with `BlockCypherMockServer.CreateWithDefaultResponses()` unless the scenario needs custom responses.
3. Create the API client from `ApiTestFactory`.
4. Exercise real HTTP endpoints with `HttpClient`.
5. Assert status code first, then response body and persisted behavior.
6. Add pagination, error, and unsupported-source cases when the changed behavior depends on them.

## Repository Patterns

- Test project: `tests/ICMarkets.Blockchains.IntegrationTests`.
- Framework: xUnit with `Microsoft.AspNetCore.Mvc.Testing`.
- Parse JSON responses with `JsonDocument` when asserting raw response shape.
- Keep tests isolated; each test should create its own mock server, factory, and client.
- Use mocked BlockCypher paths like `/v1/btc/main` for custom upstream responses.

## Verification

```bash
dotnet test tests/ICMarkets.Blockchains.IntegrationTests
dotnet test
```
