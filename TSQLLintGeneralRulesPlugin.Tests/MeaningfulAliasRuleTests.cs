using TSQLLintGeneralRulesPlugin;
using Xunit;

namespace TSQLLintGeneralRulesPlugin.Tests;

/// <summary>
/// Unit tests for <see cref="MeaningfulAliasRule"/>.
/// </summary>
public sealed class MeaningfulAliasRuleTests
{
    [Fact]
    public void Flags_WhenSingleCharacterAliasesAreUsedInMultiTableQuery()
    {
        var sql = @"SELECT c.object_id
FROM sys.objects c
JOIN sys.objects o
    ON c.object_id = o.object_id;";
        var violations = TestSqlLintRunner.Lint(sql, callback => new MeaningfulAliasRule(callback));

        Assert.Equal(2, violations.Count);
        Assert.All(violations, v => Assert.Equal("meaningful-alias", v.RuleName));
    }

    [Fact]
    public void Allows_WhenAliasesAreLongerThanOneCharacter()
    {
        var sql = @"SELECT obj1.object_id
FROM sys.objects obj1
JOIN sys.objects obj2
    ON obj1.object_id = obj2.object_id;";
        var violations = TestSqlLintRunner.Lint(sql, callback => new MeaningfulAliasRule(callback));

        Assert.Empty(violations);
    }
}

