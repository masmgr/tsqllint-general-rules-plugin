using TSQLLintGeneralRulesPlugin;
using Xunit;

namespace TSQLLintGeneralRulesPlugin.Tests;

/// <summary>
/// Unit tests for <see cref="RequireQualifiedColumnsEverywhereRule"/>.
/// </summary>
public sealed class RequireQualifiedColumnsEverywhereRuleTests
{
    [Fact]
    public void Flags_WhenWhereOrOrderByUsesUnqualifiedColumns()
    {
        var sql = @"SELECT a.object_id
FROM sys.objects a
JOIN sys.objects b
    ON a.object_id = b.object_id
WHERE object_id = 1
ORDER BY name;";
        var violations = TestSqlLintRunner.Lint(sql, callback => new RequireQualifiedColumnsEverywhereRule(callback));

        Assert.Equal(2, violations.Count);
        Assert.All(violations, v => Assert.Equal("require-qualified-columns-everywhere", v.RuleName));
    }

    [Fact]
    public void Allows_WhenColumnsAreQualified()
    {
        var sql = @"SELECT a.object_id
FROM sys.objects a
JOIN sys.objects b
    ON a.object_id = b.object_id
WHERE a.object_id = 1
ORDER BY a.name;";
        var violations = TestSqlLintRunner.Lint(sql, callback => new RequireQualifiedColumnsEverywhereRule(callback));

        Assert.Empty(violations);
    }
}

