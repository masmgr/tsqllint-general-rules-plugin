using TSQLLintGeneralRulesPlugin;
using Xunit;

namespace TSQLLintGeneralRulesPlugin.Tests;

/// <summary>
/// Unit tests for <see cref="AvoidNolockOrReadUncommittedRule"/>.
/// </summary>
public sealed class AvoidNolockOrReadUncommittedRuleTests
{
    [Fact]
    public void Flags_WhenNoLockHintIsUsed()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT 1 FROM sys.objects WITH (NOLOCK);",
            callback => new AvoidNolockOrReadUncommittedRule(callback));

        Assert.Single(violations);
        Assert.Equal("avoid-nolock-or-read-uncommitted", violations[0].RuleName);
    }

    [Fact]
    public void Allows_WhenNoIsolationHintIsUsed()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT 1 FROM sys.objects;",
            callback => new AvoidNolockOrReadUncommittedRule(callback));

        Assert.Empty(violations);
    }
}

