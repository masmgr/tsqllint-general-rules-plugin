using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin;

/// <summary>
/// Disallows SELECT * (including qualified stars like t.*).
/// </summary>
public sealed class DisallowSelectStarRule : SqlLintRuleBase
{

    public DisallowSelectStarRule(Action<string, string, int, int> errorCallback) : base(errorCallback)
    {
    }

    public override string RULE_NAME => "disallow-select-star";

    public override string RULE_TEXT => "Avoid SELECT *; explicitly list columns to prevent breaking changes and improve readability.";

    public override RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

    public override void Visit(QuerySpecification node)
    {
        if (node == null)
        {
            base.Visit(node);
            return;
        }

        if (node.SelectElements != null)
        {
            foreach (var element in node.SelectElements)
            {
                if (element is SelectStarExpression star)
                {
                    ReportViolation(star.StartLine, star.StartColumn);
                }
            }
        }

        base.Visit(node);
    }

    public override void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
    {
        // No automatic fix is provided for this rule.
    }
}


