# AGENTS.md

Instructions for coding agents working in this repository.

## Project

This is a .NET 8 plugin for `TSQLLint` that provides general-purpose SQL linting rules.

- Plugin entry point: `TSQLLintGeneralRulesPlugin/SqlPlugin.cs`
- Rules: `TSQLLintGeneralRulesPlugin/Rules/*.cs`
- Tests: `TSQLLintGeneralRulesPlugin.Tests/*RuleTests.cs` (xUnit, uses `TestSqlLintRunner`)
- Rule docs: `docs/rules/*.md` (filename matches rule ID)

For deeper architecture notes, see `CLAUDE.md`.

## Common commands

```powershell
dotnet build TSQLLintGeneralRulesPlugin.sln -c Release
dotnet test  TSQLLintGeneralRulesPlugin.sln -c Release
dotnet pack  TSQLLintGeneralRulesPlugin/TSQLLintGeneralRulesPlugin.csproj -c Release -o artifacts/nuget
```

Generated output directories like `bin/`, `obj/`, `dist/`, and `artifacts/` should not be committed.

## Rule implementation conventions

- Rules are `sealed class ... : TSqlFragmentVisitor, ISqlLintRule`.
- Rule IDs are kebab-case (`RULE_NAME`) and must match the key used in `SqlPlugin.GetRules()`.
- Violations are reported via the error callback using 1-based line/column numbers from ScriptDom nodes.
- Most rules intentionally do not auto-fix; keep `FixViolation` as a no-op unless explicitly requested.
- Project uses `<Nullable>enable</Nullable>`; keep nullability explicit.
- Follow `.editorconfig` (4 spaces, LF, trim trailing whitespace, final newline).

## Adding a new rule (expected workflow)

1. Add rule class under `TSQLLintGeneralRulesPlugin/Rules/`.
2. Register it in `TSQLLintGeneralRulesPlugin/SqlPlugin.cs` (`GetRules()`).
3. Add tests under `TSQLLintGeneralRulesPlugin.Tests/` covering both violation and non-violation cases.
4. Add documentation under `docs/rules/` (markdown file named after `RULE_NAME`).

## Agent working guidelines

- Prefer `rg`/`rg --files` for searching.
- Prefer `apply_patch` for focused edits; avoid reformatting unrelated code.
- Avoid destructive `git` commands (e.g., `reset --hard`, mass file deletes) unless explicitly requested.
