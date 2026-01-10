using TSQLLintGeneralRulesPlugin;
using Xunit;

namespace TSQLLintGeneralRulesPlugin.Tests;

/// <summary>
/// Unit tests for <see cref="AvoidNullComparisonRule"/>.
/// </summary>
public sealed class AvoidNullComparisonRuleTests
{
    /// <summary>
    /// Tests that = NULL comparison is flagged as violation.
    /// </summary>
    [Fact]
    public void Flags_WhenEqualsNullComparison()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT * FROM dbo.Users WHERE name = NULL;",
            callback => new AvoidNullComparisonRule(callback));

        Assert.Single(violations);
        Assert.Equal("avoid-null-comparison", violations[0].RuleName);
    }

    /// <summary>
    /// Tests that <> NULL comparison is flagged as violation.
    /// </summary>
    [Fact]
    public void Flags_WhenNotEqualNullComparison()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT * FROM dbo.Users WHERE name <> NULL;",
            callback => new AvoidNullComparisonRule(callback));

        Assert.Single(violations);
        Assert.Equal("avoid-null-comparison", violations[0].RuleName);
    }

    /// <summary>
    /// Tests that != NULL comparison is flagged as violation.
    /// </summary>
    [Fact]
    public void Flags_WhenNotEqualExclamationNullComparison()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT * FROM dbo.Users WHERE name != NULL;",
            callback => new AvoidNullComparisonRule(callback));

        Assert.Single(violations);
        Assert.Equal("avoid-null-comparison", violations[0].RuleName);
    }

    /// <summary>
    /// Tests that IS NULL is allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenIsNullComparison()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT * FROM dbo.Users WHERE name IS NULL;",
            callback => new AvoidNullComparisonRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Tests that IS NOT NULL is allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenIsNotNullComparison()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT * FROM dbo.Users WHERE name IS NOT NULL;",
            callback => new AvoidNullComparisonRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Tests that comparisons with non-NULL values are allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenComparisonWithNonNullValue()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT * FROM dbo.Users WHERE name = 'John';",
            callback => new AvoidNullComparisonRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Tests that NULL comparison in JOIN ON clause is flagged.
    /// </summary>
    [Fact]
    public void Flags_WhenNullComparisonInJoinOn()
    {
        var violations = TestSqlLintRunner.Lint(
            """
            SELECT *
            FROM dbo.Users u
            JOIN dbo.Orders o ON u.id = o.user_id AND u.status = NULL;
            """,
            callback => new AvoidNullComparisonRule(callback));

        Assert.Single(violations);
    }

    /// <summary>
    /// Tests that NULL comparison in CASE statement is flagged.
    /// </summary>
    [Fact]
    public void Flags_WhenNullComparisonInCaseStatement()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT CASE WHEN status = NULL THEN 'Null' ELSE 'Not Null' END FROM dbo.Users;",
            callback => new AvoidNullComparisonRule(callback));

        Assert.Single(violations);
    }
}
