using TSQLLintGeneralRulesPlugin;
using Xunit;

namespace TSQLLintGeneralRulesPlugin.Tests;

/// <summary>
/// Unit tests for <see cref="AvoidExecDynamicSqlRule"/>.
/// </summary>
public sealed class AvoidExecDynamicSqlRuleTests
{
    /// <summary>
    /// Verifies that dynamic SQL <c>EXEC</c> execution is detected as a violation.
    /// </summary>
    [Fact]
    public void Flags_WhenExecUsesVariable()
    {
        var violations = TestSqlLintRunner.Lint(
            "DECLARE @sql nvarchar(max) = N'SELECT 1'; EXEC(@sql);",
            callback => new AvoidExecDynamicSqlRule(callback));

        Assert.Single(violations);
        Assert.Equal("avoid-exec-dynamic-sql", violations[0].RuleName);
    }

    /// <summary>
    /// Verifies that dynamic SQL execution with <c>EXECUTE</c> is detected as a violation.
    /// </summary>
    [Fact]
    public void Flags_WhenExecuteUsesVariable()
    {
        var violations = TestSqlLintRunner.Lint(
            "DECLARE @sql nvarchar(max) = N'SELECT 1'; EXECUTE(@sql);",
            callback => new AvoidExecDynamicSqlRule(callback));

        Assert.Single(violations);
        Assert.Equal("avoid-exec-dynamic-sql", violations[0].RuleName);
    }

    /// <summary>
    /// Verifies that <c>sp_executesql</c> usage is allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenUsingSpExecutesql()
    {
        var violations = TestSqlLintRunner.Lint(
            "DECLARE @sql nvarchar(max) = N'SELECT @Id'; EXEC sp_executesql @sql, N'@Id int', @Id = 1;",
            callback => new AvoidExecDynamicSqlRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Verifies that stored procedure execution is allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenExecutingStoredProcedure()
    {
        var violations = TestSqlLintRunner.Lint(
            "EXEC dbo.ProcName @Id = 1;",
            callback => new AvoidExecDynamicSqlRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Verifies that EXEC with string literals is allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenExecUsesLiteral()
    {
        var violations = TestSqlLintRunner.Lint(
            "EXEC (N'SELECT 1');",
            callback => new AvoidExecDynamicSqlRule(callback));

        Assert.Empty(violations);
    }
}
