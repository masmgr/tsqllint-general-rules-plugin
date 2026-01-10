using TSQLLint.Plugin;
using Xunit;

namespace TSQLLint.Plugin.Tests;

/// <summary>
/// Unit tests for <see cref="PreferFormatOrDateForDatetimeConversionRule"/>.
/// </summary>
public sealed class PreferFormatOrDateForDatetimeConversionRuleTests
{
    /// <summary>
    /// Verifies that CONVERT to varchar with style is detected as a violation.
    /// </summary>
    [Fact]
    public void Flags_ConvertToVarcharWithStyle()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT CONVERT(varchar(8), GETDATE(), 112);",
            callback => new PreferFormatOrDateForDatetimeConversionRule(callback));

        Assert.Single(violations);
        Assert.Equal("prefer-format-or-date-for-datetime-conversion", violations[0].RuleName);
    }

    /// <summary>
    /// Verifies that CONVERT to nvarchar with style is detected as a violation.
    /// </summary>
    [Fact]
    public void Flags_ConvertToNVarcharWithStyle()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT CONVERT(nvarchar(19), @datetime_var, 120);",
            callback => new PreferFormatOrDateForDatetimeConversionRule(callback));

        Assert.Single(violations);
        Assert.Equal("prefer-format-or-date-for-datetime-conversion", violations[0].RuleName);
    }

    /// <summary>
    /// Verifies that CONVERT to char with style is detected as a violation.
    /// </summary>
    [Fact]
    public void Flags_ConvertToCharWithStyle()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT CONVERT(char(10), CURRENT_TIMESTAMP, 101);",
            callback => new PreferFormatOrDateForDatetimeConversionRule(callback));

        Assert.Single(violations);
        Assert.Equal("prefer-format-or-date-for-datetime-conversion", violations[0].RuleName);
    }

    /// <summary>
    /// Verifies that CONVERT to nchar with style is detected as a violation.
    /// </summary>
    [Fact]
    public void Flags_ConvertToNCharWithStyle()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT CONVERT(nchar(19), date_column, 120);",
            callback => new PreferFormatOrDateForDatetimeConversionRule(callback));

        Assert.Single(violations);
        Assert.Equal("prefer-format-or-date-for-datetime-conversion", violations[0].RuleName);
    }

    /// <summary>
    /// Verifies that CONVERT to date type is allowed.
    /// </summary>
    [Fact]
    public void Allows_ConvertToDateType()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT CONVERT(date, GETDATE());",
            callback => new PreferFormatOrDateForDatetimeConversionRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Verifies that CONVERT to string without style is allowed.
    /// </summary>
    [Fact]
    public void Allows_ConvertToStringWithoutStyle()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT CONVERT(nvarchar(100), some_column);",
            callback => new PreferFormatOrDateForDatetimeConversionRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Verifies that CONVERT of integer to string is allowed.
    /// </summary>
    [Fact]
    public void Allows_ConvertIntToString()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT CONVERT(varchar(10), 123);",
            callback => new PreferFormatOrDateForDatetimeConversionRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Verifies that FORMAT function is allowed.
    /// </summary>
    [Fact]
    public void Allows_FormatFunction()
    {
        var violations = TestSqlLintRunner.Lint(
            "SELECT FORMAT(GETDATE(), 'yyyy-MM-dd');",
            callback => new PreferFormatOrDateForDatetimeConversionRule(callback));

        Assert.Empty(violations);
    }
}
