using TSQLLintGeneralRulesPlugin;
using Xunit;

namespace TSQLLintGeneralRulesPlugin.Tests;

/// <summary>
/// Unit tests for <see cref="PreferConcatOverPlusWhenNullableOrConvertRule"/>.
/// </summary>
public sealed class PreferConcatOverPlusWhenNullableOrConvertRuleTests
{
    /// <summary>
    /// Flags concatenations that mix ISNULL/CONVERT with string literals.
    /// </summary>
    [Fact]
    public void Flags_WhenNullableOrConvertPresentWithLiteral()
    {
        var violations = TestSqlLintRunner.Lint(
            "SET @s = ISNULL(@a, '') + ',' + CONVERT(varchar(10), @d, 120);",
            callback => new PreferConcatOverPlusWhenNullableOrConvertRule(callback));

        Assert.Single(violations);
        Assert.Equal("prefer-concat-over-plus-when-nullable-or-convert", violations[0].RuleName);
    }

    /// <summary>
    /// Allows concatenations that do not involve ISNULL/CONVERT/CAST even if literals exist.
    /// </summary>
    [Fact]
    public void Allows_WhenOnlyPlainAdditionIsUsed()
    {
        var violations = TestSqlLintRunner.Lint(
            "SET @s = @a + ',' + @b;",
            callback => new PreferConcatOverPlusWhenNullableOrConvertRule(callback));

        Assert.Empty(violations);
    }

    /// <summary>
    /// Flags CAST usage when literal string building is present.
    /// </summary>
    [Fact]
    public void Flags_WhenCastCombinedWithLiteralConcatenation()
    {
        var violations = TestSqlLintRunner.Lint(
            "SET @s = 'prefix' + CAST(@d AS NVARCHAR(10));",
            callback => new PreferConcatOverPlusWhenNullableOrConvertRule(callback));

        Assert.Single(violations);
    }
}
