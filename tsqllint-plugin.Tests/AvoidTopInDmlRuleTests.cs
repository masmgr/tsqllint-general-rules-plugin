using TSQLLint.Plugin;
using Xunit;

namespace TSQLLint.Plugin.Tests;

/// <summary>
/// Unit tests for <see cref="AvoidTopInDmlRule"/>.
/// </summary>
public sealed class AvoidTopInDmlRuleTests
{
    /// <summary>
    /// Tests that TOP in UPDATE statements is flagged as violation.
    /// </summary>
    [Fact]
    public void Flags_WhenTopInUpdateStatement()
    {
        var violations = TestSqlLintRunner.Lint(
            "UPDATE TOP (10) dbo.Users SET name = 'test' WHERE active = 1;",
            callback => new AvoidTopInDmlRule(callback));

        Assert.Single(violations);
        Assert.Equal("avoid-top-in-dml", violations[0].RuleName);
    }

    /// <summary>
    /// Tests that TOP in DELETE statements is flagged as violation.
    /// </summary>
    [Fact]
    public void Flags_WhenTopInDeleteStatement()
    {
        var violations = TestSqlLintRunner.Lint(
            "DELETE TOP (100) FROM dbo.AuditLog WHERE logDate < '2020-01-01';",
            callback => new AvoidTopInDmlRule(callback));

        Assert.Single(violations);
        Assert.Equal("avoid-top-in-dml", violations[0].RuleName);
    }

    /// <summary>
    /// Tests that UPDATE statements without TOP are allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenUpdateWithoutTop()
    {
        var violations = TestSqlLintRunner.Lint(
            "UPDATE dbo.Users SET name = 'test' WHERE active = 1;",
            callback => new AvoidTopInDmlRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Tests that DELETE statements without TOP are allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenDeleteWithoutTop()
    {
        var violations = TestSqlLintRunner.Lint(
            "DELETE FROM dbo.AuditLog WHERE logDate < '2020-01-01';",
            callback => new AvoidTopInDmlRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Tests that TOP in SELECT statements are not flagged (different context).
    /// </summary>
    [Fact]
    public void Allows_WhenTopInSelectStatement()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT TOP (10) id, name FROM dbo.Users;",
            callback => new AvoidTopInDmlRule(callback));

        Assert.Empty(violations);
    }
}
