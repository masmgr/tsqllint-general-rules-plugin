using TSQLLintGeneralRulesPlugin;
using Xunit;

namespace TSQLLintGeneralRulesPlugin.Tests;

/// <summary>
/// Unit tests for <see cref="RequireAsForTableAliasRule"/>.
/// </summary>
public sealed class RequireAsForTableAliasRuleTests
{
    /// <summary>
    /// Verifies that missing <c>AS</c> in table aliases is detected as a violation.
    /// </summary>
    [Fact]
    public void Flags_WhenAsMissing()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT * FROM dbo.TableName t;",
            callback => new RequireAsForTableAliasRule(callback));

        Assert.Single(violations);
        Assert.Equal("require-as-for-table-alias", violations[0].RuleName);
    }

    /// <summary>
    /// Verifies that missing <c>AS</c> in derived table aliases is detected as a violation.
    /// </summary>
    [Fact]
    public void Flags_DerivedTableWithoutAs()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT * FROM (SELECT 1 AS Value) t;",
            callback => new RequireAsForTableAliasRule(callback));

        Assert.Single(violations);
        Assert.Equal("require-as-for-table-alias", violations[0].RuleName);
    }

    /// <summary>
    /// Verifies that table aliases with <c>AS</c> are allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenAsPresent()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT * FROM dbo.TableName AS t;",
            callback => new RequireAsForTableAliasRule(callback));

        Assert.Empty(violations);
    }
}
