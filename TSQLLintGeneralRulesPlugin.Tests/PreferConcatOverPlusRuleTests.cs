using TSQLLintGeneralRulesPlugin;
using Xunit;

namespace TSQLLintGeneralRulesPlugin.Tests;

/// <summary>
/// Unit tests for <see cref="PreferConcatOverPlusRule"/>.
/// </summary>
public sealed class PreferConcatOverPlusRuleTests
{
    [Fact]
    public void Flags_WhenPlusConcatenationUsesIsNullOrCoalesce()
    {
        var sql = @"DECLARE @a NVARCHAR(10) = NULL, @b NVARCHAR(10) = N'x';
SELECT ISNULL(@a, N'') + COALESCE(@b, N'');";
        var violations = TestSqlLintRunner.Lint(sql, callback => new PreferConcatOverPlusRule(callback));

        Assert.Single(violations);
        Assert.Equal("prefer-concat-over-plus", violations[0].RuleName);
    }

    [Fact]
    public void Allows_WhenConcatIsUsed()
    {
        var sql = @"DECLARE @a NVARCHAR(10) = NULL, @b NVARCHAR(10) = N'x';
SELECT CONCAT(ISNULL(@a, N''), @b);";
        var violations = TestSqlLintRunner.Lint(sql, callback => new PreferConcatOverPlusRule(callback));

        Assert.Empty(violations);
    }
}

