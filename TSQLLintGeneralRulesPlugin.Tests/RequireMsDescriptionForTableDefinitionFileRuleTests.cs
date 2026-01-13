using TSQLLintGeneralRulesPlugin;
using Xunit;

namespace TSQLLintGeneralRulesPlugin.Tests;

/// <summary>
/// Unit tests for <see cref="RequireMsDescriptionForTableDefinitionFileRule"/>.
/// </summary>
public sealed class RequireMsDescriptionForTableDefinitionFileRuleTests
{
    /// <summary>
    /// Flags a table definition that lacks MS_Description metadata.
    /// </summary>
    [Fact]
    public void Flags_WhenDescriptionIsMissing()
    {
        var sql = @"CREATE TABLE dbo.Customer (
    CustomerId INT NOT NULL,
    Name NVARCHAR(100) NOT NULL
);";
        var violations = TestSqlLintRunner.Lint(sql, callback => new RequireMsDescriptionForTableDefinitionFileRule(callback));

        Assert.Single(violations);
        Assert.Equal("require-ms-description-for-table-definition-file", violations[0].RuleName);
    }

    /// <summary>
    /// Allows table definitions that include an MS_Description extended property for the table.
    /// </summary>
    [Fact]
    public void Allows_WhenDescriptionIsProvided()
    {
        var sql = @"CREATE TABLE dbo.Customer (
    CustomerId INT NOT NULL,
    Name NVARCHAR(100) NOT NULL
);
EXEC sys.sp_addextendedproperty
  @name = N'MS_Description', @value = N'Customer master table',
  @level0type = N'SCHEMA', @level0name = N'dbo',
  @level1type = N'TABLE',  @level1name = N'Customer';";
        var violations = TestSqlLintRunner.Lint(sql, callback => new RequireMsDescriptionForTableDefinitionFileRule(callback));

        Assert.Empty(violations);
    }
}
