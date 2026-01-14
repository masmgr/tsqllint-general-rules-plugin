using TSQLLintGeneralRulesPlugin;
using Xunit;

namespace TSQLLintGeneralRulesPlugin.Tests;

/// <summary>
/// Unit tests for <see cref="RequireBeginEndForWhileRule"/>.
/// </summary>
public sealed class RequireBeginEndForWhileRuleTests
{
    /// <summary>
    /// Flags a WHILE loop that lacks a BEGIN/END block.
    /// </summary>
    [Fact]
    public void Flags_WhenBodyIsMissingBeginEnd()
    {
        var violations = TestSqlLintRunner.Lint(
            "WHILE @i < 10\n    SET @i += 1;",
            callback => new RequireBeginEndForWhileRule(callback));

        Assert.Single(violations);
        Assert.Equal("require-begin-end-for-while", violations[0].RuleName);
        Assert.Equal(1, violations[0].Line);
        Assert.Equal(1, violations[0].Column);
    }

    /// <summary>
    /// Allows WHILE loops that already wrap the body in BEGIN/END.
    /// </summary>
    [Fact]
    public void Allows_WhenBodyUsesBeginEnd()
    {
        var violations = TestSqlLintRunner.Lint(
            "WHILE @i < 10\nBEGIN\n    SET @i += 1;\nEND",
            callback => new RequireBeginEndForWhileRule(callback));

        Assert.Empty(violations);
    }
}
