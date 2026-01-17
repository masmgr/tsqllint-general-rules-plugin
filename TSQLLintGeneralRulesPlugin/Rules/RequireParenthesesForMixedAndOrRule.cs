using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin;

/// <summary>
/// Requires explicit parentheses when AND/OR are mixed in a predicate.
/// </summary>
public sealed class RequireParenthesesForMixedAndOrRule : SqlLintRuleBase
{

    public RequireParenthesesForMixedAndOrRule(Action<string, string, int, int> errorCallback) : base(errorCallback)
    {
    }

    public override string RULE_NAME => "require-parentheses-for-mixed-and-or";

    public override string RULE_TEXT => "When mixing AND and OR, add parentheses to make operator precedence explicit and avoid misreads.";

    public override RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

    public override void Visit(WhereClause node)
    {
        if (node?.SearchCondition != null && HasMixedAndOrWithoutParentheses(node.SearchCondition))
        {
            ReportViolation(node.SearchCondition.StartLine, node.SearchCondition.StartColumn);
        }

        base.Visit(node);
    }

    public override void Visit(QualifiedJoin node)
    {
        if (node?.SearchCondition != null && HasMixedAndOrWithoutParentheses(node.SearchCondition))
        {
            ReportViolation(node.SearchCondition.StartLine, node.SearchCondition.StartColumn);
        }

        base.Visit(node);
    }

    public override void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
    {
        // No automatic fix is provided for this rule.
    }

    private static bool HasMixedAndOrWithoutParentheses(BooleanExpression searchCondition)
    {
        var hasAnd = ContainsOperatorAtTopLevel(searchCondition, BooleanBinaryExpressionType.And);
        var hasOr = ContainsOperatorAtTopLevel(searchCondition, BooleanBinaryExpressionType.Or);
        return hasAnd && hasOr;
    }

    private static bool ContainsOperatorAtTopLevel(BooleanExpression expression, BooleanBinaryExpressionType type)
    {
        switch (expression)
        {
            case BooleanParenthesisExpression:
                return false;
            case BooleanNotExpression notExpression when notExpression.Expression != null:
                return ContainsOperatorAtTopLevel(notExpression.Expression, type);
            case BooleanBinaryExpression binary:
                if (binary.BinaryExpressionType == type)
                {
                    return true;
                }

                return (binary.FirstExpression != null && ContainsOperatorAtTopLevel(binary.FirstExpression, type)) ||
                    (binary.SecondExpression != null && ContainsOperatorAtTopLevel(binary.SecondExpression, type));
            default:
                return false;
        }
    }
}


