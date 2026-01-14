using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin;

/// <summary>
/// Forbids TOP 100 PERCENT ORDER BY, which is typically ignored and misleading.
/// </summary>
public sealed class ForbidTop100PercentOrderByRule : TSqlFragmentVisitor, ISqlLintRule
{
    private readonly Action<string, string, int, int> _errorCallback;

    public ForbidTop100PercentOrderByRule(Action<string, string, int, int> errorCallback)
    {
        _errorCallback = errorCallback;
    }

    public string RULE_NAME => "forbid-top-100-percent-order-by";

    public string RULE_TEXT => "Avoid TOP 100 PERCENT ORDER BY; it is redundant and often ignored by the optimizer. Remove TOP 100 PERCENT or move ORDER BY to the outer query.";

    public RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

    public override void Visit(QuerySpecification node)
    {
        if (node == null)
        {
            base.Visit(node);
            return;
        }

        if (node.TopRowFilter?.Percent == true &&
            node.OrderByClause != null &&
            IsHundred(node.TopRowFilter.Expression))
        {
            _errorCallback?.Invoke(RULE_NAME, RULE_TEXT, node.TopRowFilter.StartLine, node.TopRowFilter.StartColumn);
        }

        base.Visit(node);
    }

    public void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
    {
        // No automatic fix is provided for this rule.
    }

    private static bool IsHundred(ScalarExpression? expression)
    {
        return expression switch
        {
            IntegerLiteral integerLiteral => integerLiteral.Value == "100",
            NumericLiteral numericLiteral => numericLiteral.Value == "100" || numericLiteral.Value == "100.0",
            _ => false
        };
    }
}

