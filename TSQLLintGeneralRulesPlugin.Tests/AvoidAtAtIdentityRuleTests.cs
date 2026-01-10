using TSQLLintGeneralRulesPlugin;
using Xunit;

namespace TSQLLintGeneralRulesPlugin.Tests;

/// <summary>
/// Unit tests for <see cref="AvoidAtAtIdentityRule"/>.
/// </summary>
public sealed class AvoidAtAtIdentityRuleTests
{
    /// <summary>
    /// Verifies that <c>@@IDENTITY</c> usage is detected as a violation.
    /// </summary>
    [Fact]
    public void Flags_WhenUsesAtAtIdentity()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT @@IDENTITY;",
            callback => new AvoidAtAtIdentityRule(callback));

        Assert.Single(violations);
        Assert.Equal("avoid-atat-identity", violations[0].RuleName);
    }

    /// <summary>
    /// Verifies that <c>SCOPE_IDENTITY()</c> usage is allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenUsesScopeIdentity()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT SCOPE_IDENTITY();",
            callback => new AvoidAtAtIdentityRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Verifies that other global variables are allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenUsesOtherGlobalVariable()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT @@ROWCOUNT;",
            callback => new AvoidAtAtIdentityRule(callback));

        Assert.Empty(violations);
    }
}
