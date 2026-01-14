using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin;

/// <summary>
/// Warns when predicates apply conversions to column expressions, which can lead to implicit conversions and poor index usage.
/// </summary>
public sealed class AvoidImplicitConversionInPredicateRule : SqlLintRuleBase
{
    private bool _inPredicate;

    public AvoidImplicitConversionInPredicateRule(Action<string, string, int, int> errorCallback) : base(errorCallback)
    {
    }

    public override string RULE_NAME => "avoid-implicit-conversion-in-predicate";

    public override string RULE_TEXT => "Avoid conversions on columns in WHERE/JOIN predicates (e.g., CAST(col AS ...) = ...); they can force scans by preventing index seeks. Prefer correctly typed parameters/literals instead.";

    public override RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

    public override void ExplicitVisit(WhereClause node)
    {
        if (node == null)
        {
            return;
        }

        var previous = _inPredicate;
        _inPredicate = true;
        try
        {
            base.ExplicitVisit(node);
        }
        finally
        {
            _inPredicate = previous;
        }
    }

    public override void ExplicitVisit(QualifiedJoin node)
    {
        if (node == null)
        {
            return;
        }

        var previous = _inPredicate;
        _inPredicate = true;
        try
        {
            base.ExplicitVisit(node);
        }
        finally
        {
            _inPredicate = previous;
        }
    }

    public override void ExplicitVisit(UnqualifiedJoin node)
    {
        if (node == null)
        {
            return;
        }

        var previous = _inPredicate;
        _inPredicate = true;
        try
        {
            base.ExplicitVisit(node);
        }
        finally
        {
            _inPredicate = previous;
        }
    }

    public override void Visit(BooleanComparisonExpression node)
    {
        if (_inPredicate && node != null)
        {
            if (IsConversionAppliedToColumn(node.FirstExpression) || IsConversionAppliedToColumn(node.SecondExpression))
            {
                ReportViolation(node.StartLine, node.StartColumn);
            }
        }

        base.Visit(node);
    }

    public override void Visit(LikePredicate node)
    {
        if (_inPredicate && node != null)
        {
            if (IsConversionAppliedToColumn(node.FirstExpression) || IsConversionAppliedToColumn(node.SecondExpression))
            {
                ReportViolation(node.StartLine, node.StartColumn);
            }
        }

        base.Visit(node);
    }

    public override void Visit(InPredicate node)
    {
        if (_inPredicate && node?.Expression != null)
        {
            if (IsConversionAppliedToColumn(node.Expression))
            {
                ReportViolation(node.StartLine, node.StartColumn);
            }
        }

        base.Visit(node);
    }

    public override void Visit(BooleanTernaryExpression node)
    {
        if (_inPredicate && node != null)
        {
            if (node.TernaryExpressionType == BooleanTernaryExpressionType.Between &&
                (IsConversionAppliedToColumn(node.FirstExpression) ||
                    IsConversionAppliedToColumn(node.SecondExpression) ||
                    IsConversionAppliedToColumn(node.ThirdExpression)))
            {
                ReportViolation(node.StartLine, node.StartColumn);
            }
        }

        base.Visit(node);
    }

    public override void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
    {
        // No automatic fix is provided for this rule.
    }

    private static bool IsConversionAppliedToColumn(ScalarExpression? expression)
    {
        expression = UnwrapParentheses(expression);
        if (expression == null)
        {
            return false;
        }

        switch (expression)
        {
            case ConvertCall convertCall:
                return ContainsColumnReference(convertCall.Parameter);
            case CastCall castCall:
                return ContainsColumnReference(castCall.Parameter);
            case FunctionCall functionCall when functionCall.FunctionName?.Value != null:
                var name = functionCall.FunctionName.Value;
                if (name.Equals("TRY_CONVERT", StringComparison.OrdinalIgnoreCase) ||
                    name.Equals("TRY_CAST", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var parameter in functionCall.Parameters)
                    {
                        if (ContainsColumnReference(parameter))
                        {
                            return true;
                        }
                    }
                }

                return false;
            default:
                return false;
        }
    }

    private static ScalarExpression? UnwrapParentheses(ScalarExpression? expression)
    {
        var current = expression;
        while (current is ParenthesisExpression parenthesisExpression && parenthesisExpression.Expression != null)
        {
            current = parenthesisExpression.Expression;
        }

        return current;
    }

    private static bool ContainsColumnReference(TSqlFragment? fragment)
    {
        if (fragment == null)
        {
            return false;
        }

        var finder = new ColumnReferenceFinder();
        fragment.Accept(finder);
        return finder.Found;
    }

    private sealed class ColumnReferenceFinder : TSqlFragmentVisitor
    {
        public bool Found { get; private set; }

        public override void Visit(ColumnReferenceExpression node)
        {
            if (node?.ColumnType != ColumnType.Wildcard)
            {
                Found = true;
            }
        }
    }
}


