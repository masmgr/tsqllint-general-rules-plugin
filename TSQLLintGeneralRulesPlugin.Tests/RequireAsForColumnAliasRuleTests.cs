using TSQLLintGeneralRulesPlugin;
using Xunit;

namespace TSQLLintGeneralRulesPlugin.Tests;

/// <summary>
/// Unit tests for <see cref="RequireAsForColumnAliasRule"/>.
/// </summary>
public sealed class RequireAsForColumnAliasRuleTests
{
    /// <summary>
    /// Verifies that missing <c>AS</c> in column aliases is detected as a violation.
    /// </summary>
    [Fact]
    public void Flags_WhenAsMissing()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT col alias FROM dbo.TableName;",
            callback => new RequireAsForColumnAliasRule(callback));

        Assert.Single(violations);
        Assert.Equal("require-as-for-column-alias", violations[0].RuleName);
        Assert.True(violations[0].Line > 0);
        Assert.True(violations[0].Column > 0);
    }

    /// <summary>
    /// Verifies that column aliases with <c>AS</c> are allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenAsPresent()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT col AS alias FROM dbo.TableName;",
            callback => new RequireAsForColumnAliasRule(callback));

        Assert.Empty(violations);
    }
}
