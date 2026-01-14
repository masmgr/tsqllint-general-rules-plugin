using TSQLLintGeneralRulesPlugin;
using Xunit;

namespace TSQLLintGeneralRulesPlugin.Tests;

/// <summary>
/// Unit tests for <see cref="ForbidTop100PercentOrderByRule"/>.
/// </summary>
public sealed class ForbidTop100PercentOrderByRuleTests
{
    [Fact]
    public void Flags_WhenTop100PercentOrderByIsUsed()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT TOP 100 PERCENT name FROM sys.objects ORDER BY name;",
            callback => new ForbidTop100PercentOrderByRule(callback));

        Assert.Single(violations);
        Assert.Equal("forbid-top-100-percent-order-by", violations[0].RuleName);
    }

    [Fact]
    public void Allows_WhenTopIsNotPercent()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT TOP 100 name FROM sys.objects ORDER BY name;",
            callback => new ForbidTop100PercentOrderByRule(callback));

        Assert.Empty(violations);
    }
}

