---
name: code-reviewer
description: Review ICMarkets Blockchains changes for correctness, regression risk, architecture fit, and missing tests.
mode: subagent
---

# Code Reviewer

Use this skill when reviewing committed or uncommitted changes in this repository.

## Review Scope

Start by identifying the diff scope:

```bash
git status --short
git diff --stat
git diff --name-only
```

If reviewing a branch against its base, prefer:

```bash
git merge-base HEAD master
git diff --stat master...HEAD
git diff --name-only master...HEAD
```

Use `main` instead of `master` if that is the repository default.

## Review Priorities

Prioritize findings in this order:

1. Correctness bugs and behavior regressions.
2. Broken Clean Architecture boundaries.
3. Persistence, SQLite, or EF Core query issues.
4. External BlockCypher client behavior, retry/error handling, or accidental live calls from tests.
5. API contract, status code, pagination, and validation problems.
6. Missing or weak xUnit coverage for changed behavior.

Avoid low-value style comments unless they obscure a real issue.

## Output Format

Lead with findings, ordered by severity. Include concrete file and line references whenever possible.

```text
Findings
1. [high|medium|low] Issue summary with file reference.

Open questions / assumptions
- Only include if needed.

Summary
- One short sentence if useful.
```

If no issues are found, say so clearly and mention any tests not run or residual risk.
