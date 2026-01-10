using TSQLLint.Plugin;
using Xunit;

namespace TSQLLint.Plugin.Tests;

/// <summary>
/// Unit tests for <see cref="RequireColumnListForInsertSelectRule"/>.
/// </summary>
public sealed class RequireColumnListForInsertSelectRuleTests
{
    /// <summary>
    /// Verifies that missing column list in <c>INSERT INTO ... SELECT</c> is detected as a violation.
    /// </summary>
    [Fact]
    public void Flags_WhenColumnListMissing()
    {
        var violations = TestSqlLintRunner.Lint(
            "INSERT INTO dbo.TableName SELECT col FROM dbo.Source;",
            callback => new RequireColumnListForInsertSelectRule(callback));

        Assert.Single(violations);
        Assert.Equal("require-column-list-for-insert-select", violations[0].RuleName);
    }

    /// <summary>
    /// Verifies that <c>INSERT INTO ... SELECT</c> with column list is allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenColumnListPresent()
    {
        var violations = TestSqlLintRunner.Lint(
            "INSERT INTO dbo.TableName (col) SELECT col FROM dbo.Source;",
            callback => new RequireColumnListForInsertSelectRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Verifies that <c>INSERT INTO ... VALUES</c> is allowed.
    /// </summary>
    [Fact]
    public void Allows_InsertValues()
    {
        var violations = TestSqlLintRunner.Lint(
            "INSERT INTO dbo.TableName VALUES (1);",
            callback => new RequireColumnListForInsertSelectRule(callback));

        Assert.Empty(violations);
    }
}
