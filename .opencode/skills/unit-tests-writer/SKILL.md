---
name: unit-tests-writer
description: Write focused xUnit unit tests for ICMarkets Blockchains domain, application, strategy, and client behavior.
mode: subagent
---

# Unit Tests Writer

Use this skill when adding or updating fast, isolated tests in `tests/ICMarkets.Blockchains.UnitTests`.

## Scope

Prefer unit tests for:

- Domain invariants in `ICMarkets.Blockchains.Domain`.
- Application handlers, mapping, result objects, and validation logic that can be tested with fakes.
- Infrastructure components with isolated dependencies, such as source strategy behavior or HTTP client response handling.

Do not use unit tests for full API routing, dependency injection wiring, EF Core persistence behavior, or end-to-end HTTP flows. Put those in integration tests.

## Workflow

1. Inspect the production code and any neighboring tests first.
2. Add tests in the matching unit test file, or create a focused `{Subject}Tests.cs`.
3. Use descriptive xUnit names with underscore-separated behavior, for example `Create_Throws_When_RawJson_Is_Empty`.
4. Keep arrangements small and explicit. Avoid shared setup unless it removes real duplication.
5. Assert meaningful behavior, not implementation details.

## Repository Patterns

- Test project: `tests/ICMarkets.Blockchains.UnitTests`.
- Framework: xUnit.
- Use `Fact` for fixed scenarios and `Theory` for input variations.
- Keep tests deterministic; avoid real time, network calls, and filesystem dependencies unless wrapped by simple fakes.
- Reuse application/domain abstractions where available rather than mocking concrete infrastructure.

## Verification

```bash
dotnet test tests/ICMarkets.Blockchains.UnitTests
dotnet test
```
