# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a .NET plugin for `TSQLLint` that provides general-purpose SQL linting rules. The plugin implements `IPlugin` from `TSQLLint.Common` and registers custom rules via `SqlPlugin.GetRules()`.

**Key characteristics:**
- .NET 8.0 target framework
- All rules are general-purpose and reusable across projects
- All rules extend `TSqlFragmentVisitor` and implement `ISqlLintRule`
- Rule violations are detected using the T-SQL AST (Abstract Syntax Tree) from `Microsoft.SqlServer.TransactSql.ScriptDom`
- Tests use xUnit with a custom `TestSqlLintRunner` helper

## Common Commands

**Build:**
```
dotnet build tsqllint-plugin.sln -c Release
```

**Run all tests:**
```
dotnet test tsqllint-plugin.sln -c Release
```

**Run a single test:**
```
dotnet test tsqllint-plugin.sln -c Release --filter "ClassName"
```
Example: `dotnet test tsqllint-plugin.sln -c Release --filter "RequireAsForColumnAliasRuleTests"`

**Pack for NuGet:**
```
dotnet pack tsqllint-plugin/tsqllint-plugin.csproj -c Release -o artifacts/nuget
```
Note: `artifacts/` directory should not be committed (it contains generated artifacts).

## Architecture

### Rule Implementation Pattern

Each rule follows a consistent pattern:

1. **Class structure:** `sealed class RuleName : TSqlFragmentVisitor, ISqlLintRule`
2. **Constructor:** Takes `Action<string, string, int, int>` errorCallback
3. **Required properties:**
   - `RULE_NAME` (string): Kebab-case unique identifier
   - `RULE_TEXT` (string): User-facing violation message
   - `RULE_SEVERITY` (RuleViolationSeverity): Warning, Error, or CriticalError
4. **Visit methods:** Override `Visit(SpecificNodeType)` to traverse the T-SQL AST
5. **Error reporting:** Call `_errorCallback(RULE_NAME, RULE_TEXT, line, column)` to report violations
6. **FixViolation method:** Implement (currently all rules have no-op implementations with comments)

### Key File Structure

```
tsqllint-plugin/
├── SqlPlugin.cs              # Main plugin entry point, registers all rules
├── Rules/                    # General-purpose rules
tsqllint-plugin.Tests/
├── TestSqlLintRunner.cs      # Test helper for running rules against SQL
└── *RuleTests.cs             # One test file per rule

docs/
└── rules/                    # Documentation for each rule (filename matches RULE_NAME)
```

### Rule Registration

All rules must be registered in [SqlPlugin.cs:25-36](tsqllint-plugin/SqlPlugin.cs#L25-L36):
```csharp
public IDictionary<string, ISqlLintRule> GetRules() => new Dictionary<string, ISqlLintRule>
{
    ["rule-id"] = new RuleClass(null),
    // ...
};
```
The dictionary key must match the rule's `RULE_NAME` property. All rules use the "" prefix.

## Adding or Modifying Rules

### Full workflow:

1. **Create the rule class** in `tsqllint-plugin/Rules`
   - Use a descriptive name like `PreferCoalesceOverNestedIsNullRule.cs`
   - Implement `ISqlLintRule` and extend `TSqlFragmentVisitor`
   - Use 1-based line/column numbers (T-SQL parser convention)
   - Use the "" prefix for the rule ID

2. **Register in SqlPlugin.cs**
   - Add entry to `GetRules()` dictionary with the rule ID and instance

3. **Add or update tests** in `tsqllint-plugin.Tests/`
   - Use [TestSqlLintRunner.Lint()](tsqllint-plugin.Tests/TestSqlLintRunner.cs) to test rules
   - Always include both violation-detection cases and allowed cases
   - Example: `TestSqlLintRunner.Lint("SELECT ...", cb => new MyRule(cb))`

4. **Document the rule** in `docs/rules/`
   - Create/update markdown file with name matching the rule ID
   - Include description, examples, and rationale

### T-SQL AST Navigation

When implementing rules, use these key APIs:

- **ScriptTokenStream:** Access to raw tokens for keyword detection
  - `node.ScriptTokenStream[index].TokenType == TSqlTokenType.As` (check for keywords)
  - `node.FirstTokenIndex` / `node.LastTokenIndex` properties on AST nodes

- **AST visitor methods:** Override `Visit(NodeType)` for specific T-SQL constructs
  - `SelectScalarExpression` - SELECT list items
  - `SelectStatement` - SELECT statements
  - `ExecuteStatement` - EXEC statements
  - etc.

Example pattern from [RequireAsForColumnAliasRule.cs:43-82](tsqllint-plugin/Rules/RequireAsForColumnAliasRule.cs#L43-L82):
```csharp
public override void Visit(SelectScalarExpression node)
{
    if (node.ColumnName == null) return;

    var tokens = node.ScriptTokenStream;
    var hasAsKeyword = false;
    for (var i = node.Expression.LastTokenIndex + 1; i < node.ColumnName.FirstTokenIndex; i++)
    {
        if (tokens[i].TokenType == TSqlTokenType.As)
        {
            hasAsKeyword = true;
            break;
        }
    }

    if (!hasAsKeyword)
    {
        _errorCallback?.Invoke(RULE_NAME, RULE_TEXT, node.ColumnName.StartLine, node.ColumnName.StartColumn);
    }
}
```

## Testing

**Test helper:** [TestSqlLintRunner.cs](tsqllint-plugin.Tests/TestSqlLintRunner.cs)
- Parses SQL using `TSql150Parser`
- Applies rules via AST visitor pattern
- Returns list of `RuleViolation` records with rule name, text, line, and column

**Test structure:**
```csharp
[Fact]
public void ShouldDetectViolation()
{
    var violations = TestSqlLintRunner.Lint(
        "SELECT col alias FROM table;",
        cb => new MyRule(cb));

    Assert.Single(violations);
    Assert.Equal("rule-id", violations[0].RuleName);
}
```

## Development Notes

- **Naming convention:** Rule files use PascalCase (`MyRule.cs`), rule IDs use kebab-case (`my-rule`)
- **XML documentation:** Rules and utilities should have XML doc comments (in Japanese per project convention)
- **Null-safety:** Project has `<Nullable>enable</Nullable>` - be explicit about nullability
- **EditorConfig:** Project uses 4-space indentation, LF line endings, trailing newline required
- **No auto-fix:** Most rules intentionally don't implement automatic fixes (see `FixViolation` implementations)

## External Dependencies

- **TSQLLint.Common** v3.3.3 - Plugin interface and base rule infrastructure
- **xUnit** - Test framework
- **Microsoft.SqlServer.TransactSql.ScriptDom** - Implicit dependency via TSQLLint.Common; provides T-SQL AST parsing
