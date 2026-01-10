using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;
using Xunit;

namespace TSQLLint.Plugin.Tests;

/// <summary>
/// A simple runner for rule unit tests.
/// </summary>
internal static class TestSqlLintRunner
{
    /// <summary>
    /// Parses SQL, applies the specified rule, and returns the list of detected violations.
    /// </summary>
    /// <param name="sql">The SQL to validate.</param>
    /// <param name="ruleFactory">A function that takes a callback and returns a rule instance.</param>
    /// <returns>The list of detected violations.</returns>
    public static List<RuleViolation> Lint(
        string sql,
        Func<Action<string, string, int, int>, ISqlLintRule> ruleFactory)
    {
        var violations = new List<RuleViolation>();
        var rule = ruleFactory((name, text, line, column) =>
            violations.Add(new RuleViolation(name, text, line, column)));

        var parser = new TSql150Parser(false);
        using var reader = new StringReader(sql);
        var fragment = parser.Parse(reader, out var errors);

        Assert.Empty(errors);
        fragment.Accept((TSqlFragmentVisitor)rule);

        return violations;
    }

    /// <summary>
    /// A rule violation record formatted for easy use in tests.
    /// </summary>
    internal sealed record RuleViolation(string RuleName, string RuleText, int Line, int Column);
}
