# TSQLLintGeneralRulesPlugin

A .NET plugin that provides general-purpose rules for `TSQLLint`.

This plugin exposes `TSQLLint.Plugin.SqlPlugin` which implements `IPlugin` from `TSQLLint.Common` and registers rules returned by `SqlPlugin.GetRules()` with TSQLLint.

## Requirements

- .NET SDK 8.0+
- TSQLLint (runtime environment compatible with `TSQLLint.Common`)

## Build / Test

- Build: `dotnet build TSQLLintGeneralRulesPlugin.sln -c Release`
- Test: `dotnet test TSQLLintGeneralRulesPlugin.sln -c Release`

## Pack (NuGet)

The `artifacts/` directory is generated and should not be committed.

- Pack: `dotnet pack TSQLLintGeneralRulesPlugin/TSQLLintGeneralRulesPlugin.csproj -c Release -o artifacts/nuget`

## Rules

See `docs/rules/README.md` for a list of rules.

## How to use

In your TSQLLint runner's "Load Plugin DLL" configuration, specify the build artifact (e.g., `TSQLLintGeneralRulesPlugin/bin/Release/net8.0/TSQLLintGeneralRulesPlugin.dll`) and the plugin type `TSQLLint.Plugin.SqlPlugin`.

Enable or disable rules by specifying the rule ID (`RULE_NAME`) in your environment's TSQLLint configuration.

## Development notes

- When adding a rule, add `*Rule.cs` to `TSQLLintGeneralRulesPlugin/Rules` and add a dictionary entry to `GetRules()` in `TSQLLintGeneralRulesPlugin/SqlPlugin.cs`.
- Add common processing to `TSQLLintGeneralRulesPlugin/Utils`.
- For each rule, add `*RuleTests.cs` to `TSQLLintGeneralRulesPlugin.Tests/` and include both "violation detection cases" and "allowed cases".
