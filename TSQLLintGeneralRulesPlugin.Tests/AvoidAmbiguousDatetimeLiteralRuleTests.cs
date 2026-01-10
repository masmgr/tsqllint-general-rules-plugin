using TSQLLintGeneralRulesPlugin;
using Xunit;

namespace TSQLLintGeneralRulesPlugin.Tests;

/// <summary>
/// Unit tests for <see cref="AvoidAmbiguousDatetimeLiteralRule"/>.
/// </summary>
public sealed class AvoidAmbiguousDatetimeLiteralRuleTests
{
    /// <summary>
    /// Tests that slash-delimited date with MM/DD/YYYY format is flagged.
    /// </summary>
    [Fact]
    public void Flags_WhenSlashDelimitedDateMmDdYyyy()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT * FROM dbo.Orders WHERE orderDate = '01/15/2026';",
            callback => new AvoidAmbiguousDatetimeLiteralRule(callback));

        Assert.Single(violations);
        Assert.Equal("avoid-ambiguous-datetime-literal", violations[0].RuleName);
    }

    /// <summary>
    /// Tests that slash-delimited date with YYYY/MM/DD format is flagged.
    /// </summary>
    [Fact]
    public void Flags_WhenSlashDelimitedDateYyyyMmDd()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT * FROM dbo.Orders WHERE orderDate = '2026/01/15';",
            callback => new AvoidAmbiguousDatetimeLiteralRule(callback));

        Assert.Single(violations);
        Assert.Equal("avoid-ambiguous-datetime-literal", violations[0].RuleName);
    }

    /// <summary>
    /// Tests that slash-delimited date with single-digit month and day is flagged.
    /// </summary>
    [Fact]
    public void Flags_WhenSlashDelimitedDateSingleDigits()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT '1/2/2026';",
            callback => new AvoidAmbiguousDatetimeLiteralRule(callback));

        Assert.Single(violations);
    }

    /// <summary>
    /// Tests that ISO 8601 format with dashes is allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenIso8601DateFormat()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT * FROM dbo.Orders WHERE orderDate = '2026-01-15';",
            callback => new AvoidAmbiguousDatetimeLiteralRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Tests that date without delimiters is allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenDateWithoutDelimiters()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT '20260115';",
            callback => new AvoidAmbiguousDatetimeLiteralRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Tests that file paths with slashes are allowed (not date patterns).
    /// </summary>
    [Fact]
    public void Allows_WhenFilePathWithSlashes()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT 'C:/Users/john/documents/file.txt';",
            callback => new AvoidAmbiguousDatetimeLiteralRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Tests that non-date strings with slashes are allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenNonDateStringWithSlashes()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT 'version/2.0/beta';",
            callback => new AvoidAmbiguousDatetimeLiteralRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Tests that ISO 8601 datetime with T separator is allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenIso8601DatetimeWithT()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT '2026-01-15T10:30:00';",
            callback => new AvoidAmbiguousDatetimeLiteralRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Tests that slash-delimited date in INSERT is flagged.
    /// </summary>
    [Fact]
    public void Flags_WhenSlashDelimitedDateInInsert()
    {
        var violations = TestSqlLintRunner.Lint(
            "INSERT INTO dbo.Orders (orderDate) VALUES ('12/25/2026');",
            callback => new AvoidAmbiguousDatetimeLiteralRule(callback));

        Assert.Single(violations);
    }

    /// <summary>
    /// Tests that slash-delimited date in WHERE clause is flagged.
    /// </summary>
    [Fact]
    public void Flags_WhenSlashDelimitedDateInWhere()
    {
        var violations = TestSqlLintRunner.Lint(
            """
            SELECT *
            FROM dbo.Orders
            WHERE orderDate >= '2026/01/01'
              AND orderDate < '2026/02/01';
            """,
            callback => new AvoidAmbiguousDatetimeLiteralRule(callback));

        Assert.Equal(2, violations.Count);
    }
}
