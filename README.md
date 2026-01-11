# TSQLLintGeneralRulesPlugin

A .NET 8.0 plugin for `TSQLLint` that provides general-purpose SQL linting rules.

This plugin implements `IPlugin` from `TSQLLint.Common` via `SqlPlugin` and registers reusable T-SQL linting rules. All rules analyze T-SQL using the Microsoft SQL Server parser's Abstract Syntax Tree (AST).

## Requirements

- .NET SDK 8.0+
- TSQLLint (compatible with `TSQLLint.Common` v3.3.0+)

## Quick Start

### Build

```bash
dotnet build TSQLLintGeneralRulesPlugin.sln -c Release
```

### Test

```bash
# Run all tests
dotnet test TSQLLintGeneralRulesPlugin.sln -c Release

# Run tests for a specific rule
dotnet test TSQLLintGeneralRulesPlugin.sln -c Release --filter "RequireAsForColumnAliasRuleTests"
```

## Rules

See [docs/rules/README.md](docs/rules/README.md) for a complete list of available rules.

## Installation & Usage

### How It Works

This library is a **TSQLLint plugin** that extends the base functionality of [TSQLLint](https://github.com/tsqllint/tsqllint). TSQLLint plugins are .NET assemblies that implement the `IPlugin` interface from `TSQLLint.Common`. This plugin implements `IPlugin` via the `SqlPlugin` class, which registers custom T-SQL linting rules through the `GetRules()` method.

When TSQLLint loads this plugin, it discovers and applies all registered rules during SQL analysis, using the T-SQL AST (Abstract Syntax Tree) to detect violations.

### Setup Steps

1. Build the plugin:
   ```bash
   dotnet build TSQLLintGeneralRulesPlugin.sln -c Release
   ```

2. Configure TSQLLint to load this plugin by updating your `.tsqllintrc` file:
   ```json
   {
     "rules": {
       "require-as-for-column-alias": "error",
       "other-rule-id": "warning"
     },
     "plugins": {
       "tsqllint-general-rules": "path/to/TSQLLintGeneralRulesPlugin.dll"
     }
   }
   ```

3. Enable/disable rules by rule ID (kebab-case) in the `rules` section of your configuration

### Plugin Architecture

- **Entry Point:** `SqlPlugin` class implements `IPlugin` interface
- **Rule Discovery:** The `GetRules()` method returns a dictionary of all available rules
- **Rule Registration:** Each rule is registered with a unique kebab-case identifier
- **Rule Execution:** TSQLLint applies rules to SQL code through the T-SQL parser's AST visitor pattern

## Architecture

### Rule Implementation

Each rule follows a consistent pattern:

- **Location:** `TSQLLintGeneralRulesPlugin/Rules/`
- **Base class:** Extends `TSqlFragmentVisitor` and implements `ISqlLintRule`
- **Methods:** Override `Visit()` methods to traverse the T-SQL AST
- **Violations:** Report violations via the error callback with rule name, message, line, and column

Example structure:
```csharp
sealed class MyRule : TSqlFragmentVisitor, ISqlLintRule
{
    public string RULE_NAME => "my-rule";
    public string RULE_TEXT => "Description of violation";
    public RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

    public MyRule(Action<string, string, int, int>? errorCallback) { /* ... */ }

    public override void Visit(SelectStatement node) { /* AST traversal */ }
    public void FixViolation(string code) { /* optional fix */ }
}
```

### Rule Registration

All rules are registered in [SqlPlugin.cs](TSQLLintGeneralRulesPlugin/SqlPlugin.cs):

```csharp
public IDictionary<string, ISqlLintRule> GetRules() => new Dictionary<string, ISqlLintRule>
{
    ["my-rule"] = new MyRule(null),
    // ...
};
```

The dictionary key must match the rule's `RULE_NAME` property.

### Testing

Tests use xUnit with a custom `TestSqlLintRunner` helper:

```csharp
[Fact]
public void ShouldDetectViolation()
{
    var violations = TestSqlLintRunner.Lint(
        "SELECT col alias FROM table;",
        cb => new MyRule(cb));

    Assert.Single(violations);
    Assert.Equal("my-rule", violations[0].RuleName);
}
```

See [TSQLLintGeneralRulesPlugin.Tests/TestSqlLintRunner.cs](TSQLLintGeneralRulesPlugin.Tests/TestSqlLintRunner.cs) for the test helper implementation.

## Development

### Adding a New Rule

1. Create the rule class in `TSQLLintGeneralRulesPlugin/Rules/`
   - Use PascalCase filename (e.g., `MyNewRule.cs`)
   - Use kebab-case for the rule ID (e.g., `my-new-rule`)

2. Register the rule in `SqlPlugin.GetRules()` dictionary

3. Add tests in `TSQLLintGeneralRulesPlugin.Tests/`
   - Test both violation-detection and allowed cases
   - Use `TestSqlLintRunner.Lint()` helper

4. Document the rule in `docs/rules/` with a markdown file matching the rule ID

### Key Resources

- **T-SQL AST Navigation:** Use `Visit()` methods to traverse syntax nodes
- **Token Access:** Use `node.ScriptTokenStream` to check for specific keywords
- **Line/Column Numbers:** Use 1-based numbering (T-SQL parser convention)

### Project Configuration

- **Framework:** .NET 8.0
- **Nullable:** Enabled (`<Nullable>enable</Nullable>`)
- **Indentation:** 4 spaces
- **Line endings:** LF with trailing newline
