using TSQLLintGeneralRulesPlugin;
using Xunit;

namespace TSQLLintGeneralRulesPlugin.Tests;

/// <summary>
/// Unit tests for <see cref="DisallowSelectStarRule"/>.
/// </summary>
public sealed class DisallowSelectStarRuleTests
{
    [Fact]
    public void Flags_WhenSelectStarIsUsed()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT * FROM sys.objects;",
            callback => new DisallowSelectStarRule(callback));

        Assert.Single(violations);
        Assert.Equal("disallow-select-star", violations[0].RuleName);
    }

    [Fact]
    public void Allows_WhenColumnsAreListed()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT o.name FROM sys.objects o;",
            callback => new DisallowSelectStarRule(callback));

        Assert.Empty(violations);
    }
}

