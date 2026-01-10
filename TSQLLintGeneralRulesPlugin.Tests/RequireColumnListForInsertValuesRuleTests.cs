using TSQLLint.Plugin;
using Xunit;

namespace TSQLLint.Plugin.Tests;

/// <summary>
/// Unit tests for <see cref="RequireColumnListForInsertValuesRule"/>.
/// </summary>
public sealed class RequireColumnListForInsertValuesRuleTests
{
    /// <summary>
    /// Tests that INSERT VALUES without column list is flagged as violation.
    /// </summary>
    [Fact]
    public void Flags_WhenInsertValuesWithoutColumnList()
    {
        var violations = TestSqlLintRunner.Lint(
            "INSERT INTO dbo.Users VALUES (1, 'John', 'john@example.com');",
            callback => new RequireColumnListForInsertValuesRule(callback));

        Assert.Single(violations);
        Assert.Equal("require-column-list-for-insert-values", violations[0].RuleName);
    }

    /// <summary>
    /// Tests that INSERT VALUES with column list is allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenInsertValuesWithColumnList()
    {
        var violations = TestSqlLintRunner.Lint(
            "INSERT INTO dbo.Users (id, name, email) VALUES (1, 'John', 'john@example.com');",
            callback => new RequireColumnListForInsertValuesRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Tests that INSERT SELECT statements are not flagged by this rule.
    /// </summary>
    [Fact]
    public void Allows_WhenInsertSelectWithoutColumnList()
    {
        var violations = TestSqlLintRunner.Lint(
            "INSERT INTO dbo.Users SELECT id, name, email FROM dbo.Users_Staging;",
            callback => new RequireColumnListForInsertValuesRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Tests that INSERT with multiple VALUE rows without column list is flagged.
    /// </summary>
    [Fact]
    public void Flags_WhenInsertValuesMultipleRowsWithoutColumnList()
    {
        var violations = TestSqlLintRunner.Lint(
            """
            INSERT INTO dbo.Users VALUES
            (1, 'John', 'john@example.com'),
            (2, 'Jane', 'jane@example.com');
            """,
            callback => new RequireColumnListForInsertValuesRule(callback));

        Assert.Single(violations);
        Assert.Equal("require-column-list-for-insert-values", violations[0].RuleName);
    }

    /// <summary>
    /// Tests that INSERT with multiple VALUE rows and column list is allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenInsertValuesMultipleRowsWithColumnList()
    {
        var violations = TestSqlLintRunner.Lint(
            """
            INSERT INTO dbo.Users (id, name, email) VALUES
            (1, 'John', 'john@example.com'),
            (2, 'Jane', 'jane@example.com');
            """,
            callback => new RequireColumnListForInsertValuesRule(callback));

        Assert.Empty(violations);
    }
}
