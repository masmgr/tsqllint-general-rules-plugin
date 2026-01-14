using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin;

/// <summary>
/// Disallows SELECT * (including qualified stars like t.*).
/// </summary>
public sealed class DisallowSelectStarRule : TSqlFragmentVisitor, ISqlLintRule
{
    private readonly Action<string, string, int, int> _errorCallback;

    public DisallowSelectStarRule(Action<string, string, int, int> errorCallback)
    {
        _errorCallback = errorCallback;
    }

    public string RULE_NAME => "disallow-select-star";

    public string RULE_TEXT => "Avoid SELECT *; explicitly list columns to prevent breaking changes and improve readability.";

    public RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

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
                    _errorCallback?.Invoke(RULE_NAME, RULE_TEXT, star.StartLine, star.StartColumn);
                }
            }
        }

        base.Visit(node);
    }

    public void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
    {
        // No automatic fix is provided for this rule.
    }
}

