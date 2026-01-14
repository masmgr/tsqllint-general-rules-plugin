using TSQLLintGeneralRulesPlugin;
using Xunit;

namespace TSQLLintGeneralRulesPlugin.Tests;

/// <summary>
/// Unit tests for <see cref="PreferStringAggOverStuffRule"/>.
/// </summary>
public sealed class PreferStringAggOverStuffRuleTests
{
    [Fact]
    public void Flags_WhenStuffForXmlPathIsUsed()
    {
        var sql = @"SELECT STUFF((
    SELECT ',' + o.name
    FROM sys.objects o
    FOR XML PATH('')
), 1, 1, '');";
        var violations = TestSqlLintRunner.Lint(sql, callback => new PreferStringAggOverStuffRule(callback));

        Assert.Single(violations);
        Assert.Equal("prefer-string-agg-over-stuff", violations[0].RuleName);
    }

    [Fact]
    public void Allows_WhenStringAggIsUsed()
    {
        var sql = @"SELECT STRING_AGG(o.name, ',')
FROM sys.objects o;";
        var violations = TestSqlLintRunner.Lint(sql, callback => new PreferStringAggOverStuffRule(callback));

        Assert.Empty(violations);
    }
}

