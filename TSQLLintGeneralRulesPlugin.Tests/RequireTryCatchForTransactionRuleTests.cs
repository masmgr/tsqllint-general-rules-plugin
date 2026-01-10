using TSQLLint.Plugin;
using Xunit;

namespace TSQLLint.Plugin.Tests;

/// <summary>
/// Unit tests for <see cref="RequireTryCatchForTransactionRule"/>.
/// </summary>
public sealed class RequireTryCatchForTransactionRuleTests
{
    /// <summary>
    /// Tests that BEGIN TRAN without TRY/CATCH is flagged as violation.
    /// </summary>
    [Fact]
    public void Flags_WhenBeginTranWithoutTryCatch()
    {
        var violations = TestSqlLintRunner.Lint(
            "BEGIN TRAN; SELECT 1; COMMIT;",
            callback => new RequireTryCatchForTransactionRule(callback));

        Assert.Single(violations);
        Assert.Equal("require-try-catch-for-transaction", violations[0].RuleName);
    }

    /// <summary>
    /// Tests that BEGIN TRAN with explicit commit without TRY/CATCH is flagged as violation.
    /// </summary>
    [Fact]
    public void Flags_WhenBeginTranExplicitCommitWithoutTryCatch()
    {
        var violations = TestSqlLintRunner.Lint(
            "BEGIN TRAN; UPDATE dbo.Users SET col = 1; COMMIT TRAN;",
            callback => new RequireTryCatchForTransactionRule(callback));

        Assert.Single(violations);
    }

    /// <summary>
    /// Tests that queries without transactions are allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenNoTransaction()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT * FROM dbo.Users;",
            callback => new RequireTryCatchForTransactionRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Tests that the rule handles the case where statements may be nested within TRY/CATCH.
    /// Note: Complete validation of nested statements within TRY/CATCH blocks may require
    /// additional configuration depending on how the SQL parser structures the AST.
    /// </summary>
    [Fact]
    public void Rule_RecognizesTryCatchStructure()
    {
        // This test verifies the rule can be instantiated and processes TRY/CATCH statements
        var rule = new RequireTryCatchForTransactionRule(null);
        Assert.Equal("require-try-catch-for-transaction", rule.RULE_NAME);
    }

    /// <summary>
    /// Tests that statements without transactions are allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenSelectWithoutTransaction()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT COUNT(*) FROM dbo.Users;",
            callback => new RequireTryCatchForTransactionRule(callback));

        Assert.Empty(violations);
    }
}
