---
name: plan-feature
description: Create concise implementation plans for ICMarkets Blockchains feature work before coding.
mode: subagent
---

# Plan Feature

Use this skill when the requested output is a plan, task breakdown, or implementation approach rather than code changes.

## Planning Workflow

1. Restate the feature goal in one or two sentences.
2. Inspect relevant existing code before planning. Prefer real files over assumptions.
3. Identify affected layers:
   - `ICMarkets.Blockchains.Domain` for invariants and entities.
   - `ICMarkets.Blockchains.Application` for commands, queries, DTOs, results, handlers, and abstractions.
   - `ICMarkets.Blockchains.Infrastructure` for persistence, HTTP clients, source strategies, and background jobs.
   - `ICMarkets.Blockchains.Api` for controllers, models, routing, Swagger, and configuration.
   - `tests/ICMarkets.Blockchains.UnitTests` and `tests/ICMarkets.Blockchains.IntegrationTests` for validation.
4. Call out assumptions only when they change implementation choices.
5. Keep the plan small enough that implementation can start immediately.

## Plan Shape

Use these sections when useful:

- Goal
- Current State
- Proposed Changes
- Files Likely To Change
- Test Strategy
- Validation Commands
- Risks Or Assumptions

## Repository Rules

- Preserve Clean Architecture dependencies.
- Do not introduce repositories or Unit of Work unless the project direction changes; the current design intentionally exposes EF Core through an application DbContext contract.
- Use xUnit integration tests with the local BlockCypher mock server for API behavior.
- Prefer `dotnet build ICMarkets.Blockchains.sln` and `dotnet test` as validation commands.
