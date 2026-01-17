using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin;

/// <summary>
/// Encourages TRIM(x) instead of nested LTRIM(RTRIM(x)) or RTRIM(LTRIM(x)).
/// </summary>
public sealed class PreferTrimOverLtrimRtrimRule : SqlLintRuleBase
{

    public PreferTrimOverLtrimRtrimRule(Action<string, string, int, int> errorCallback) : base(errorCallback)
    {
    }

    public override string RULE_NAME => "prefer-trim-over-ltrim-rtrim";

    public override string RULE_TEXT => "Prefer TRIM(x) over nested LTRIM(RTRIM(x)) for readability and standardization.";

    public override RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

    public override void Visit(FunctionCall node)
    {
        if (node?.FunctionName?.Value == null)
        {
            base.Visit(node);
            return;
        }

        var outerName = node.FunctionName.Value;
        if (!IsLtrimOrRtrim(outerName))
        {
            base.Visit(node);
            return;
        }

        if (node.Parameters == null || node.Parameters.Count != 1)
        {
            base.Visit(node);
            return;
        }

        var parameter = UnwrapParentheses(node.Parameters[0]);
        if (parameter is FunctionCall innerCall &&
            innerCall.FunctionName?.Value != null &&
            IsLtrimOrRtrim(innerCall.FunctionName.Value) &&
            innerCall.Parameters != null &&
            innerCall.Parameters.Count == 1)
        {
            ReportViolation(node.StartLine, node.StartColumn);
        }

        base.Visit(node);
    }

    public override void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
    {
        // No automatic fix is provided for this rule.
    }

    private static bool IsLtrimOrRtrim(string name)
    {
        return name.Equals("LTRIM", StringComparison.OrdinalIgnoreCase) ||
            name.Equals("RTRIM", StringComparison.OrdinalIgnoreCase);
    }

    private static ScalarExpression UnwrapParentheses(ScalarExpression expression)
    {
        var current = expression;
        while (current is ParenthesisExpression parenthesisExpression && parenthesisExpression.Expression != null)
        {
            current = parenthesisExpression.Expression;
        }

        return current;
    }
}


