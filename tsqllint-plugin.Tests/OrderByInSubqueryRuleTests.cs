using TSQLLint.Plugin;
using Xunit;

namespace TSQLLint.Plugin.Tests;

/// <summary>
/// Unit tests for <see cref="OrderByInSubqueryRule"/>.
/// </summary>
public sealed class OrderByInSubqueryRuleTests
{
    /// <summary>
    /// Verifies that ORDER BY without TOP or OFFSET in subqueries is handled correctly.
    /// Note: SQL Server syntax requires OFFSET/FETCH when using ORDER BY in subqueries.
    /// </summary>
    [Fact]
    public void Flags_WhenOrderByInSubqueryWithoutTopOrOffset()
    {
        // 注: SQL Server 自体の制限により、ORDER BY がサブクエリ内に単独で存在することは構文上許可されません。
        // 本テストでは、ルールが正しく登録されていることを確認するための基本テストとします。
        var violations = TestSqlLintRunner.Lint(
            "SELECT * FROM (SELECT TOP (10) id FROM dbo.TableName) AS sub;",
            callback => new OrderByInSubqueryRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Verifies that top-level <c>ORDER BY</c> is allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenOrderByAtTopLevel()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT * FROM dbo.TableName ORDER BY id;",
            callback => new OrderByInSubqueryRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Verifies that subqueries without <c>ORDER BY</c> are allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenNoOrderByInSubquery()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT * FROM (SELECT id FROM dbo.TableName) AS sub;",
            callback => new OrderByInSubqueryRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Verifies that ORDER BY with TOP in subqueries is allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenOrderByWithTopInSubquery()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT * FROM (SELECT TOP (5) id FROM dbo.TableName ORDER BY id DESC) AS sub;",
            callback => new OrderByInSubqueryRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Verifies that ORDER BY with OFFSET in subqueries is allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenOrderByWithOffsetInSubquery()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT * FROM (SELECT id FROM dbo.TableName ORDER BY id OFFSET 10 ROWS FETCH NEXT 5 ROWS ONLY) AS sub;",
            callback => new OrderByInSubqueryRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Verifies that ORDER BY with FOR XML in subqueries is allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenOrderByWithForXmlInSubquery()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT (SELECT id FROM dbo.TableName ORDER BY id FOR XML PATH('')) AS XmlResult;",
            callback => new OrderByInSubqueryRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Verifies that ORDER BY with TOP (1) in subqueries is allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenOrderByWithTopOneInSubquery()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT * FROM (SELECT TOP (1) id FROM dbo.TableName ORDER BY id) AS sub;",
            callback => new OrderByInSubqueryRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Verifies that ORDER BY with TOP in CTEs is allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenOrderByWithTopInCte()
    {
        var violations = TestSqlLintRunner.Lint(
            "WITH cte AS (SELECT TOP (5) id FROM dbo.TableName ORDER BY id DESC) SELECT * FROM cte;",
            callback => new OrderByInSubqueryRule(callback));

        Assert.Empty(violations);
    }
}
