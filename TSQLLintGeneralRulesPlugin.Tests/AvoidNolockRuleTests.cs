using TSQLLint.Plugin;
using Xunit;

namespace TSQLLint.Plugin.Tests;

/// <summary>
/// Unit tests for <see cref="AvoidNolockRule"/>.
/// </summary>
public sealed class AvoidNolockRuleTests
{
    /// <summary>
    /// Tests that WITH (NOLOCK) is flagged as violation.
    /// </summary>
    [Fact]
    public void Flags_WhenNoLockHintExists()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT * FROM dbo.Users WITH (NOLOCK);",
            callback => new AvoidNolockRule(callback));

        Assert.Single(violations);
        Assert.Equal("avoid-nolock", violations[0].RuleName);
    }

    /// <summary>
    /// Tests that WITH (READUNCOMMITTED) is flagged as violation.
    /// </summary>
    [Fact]
    public void Flags_WhenReadUncommittedHintExists()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT * FROM dbo.Users WITH (READUNCOMMITTED);",
            callback => new AvoidNolockRule(callback));

        Assert.Single(violations);
        Assert.Equal("avoid-nolock", violations[0].RuleName);
    }

    /// <summary>
    /// Tests that SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED is flagged as violation.
    /// </summary>
    [Fact]
    public void Flags_WhenSetTransactionReadUncommitted()
    {
        var violations = TestSqlLintRunner.Lint(
            "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED; SELECT * FROM dbo.Users;",
            callback => new AvoidNolockRule(callback));

        Assert.Single(violations);
        Assert.Equal("avoid-nolock", violations[0].RuleName);
    }

    /// <summary>
    /// Tests that SELECT without NOLOCK or READUNCOMMITTED is allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenNoUncommittedReadHint()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT * FROM dbo.Users;",
            callback => new AvoidNolockRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Tests that other table hints are allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenOtherTableHintExists()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT * FROM dbo.Users WITH (READCOMMITTEDLOCK);",
            callback => new AvoidNolockRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Tests that READ COMMITTED isolation level is allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenSetTransactionReadCommitted()
    {
        var violations = TestSqlLintRunner.Lint(
            "SET TRANSACTION ISOLATION LEVEL READ COMMITTED; SELECT * FROM dbo.Users;",
            callback => new AvoidNolockRule(callback));

        Assert.Empty(violations);
    }
}
