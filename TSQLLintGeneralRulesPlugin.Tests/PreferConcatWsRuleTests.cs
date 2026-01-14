using TSQLLintGeneralRulesPlugin;
using Xunit;

namespace TSQLLintGeneralRulesPlugin.Tests;

/// <summary>
/// Unit tests for <see cref="PreferConcatWsRule"/>.
/// </summary>
public sealed class PreferConcatWsRuleTests
{
    [Fact]
    public void Flags_WhenPlusConcatenationRepeatsSeparators()
    {
        var sql = @"DECLARE @a NVARCHAR(10) = NULL, @b NVARCHAR(10) = N'x', @c NVARCHAR(10) = N'y';
SELECT ISNULL(@a, N'') + N',' + ISNULL(@b, N'') + N',' + COALESCE(@c, N'');";
        var violations = TestSqlLintRunner.Lint(sql, callback => new PreferConcatWsRule(callback));

        Assert.Single(violations);
        Assert.Equal("prefer-concat-ws", violations[0].RuleName);
    }

    [Fact]
    public void Allows_WhenConcatWsIsUsed()
    {
        var sql = @"DECLARE @a NVARCHAR(10) = NULL, @b NVARCHAR(10) = N'x', @c NVARCHAR(10) = N'y';
SELECT CONCAT_WS(N',', @a, @b, @c);";
        var violations = TestSqlLintRunner.Lint(sql, callback => new PreferConcatWsRule(callback));

        Assert.Empty(violations);
    }
}

