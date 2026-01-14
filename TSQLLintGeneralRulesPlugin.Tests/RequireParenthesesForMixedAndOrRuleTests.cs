using TSQLLintGeneralRulesPlugin;
using Xunit;

namespace TSQLLintGeneralRulesPlugin.Tests;

/// <summary>
/// Unit tests for <see cref="RequireParenthesesForMixedAndOrRule"/>.
/// </summary>
public sealed class RequireParenthesesForMixedAndOrRuleTests
{
    [Fact]
    public void Flags_WhenAndOrAreMixedWithoutParentheses()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT 1 WHERE 1 = 1 AND 2 = 2 OR 3 = 3;",
            callback => new RequireParenthesesForMixedAndOrRule(callback));

        Assert.Single(violations);
        Assert.Equal("require-parentheses-for-mixed-and-or", violations[0].RuleName);
    }

    [Fact]
    public void Allows_WhenParenthesesMakePrecedenceExplicit()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT 1 WHERE (1 = 1 AND 2 = 2) OR 3 = 3;",
            callback => new RequireParenthesesForMixedAndOrRule(callback));

        Assert.Empty(violations);
    }
}

