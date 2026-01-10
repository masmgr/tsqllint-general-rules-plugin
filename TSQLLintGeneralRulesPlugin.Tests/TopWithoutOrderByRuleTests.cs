using TSQLLintGeneralRulesPlugin;
using Xunit;

namespace TSQLLintGeneralRulesPlugin.Tests;

/// <summary>
/// Unit tests for <see cref="TopWithoutOrderByRule"/>.
/// </summary>
public sealed class TopWithoutOrderByRuleTests
{
    /// <summary>
    /// Verifies that <c>TOP</c> without <c>ORDER BY</c> is detected as a violation.
    /// </summary>
    [Fact]
    public void Flags_WhenTopHasNoOrderBy()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT TOP (5) col FROM dbo.TableName;",
            callback => new TopWithoutOrderByRule(callback));

        Assert.Single(violations);
        Assert.Equal("top-without-order-by", violations[0].RuleName);
    }

    /// <summary>
    /// Verifies that <c>TOP</c> with <c>ORDER BY</c> is allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenTopHasOrderBy()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT TOP (5) col FROM dbo.TableName ORDER BY col;",
            callback => new TopWithoutOrderByRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Verifies that queries without <c>TOP</c> are allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenNoTop()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT col FROM dbo.TableName ORDER BY col;",
            callback => new TopWithoutOrderByRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Verifies that <c>TOP (1)</c> with <c>WHERE</c> clause is allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenTopOneWithWhere()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT TOP (1) col FROM dbo.TableName WHERE ID = 1;",
            callback => new TopWithoutOrderByRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Verifies that <c>TOP (1)</c> without <c>WHERE</c> clause is detected as a violation.
    /// </summary>
    [Fact]
    public void Flags_WhenTopOneWithoutWhere()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT TOP (1) col FROM dbo.TableName;",
            callback => new TopWithoutOrderByRule(callback));

        Assert.Single(violations);
        Assert.Equal("top-without-order-by", violations[0].RuleName);
    }

    /// <summary>
    /// Verifies that <c>TOP (5)</c> with <c>WHERE</c> clause is detected as a violation.
    /// </summary>
    [Fact]
    public void Flags_WhenTopGreaterThanOneWithWhere()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT TOP (5) col FROM dbo.TableName WHERE ID = 1;",
            callback => new TopWithoutOrderByRule(callback));

        Assert.Single(violations);
        Assert.Equal("top-without-order-by", violations[0].RuleName);
    }
}
