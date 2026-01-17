using TSQLLintGeneralRulesPlugin;
using Xunit;

namespace TSQLLintGeneralRulesPlugin.Tests;

/// <summary>
/// Unit tests for <see cref="PreferJsonFunctionsRule"/>.
/// </summary>
public sealed class PreferJsonFunctionsRuleTests
{
    [Fact]
    public void Flags_WhenJsonIsBuiltByStringConcatenation()
    {
        var sql = "DECLARE @id INT = 1;\n" +
            "SELECT '{\"id\":' + CAST(@id AS NVARCHAR(10)) + '}';";
        var violations = TestSqlLintRunner.Lint(sql, callback => new PreferJsonFunctionsRule(callback));

        Assert.Single(violations);
        Assert.Equal("prefer-json-functions", violations[0].RuleName);
    }

    [Fact]
    public void Allows_WhenJsonValueIsUsed()
    {
        var sql = "DECLARE @json NVARCHAR(MAX) = N'{\"id\":1}';\n" +
            "SELECT JSON_VALUE(@json, '$.id');";
        var violations = TestSqlLintRunner.Lint(sql, callback => new PreferJsonFunctionsRule(callback));

        Assert.Empty(violations);
    }
}
