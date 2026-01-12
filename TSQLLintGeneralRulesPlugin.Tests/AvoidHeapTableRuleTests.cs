using TSQLLintGeneralRulesPlugin;
using Xunit;

namespace TSQLLintGeneralRulesPlugin.Tests;

/// <summary>
/// Unit tests for <see cref="AvoidHeapTableRule"/>.
/// </summary>
public sealed class AvoidHeapTableRuleTests
{
    /// <summary>
    /// Verifies that heap tables are flagged.
    /// </summary>
    [Fact]
    public void Flags_WhenMissingClusteredIndex()
    {
        var violations = TestSqlLintRunner.Lint(
            "CREATE TABLE dbo.Customer (Id INT, Name NVARCHAR(100));",
            callback => new AvoidHeapTableRule(callback));

        Assert.Single(violations);
        Assert.Equal("avoid-heap-table", violations[0].RuleName);
    }

    /// <summary>
    /// Verifies that clustered primary keys are allowed.
    /// </summary>
    [Fact]
    public void Allows_ClusteredPrimaryKey()
    {
        var violations = TestSqlLintRunner.Lint(
            "CREATE TABLE dbo.Customer (Id INT NOT NULL, CONSTRAINT PK_Customer PRIMARY KEY CLUSTERED (Id));",
            callback => new AvoidHeapTableRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Verifies that explicit clustered indexes are allowed.
    /// </summary>
    [Fact]
    public void Allows_ClusteredIndexDefinition()
    {
        var violations = TestSqlLintRunner.Lint(
            "CREATE TABLE dbo.Customer (Id INT NOT NULL, INDEX IX_Customer CLUSTERED (Id));",
            callback => new AvoidHeapTableRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Verifies that clustered indexes defined later in the same file are allowed.
    /// </summary>
    [Fact]
    public void Allows_ClusteredIndexInSameFile()
    {
        var violations = TestSqlLintRunner.Lint(
            "CREATE TABLE dbo.Customer (Id INT NOT NULL);" +
            "CREATE CLUSTERED INDEX IX_Customer ON dbo.Customer (Id);",
            callback => new AvoidHeapTableRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Verifies that temporary tables are excluded.
    /// </summary>
    [Fact]
    public void Allows_TemporaryTables()
    {
        var violations = TestSqlLintRunner.Lint(
            "CREATE TABLE #Temp (Id INT);",
            callback => new AvoidHeapTableRule(callback));

        Assert.Empty(violations);
    }
}
