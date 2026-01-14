using TSQLLintGeneralRulesPlugin;
using Xunit;

namespace TSQLLintGeneralRulesPlugin.Tests;

/// <summary>
/// Unit tests for <see cref="PreferTryConvertPatternsRule"/>.
/// </summary>
public sealed class PreferTryConvertPatternsRuleTests
{
    [Fact]
    public void Flags_WhenCaseUsesIsNumericThenConvert()
    {
        var sql = @"DECLARE @x NVARCHAR(10) = N'1';
SELECT CASE WHEN ISNUMERIC(@x) = 1 THEN CONVERT(INT, @x) END;";
        var violations = TestSqlLintRunner.Lint(sql, callback => new PreferTryConvertPatternsRule(callback));

        Assert.Single(violations);
        Assert.Equal("prefer-try-convert-patterns", violations[0].RuleName);
    }

    [Fact]
    public void Allows_WhenTryConvertIsUsed()
    {
        var sql = @"DECLARE @x NVARCHAR(10) = N'1';
SELECT TRY_CONVERT(INT, @x);";
        var violations = TestSqlLintRunner.Lint(sql, callback => new PreferTryConvertPatternsRule(callback));

        Assert.Empty(violations);
    }
}

