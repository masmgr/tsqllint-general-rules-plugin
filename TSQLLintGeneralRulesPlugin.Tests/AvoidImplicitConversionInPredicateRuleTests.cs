using TSQLLintGeneralRulesPlugin;
using Xunit;

namespace TSQLLintGeneralRulesPlugin.Tests;

/// <summary>
/// Unit tests for <see cref="AvoidImplicitConversionInPredicateRule"/>.
/// </summary>
public sealed class AvoidImplicitConversionInPredicateRuleTests
{
    [Fact]
    public void Flags_WhenPredicateCastsColumn()
    {
        var sql = @"SELECT o.object_id
FROM sys.objects o
WHERE CAST(o.object_id AS NVARCHAR(20)) = N'1';";
        var violations = TestSqlLintRunner.Lint(sql, callback => new AvoidImplicitConversionInPredicateRule(callback));

        Assert.Single(violations);
        Assert.Equal("avoid-implicit-conversion-in-predicate", violations[0].RuleName);
    }

    [Fact]
    public void Allows_WhenPredicateDoesNotConvertColumn()
    {
        var sql = @"SELECT o.object_id
FROM sys.objects o
WHERE o.object_id = 1;";
        var violations = TestSqlLintRunner.Lint(sql, callback => new AvoidImplicitConversionInPredicateRule(callback));

        Assert.Empty(violations);
    }
}

