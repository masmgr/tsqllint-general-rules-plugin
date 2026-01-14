using TSQLLintGeneralRulesPlugin;
using Xunit;

namespace TSQLLintGeneralRulesPlugin.Tests;

/// <summary>
/// Unit tests for <see cref="DisallowSelectIntoRule"/>.
/// </summary>
public sealed class DisallowSelectIntoRuleTests
{
    [Fact]
    public void Flags_WhenSelectIntoIsUsed()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT 1 AS A INTO #t;",
            callback => new DisallowSelectIntoRule(callback));

        Assert.Single(violations);
        Assert.Equal("disallow-select-into", violations[0].RuleName);
    }

    [Fact]
    public void Allows_WhenTableIsCreatedExplicitly()
    {
        var sql = @"CREATE TABLE #t (A INT);
INSERT INTO #t (A)
SELECT 1;";
        var violations = TestSqlLintRunner.Lint(sql, callback => new DisallowSelectIntoRule(callback));

        Assert.Empty(violations);
    }
}

