# AGENTS.md

Instructions for coding agents working in this repository.

## Project

This is a .NET 8 plugin for `TSQLLint` that provides general-purpose SQL linting rules.

- Plugin entry point: `TSQLLintGeneralRulesPlugin/SqlPlugin.cs`
- Rules: `TSQLLintGeneralRulesPlugin/Rules/*.cs`
- Tests: `TSQLLintGeneralRulesPlugin.Tests/*RuleTests.cs` (xUnit, uses `TestSqlLintRunner`)
- Rule docs: `docs/rules/*.md` (filename matches rule ID)
- Rule catalog: `docs/rules/README.md` (keep in sync when adding rules)

For deeper architecture notes, see `CLAUDE.md`.

## Common commands

```powershell
dotnet build TSQLLintGeneralRulesPlugin.sln -c Release
dotnet test  TSQLLintGeneralRulesPlugin.sln -c Release
dotnet test  TSQLLintGeneralRulesPlugin.sln -c Release --filter "SomeRuleTests"
dotnet pack  TSQLLintGeneralRulesPlugin/TSQLLintGeneralRulesPlugin.csproj -c Release -o artifacts/nuget
```

Generated output directories like `bin/`, `obj/`, `dist/`, `artifacts/`, and `TestResults/` should not be committed.

## Rule implementation conventions

- Rules are `sealed class ... : TSqlFragmentVisitor, ISqlLintRule`.
- Rule IDs are kebab-case (`RULE_NAME`) and must match the key used in `SqlPlugin.GetRules()`.
- `ISqlLintRule` only requires `RULE_NAME`, `RULE_SEVERITY`, and `FixViolation`; this repo also keeps a `RULE_TEXT` message property for consistency (TSQLLint reads this conventionally).
- Violations are reported via the error callback using 1-based line/column numbers from ScriptDom nodes.
- Plugin registration uses `null!` placeholders; implement rules so the callback can be null at construction time (store it as nullable and invoke via `?.Invoke(...)`).
- Most rules intentionally do not auto-fix; keep `FixViolation` as a no-op unless explicitly requested.
- Prefer AST-first checks; when token-level detection is needed, use `node.ScriptTokenStream` plus `FirstTokenIndex`/`LastTokenIndex` and guard for null/empty streams.
- Project uses `<Nullable>enable</Nullable>`; keep nullability explicit.
- Follow `.editorconfig` (4 spaces, LF, trim trailing whitespace, final newline).

## Adding a new rule (expected workflow)

1. Add rule class under `TSQLLintGeneralRulesPlugin/Rules/`.
2. Register it in `TSQLLintGeneralRulesPlugin/SqlPlugin.cs` (`GetRules()`).
3. Add tests under `TSQLLintGeneralRulesPlugin.Tests/` covering both violation and non-violation cases.
4. Add documentation under `docs/rules/` (markdown file named after `RULE_NAME`) and add it to `docs/rules/README.md`.

## Agent working guidelines

- Prefer `rg`/`rg --files` for searching.
- Prefer `apply_patch` for focused edits; avoid reformatting unrelated code.
- Avoid destructive `git` commands (e.g., `reset --hard`, mass file deletes) unless explicitly requested.
