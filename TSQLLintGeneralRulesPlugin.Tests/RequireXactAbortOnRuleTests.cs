using TSQLLint.Plugin;
using Xunit;

namespace TSQLLint.Plugin.Tests;

/// <summary>
/// Unit tests for <see cref="RequireXactAbortOnRule"/>.
/// </summary>
public sealed class RequireXactAbortOnRuleTests
{
    /// <summary>
    /// Tests that the rule is registered and can be instantiated.
    /// </summary>
    [Fact]
    public void Rule_IsRegistered()
    {
        var rule = new RequireXactAbortOnRule(null);
        Assert.Equal("require-xact-abort-on", rule.RULE_NAME);
    }

    /// <summary>
    /// Tests that the rule has a non-empty message.
    /// </summary>
    [Fact]
    public void Rule_HasValidMessage()
    {
        var rule = new RequireXactAbortOnRule(null);
        Assert.NotEmpty(rule.RULE_TEXT);
    }

    /// <summary>
    /// Tests that SELECT statements without transactions don't trigger the rule.
    /// </summary>
    [Fact]
    public void Allows_WhenNoTransaction()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT * FROM dbo.Users;",
            callback => new RequireXactAbortOnRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Tests that UPDATE statements without explicit transactions are allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenImplicitTransaction()
    {
        var violations = TestSqlLintRunner.Lint(
            "UPDATE dbo.Users SET name = 'John' WHERE id = 1;",
            callback => new RequireXactAbortOnRule(callback));

        Assert.Empty(violations);
    }
}
