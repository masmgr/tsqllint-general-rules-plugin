using TSQLLint.Plugin;
using Xunit;

namespace TSQLLint.Plugin.Tests;

/// <summary>
/// Unit tests for <see cref="AvoidMergeRule"/>.
/// </summary>
public sealed class AvoidMergeRuleTests
{
    /// <summary>
    /// Tests that MERGE statements are flagged as violations.
    /// </summary>
    [Fact]
    public void Flags_WhenMergeStatementExists()
    {
        var violations = TestSqlLintRunner.Lint(
            """
            MERGE INTO target USING source ON target.id = source.id
            WHEN MATCHED THEN UPDATE SET col = source.col
            WHEN NOT MATCHED THEN INSERT VALUES (source.id, source.col);
            """,
            callback => new AvoidMergeRule(callback));

        Assert.Single(violations);
        Assert.Equal("avoid-merge", violations[0].RuleName);
    }

    /// <summary>
    /// Tests that UPDATE statements are allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenUpdateStatementExists()
    {
        var violations = TestSqlLintRunner.Lint(
            "UPDATE target SET col = 1;",
            callback => new AvoidMergeRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Tests that INSERT statements are allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenInsertStatementExists()
    {
        var violations = TestSqlLintRunner.Lint(
            "INSERT INTO target (col) VALUES (1);",
            callback => new AvoidMergeRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Tests that DELETE statements are allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenDeleteStatementExists()
    {
        var violations = TestSqlLintRunner.Lint(
            "DELETE FROM target WHERE id = 1;",
            callback => new AvoidMergeRule(callback));

        Assert.Empty(violations);
    }
}
