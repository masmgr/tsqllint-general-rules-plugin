using TSQLLint.Plugin;
using Xunit;

namespace TSQLLint.Plugin.Tests;

/// <summary>
/// Unit tests for <see cref="PreferCoalesceOverNestedIsNullRule"/>.
/// </summary>
public sealed class PreferCoalesceOverNestedIsNullRuleTests
{
    /// <summary>
    /// Verifies that nested <c>ISNULL</c> is detected as a violation.
    /// </summary>
    [Fact]
    public void Flags_NestedIsNull()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT ISNULL(ISNULL(a, b), c) FROM dbo.TableName;",
            callback => new PreferCoalesceOverNestedIsNullRule(callback));

        Assert.Single(violations);
        Assert.Equal("prefer-coalesce-over-nested-isnull", violations[0].RuleName);
    }

    /// <summary>
    /// Verifies that a single <c>ISNULL</c> is allowed.
    /// </summary>
    [Fact]
    public void Allows_SingleIsNull()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT ISNULL(a, b) FROM dbo.TableName;",
            callback => new PreferCoalesceOverNestedIsNullRule(callback));

        Assert.Empty(violations);
    }
}
