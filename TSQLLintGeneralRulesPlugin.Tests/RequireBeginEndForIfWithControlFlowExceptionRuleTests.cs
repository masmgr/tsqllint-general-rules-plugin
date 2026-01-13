using TSQLLintGeneralRulesPlugin;
using Xunit;

namespace TSQLLintGeneralRulesPlugin.Tests;

/// <summary>
/// Unit tests for <see cref="RequireBeginEndForIfWithControlFlowExceptionRule"/>.
/// </summary>
public sealed class RequireBeginEndForIfWithControlFlowExceptionRuleTests
{
    /// <summary>
    /// Flags an IF clause that lacks BEGIN/END when it contains executable statements.
    /// </summary>
    [Fact]
    public void Flags_WhenIfClauseIsMissingBeginEnd()
    {
        var violations = TestSqlLintRunner.Lint(
            "IF @flag = 1\n    SET @x = 1;",
            callback => new RequireBeginEndForIfWithControlFlowExceptionRule(callback));

        Assert.Single(violations);
        Assert.Equal("require-begin-end-for-if-with-controlflow-exception", violations[0].RuleName);
    }

    /// <summary>
    /// Allows an IF clause that consists only of RETURN.
    /// </summary>
    [Fact]
    public void Allows_WhenIfClauseContainsReturn()
    {
        var violations = TestSqlLintRunner.Lint(
            "IF @flag = 1\n    RETURN;",
            callback => new RequireBeginEndForIfWithControlFlowExceptionRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Allows an ELSE clause that only contains BREAK.
    /// </summary>
    [Fact]
    public void Allows_WhenElseClauseContainsBreak()
    {
        var violations = TestSqlLintRunner.Lint(
            "IF @flag = 1\nBEGIN\n    SET @x = 1;\nEND\nELSE\n    BREAK;",
            callback => new RequireBeginEndForIfWithControlFlowExceptionRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Flags an ELSE clause that is missing BEGIN/END when it has executable statements.
    /// </summary>
    [Fact]
    public void Flags_WhenElseClauseIsMissingBeginEnd()
    {
        var violations = TestSqlLintRunner.Lint(
            "IF @flag = 1\nBEGIN\n    SET @x = 1;\nEND\nELSE\n    SET @x = 0;",
            callback => new RequireBeginEndForIfWithControlFlowExceptionRule(callback));

        Assert.Single(violations);
    }
}
