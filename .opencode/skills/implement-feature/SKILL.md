---
name: implement-feature
description: Implement ICMarkets Blockchains features end-to-end across API, Application, Domain, Infrastructure, and tests.
mode: subagent
---

# Implement Feature

Use this skill when adding or extending behavior in the ICMarkets Blockchains .NET solution.

## Workflow

1. Inspect the current shape of the feature in `src/` and matching tests in `tests/`.
2. Keep Clean Architecture boundaries intact:
   - Domain: entities and invariants in `src/ICMarkets.Blockchains.Domain`.
   - Application: commands, queries, DTOs, handlers, and abstractions in `src/ICMarkets.Blockchains.Application`.
   - Infrastructure: EF Core SQLite, BlockCypher clients, strategies, persistence, and background workers in `src/ICMarkets.Blockchains.Infrastructure`.
   - API: controllers, request/response models, Swagger metadata, configuration, and HTTP mapping in `src/ICMarkets.Blockchains.Api`.
3. Add or update tests before considering the work complete.
4. Run targeted tests first, then the full suite when the change affects shared behavior.

## Implementation Checklist

- Add or update domain behavior and validation when business rules change.
- Add Application commands or queries under feature folders such as `Snapshots/Commands/...` or `Snapshots/Queries/...`.
- Keep handlers explicit; this repository uses CQRS without MediatR.
- Add abstractions in `Application/Abstractions` before wiring infrastructure concerns.
- Update `DependencyInjection.cs` files when registering new services.
- Keep BlockCypher traffic behind infrastructure clients and strategies.
- Avoid real external API calls in tests.

## Verification Commands

```bash
dotnet build ICMarkets.Blockchains.sln
dotnet test
```

For focused checks:

```bash
dotnet test tests/ICMarkets.Blockchains.UnitTests
dotnet test tests/ICMarkets.Blockchains.IntegrationTests
```
