using TSQLLintGeneralRulesPlugin;
using Xunit;

namespace TSQLLintGeneralRulesPlugin.Tests;

/// <summary>
/// Unit tests for <see cref="PreferTrimOverLtrimRtrimRule"/>.
/// </summary>
public sealed class PreferTrimOverLtrimRtrimRuleTests
{
    [Fact]
    public void Flags_WhenLtrimRtrimIsNested()
    {
        var sql = @"DECLARE @a NVARCHAR(10) = N' x ';
SELECT LTRIM(RTRIM(@a));";
        var violations = TestSqlLintRunner.Lint(sql, callback => new PreferTrimOverLtrimRtrimRule(callback));

        Assert.Single(violations);
        Assert.Equal("prefer-trim-over-ltrim-rtrim", violations[0].RuleName);
    }

    [Fact]
    public void Allows_WhenTrimIsUsed()
    {
        var sql = @"DECLARE @a NVARCHAR(10) = N' x ';
SELECT TRIM(@a);";
        var violations = TestSqlLintRunner.Lint(sql, callback => new PreferTrimOverLtrimRtrimRule(callback));

        Assert.Empty(violations);
    }
}

