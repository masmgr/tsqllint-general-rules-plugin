using TSQLLintGeneralRulesPlugin;
using Xunit;

namespace TSQLLintGeneralRulesPlugin.Tests;

/// <summary>
/// Unit tests for <see cref="RequireQualifiedSelectColumnsRule"/>.
/// </summary>
public sealed class RequireQualifiedSelectColumnsRuleTests
{
    /// <summary>
    /// Verifies that unqualified columns in single-table SELECT are allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenSingleTable()
    {
        var violations = TestSqlLintRunner.Lint(
            """
            SELECT id, name
            FROM users;
            """,
            callback => new RequireQualifiedSelectColumnsRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Verifies that qualified columns in JOIN are allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenQualifiedColumns()
    {
        var violations = TestSqlLintRunner.Lint(
            """
            SELECT u.id, o.id
            FROM users u
            JOIN orders o ON u.id = o.user_id;
            """,
            callback => new RequireQualifiedSelectColumnsRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Verifies that qualified columns in nested expressions are allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenNestedExpressionQualified()
    {
        var violations = TestSqlLintRunner.Lint(
            """
            SELECT COALESCE(u.name, '') AS n
            FROM users u
            JOIN orders o ON u.id = o.user_id;
            """,
            callback => new RequireQualifiedSelectColumnsRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Verifies that unqualified columns in JOIN are detected as a violation.
    /// </summary>
    [Fact]
    public void Flags_WhenUnqualifiedColumnInSelectList()
    {
        var violations = TestSqlLintRunner.Lint(
            """
            SELECT id
            FROM users u
            JOIN orders o ON u.id = o.user_id;
            """,
            callback => new RequireQualifiedSelectColumnsRule(callback));

        Assert.Single(violations);
        Assert.Equal("qualified-select-columns", violations[0].RuleName);
    }

    /// <summary>
    /// Verifies that unqualified columns in CASE expressions are detected as a violation.
    /// </summary>
    [Fact]
    public void Flags_WhenUnqualifiedColumnInCaseExpression()
    {
        var violations = TestSqlLintRunner.Lint(
            """
            SELECT CASE WHEN id > 0 THEN 1 END
            FROM users u
            JOIN orders o ON u.id = o.user_id;
            """,
            callback => new RequireQualifiedSelectColumnsRule(callback));

        Assert.Single(violations);
        Assert.Equal("qualified-select-columns", violations[0].RuleName);
    }

    /// <summary>
    /// Verifies that SELECT * is allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenSelectStar()
    {
        var violations = TestSqlLintRunner.Lint(
            """
            SELECT *
            FROM users u
            JOIN orders o ON u.id = o.user_id;
            """,
            callback => new RequireQualifiedSelectColumnsRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Verifies that qualified * (with alias) is allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenQualifiedStar()
    {
        var violations = TestSqlLintRunner.Lint(
            """
            SELECT u.*
            FROM users u
            JOIN orders o ON u.id = o.user_id;
            """,
            callback => new RequireQualifiedSelectColumnsRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Verifies that subqueries are evaluated independently.
    /// </summary>
    [Fact]
    public void Allows_WhenSubqueryScoped()
    {
        var violations = TestSqlLintRunner.Lint(
            """
            SELECT x.id
            FROM (SELECT id FROM users) x
            JOIN orders o ON x.id = o.user_id;
            """,
            callback => new RequireQualifiedSelectColumnsRule(callback));

        Assert.Empty(violations);
    }
}
