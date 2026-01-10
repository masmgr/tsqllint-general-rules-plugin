using TSQLLintGeneralRulesPlugin;
using Xunit;

namespace TSQLLintGeneralRulesPlugin.Tests;

/// <summary>
/// Unit tests for <see cref="RequireExplicitJoinTypeRule"/>.
/// </summary>
public sealed class RequireExplicitJoinTypeRuleTests
{
    /// <summary>
    /// Verifies that missing INNER keyword is detected as a violation.
    /// </summary>
    [Fact]
    public void Flags_WhenInnerKeywordMissing()
    {
        var violations = TestSqlLintRunner.Lint(
            """
            SELECT *
            FROM dbo.TableA
            JOIN dbo.TableB ON TableA.Id = TableB.Id;
            """,
            callback => new RequireExplicitJoinTypeRule(callback));

        Assert.Single(violations);
        Assert.Equal("require-explicit-join-type", violations[0].RuleName);
    }

    /// <summary>
    /// Verifies that missing LEFT OUTER keyword is detected as a violation.
    /// </summary>
    [Fact]
    public void Flags_WhenLeftOuterKeywordMissing()
    {
        var violations = TestSqlLintRunner.Lint(
            """
            SELECT *
            FROM dbo.TableA
            LEFT JOIN dbo.TableB ON TableA.Id = TableB.Id;
            """,
            callback => new RequireExplicitJoinTypeRule(callback));

        Assert.Single(violations);
        Assert.Equal("require-explicit-join-type", violations[0].RuleName);
    }

    /// <summary>
    /// Verifies that missing RIGHT OUTER keyword is detected as a violation.
    /// </summary>
    [Fact]
    public void Flags_WhenRightOuterKeywordMissing()
    {
        var violations = TestSqlLintRunner.Lint(
            """
            SELECT *
            FROM dbo.TableA
            RIGHT JOIN dbo.TableB ON TableA.Id = TableB.Id;
            """,
            callback => new RequireExplicitJoinTypeRule(callback));

        Assert.Single(violations);
        Assert.Equal("require-explicit-join-type", violations[0].RuleName);
    }

    /// <summary>
    /// Verifies that explicit join types are allowed.
    /// </summary>
    [Fact]
    public void Allows_WhenJoinTypeExplicit()
    {
        var violations = TestSqlLintRunner.Lint(
            """
            SELECT *
            FROM dbo.TableA
            INNER JOIN dbo.TableB ON TableA.Id = TableB.Id
            LEFT OUTER JOIN dbo.TableC ON TableA.Id = TableC.Id
            RIGHT OUTER JOIN dbo.TableD ON TableA.Id = TableD.Id
            FULL OUTER JOIN dbo.TableE ON TableA.Id = TableE.Id;
            """,
            callback => new RequireExplicitJoinTypeRule(callback));

        Assert.Empty(violations);
    }
}
