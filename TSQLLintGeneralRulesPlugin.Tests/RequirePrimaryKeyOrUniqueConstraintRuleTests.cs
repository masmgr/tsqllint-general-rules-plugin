using TSQLLintGeneralRulesPlugin;
using Xunit;

namespace TSQLLintGeneralRulesPlugin.Tests;

/// <summary>
/// Unit tests for <see cref="RequirePrimaryKeyOrUniqueConstraintRule"/>.
/// </summary>
public sealed class RequirePrimaryKeyOrUniqueConstraintRuleTests
{
    /// <summary>
    /// Verifies that tables without a PRIMARY KEY or UNIQUE constraint are flagged.
    /// </summary>
    [Fact]
    public void Flags_WhenMissingPrimaryKeyOrUnique()
    {
        var violations = TestSqlLintRunner.Lint(
            "CREATE TABLE dbo.Customer (Id INT, Name NVARCHAR(100));",
            callback => new RequirePrimaryKeyOrUniqueConstraintRule(callback));

        Assert.Single(violations);
        Assert.Equal("require-primary-key-or-unique-constraint", violations[0].RuleName);
    }

    /// <summary>
    /// Verifies that PRIMARY KEY constraints are allowed.
    /// </summary>
    [Fact]
    public void Allows_PrimaryKeyConstraint()
    {
        var violations = TestSqlLintRunner.Lint(
            "CREATE TABLE dbo.Customer (Id INT NOT NULL, CONSTRAINT PK_Customer PRIMARY KEY (Id));",
            callback => new RequirePrimaryKeyOrUniqueConstraintRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Verifies that UNIQUE constraints are allowed.
    /// </summary>
    [Fact]
    public void Allows_UniqueConstraint()
    {
        var violations = TestSqlLintRunner.Lint(
            "CREATE TABLE dbo.Customer (Email NVARCHAR(100) UNIQUE);",
            callback => new RequirePrimaryKeyOrUniqueConstraintRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Verifies that UNIQUE indexes defined later in the same file are allowed.
    /// </summary>
    [Fact]
    public void Allows_UniqueIndexInSameFile()
    {
        var violations = TestSqlLintRunner.Lint(
            "CREATE TABLE dbo.Customer (Email NVARCHAR(100));" +
            "CREATE UNIQUE INDEX IX_Customer_Email ON dbo.Customer (Email);",
            callback => new RequirePrimaryKeyOrUniqueConstraintRule(callback));

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
            callback => new RequirePrimaryKeyOrUniqueConstraintRule(callback));

        Assert.Empty(violations);
    }
}
