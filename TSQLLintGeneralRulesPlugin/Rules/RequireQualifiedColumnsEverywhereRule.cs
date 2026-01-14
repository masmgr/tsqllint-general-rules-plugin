using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin;

/// <summary>
/// Requires column references to be qualified in WHERE/JOIN/ORDER BY when multiple tables are referenced.
/// </summary>
public sealed class RequireQualifiedColumnsEverywhereRule : TSqlFragmentVisitor, ISqlLintRule
{
    private readonly Action<string, string, int, int> _errorCallback;

    public RequireQualifiedColumnsEverywhereRule(Action<string, string, int, int> errorCallback)
    {
        _errorCallback = errorCallback;
    }

    public string RULE_NAME => "require-qualified-columns-everywhere";

    public string RULE_TEXT =>
        "Unqualified column reference in a multi-table query. Qualify it with a table alias (e.g., t.id) in WHERE/JOIN/ORDER BY to avoid ambiguity.";

    public RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

    public override void Visit(QuerySpecification node)
    {
        if (node == null)
        {
            base.Visit(node);
            return;
        }

        if (CountTableReferences(node.FromClause) < 2)
        {
            base.Visit(node);
            return;
        }

        if (node.WhereClause?.SearchCondition != null)
        {
            ReportUnqualifiedColumns(node.WhereClause.SearchCondition);
        }

        if (node.FromClause?.TableReferences != null)
        {
            foreach (var tableReference in node.FromClause.TableReferences)
            {
                ReportUnqualifiedColumnsFromJoinPredicates(tableReference);
            }
        }

        if (node.OrderByClause?.OrderByElements != null)
        {
            foreach (var element in node.OrderByClause.OrderByElements)
            {
                if (element?.Expression != null)
                {
                    ReportUnqualifiedColumns(element.Expression);
                }
            }
        }

        base.Visit(node);
    }

    public void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
    {
        // No automatic fix is provided for this rule.
    }

    private void ReportUnqualifiedColumns(TSqlFragment fragment)
    {
        var visitor = new UnqualifiedColumnVisitor();
        fragment.Accept(visitor);

        foreach (var column in visitor.UnqualifiedColumns)
        {
            _errorCallback?.Invoke(RULE_NAME, RULE_TEXT, column.StartLine, column.StartColumn);
        }
    }

    private void ReportUnqualifiedColumnsFromJoinPredicates(TableReference? tableReference)
    {
        if (tableReference == null)
        {
            return;
        }

        switch (tableReference)
        {
            case QualifiedJoin qualifiedJoin:
                if (qualifiedJoin.SearchCondition != null)
                {
                    ReportUnqualifiedColumns(qualifiedJoin.SearchCondition);
                }

                ReportUnqualifiedColumnsFromJoinPredicates(qualifiedJoin.FirstTableReference);
                ReportUnqualifiedColumnsFromJoinPredicates(qualifiedJoin.SecondTableReference);
                break;
            case UnqualifiedJoin unqualifiedJoin:
                ReportUnqualifiedColumnsFromJoinPredicates(unqualifiedJoin.FirstTableReference);
                ReportUnqualifiedColumnsFromJoinPredicates(unqualifiedJoin.SecondTableReference);
                break;
            case JoinParenthesisTableReference parenthesis:
                ReportUnqualifiedColumnsFromJoinPredicates(parenthesis.Join);
                break;
        }
    }

    private static int CountTableReferences(FromClause? fromClause)
    {
        if (fromClause?.TableReferences == null || fromClause.TableReferences.Count == 0)
        {
            return 0;
        }

        var total = 0;
        foreach (var tableReference in fromClause.TableReferences)
        {
            total += CountTableReferences(tableReference);
        }

        return total;
    }

    private static int CountTableReferences(TableReference? tableReference)
    {
        if (tableReference == null)
        {
            return 0;
        }

        return tableReference switch
        {
            QualifiedJoin qualifiedJoin => CountTableReferences(qualifiedJoin.FirstTableReference) +
                CountTableReferences(qualifiedJoin.SecondTableReference),
            UnqualifiedJoin unqualifiedJoin => CountTableReferences(unqualifiedJoin.FirstTableReference) +
                CountTableReferences(unqualifiedJoin.SecondTableReference),
            JoinParenthesisTableReference parenthesis => CountTableReferences(parenthesis.Join),
            _ => 1
        };
    }

    private sealed class UnqualifiedColumnVisitor : TSqlFragmentVisitor
    {
        private static readonly HashSet<string> DateParts = new(StringComparer.OrdinalIgnoreCase)
        {
            "year", "yy", "yyyy",
            "quarter", "qq", "q",
            "month", "mm", "m",
            "dayofyear", "dy", "y",
            "day", "dd", "d",
            "week", "wk", "ww",
            "weekday", "dw", "w",
            "hour", "hh",
            "minute", "mi", "n",
            "second", "ss", "s",
            "millisecond", "ms",
            "microsecond", "mcs",
            "nanosecond", "ns",
            "timezoneoffset", "tz",
            "iso_week", "isowk", "isoww"
        };

        internal List<ColumnReferenceExpression> UnqualifiedColumns { get; } = new();

        private readonly HashSet<ColumnReferenceExpression> _ignoredColumnReferences = new();

        public override void ExplicitVisit(FunctionCall node)
        {
            if (node?.FunctionName?.Value != null &&
                (node.FunctionName.Value.Equals("DATEADD", StringComparison.OrdinalIgnoreCase) ||
                    node.FunctionName.Value.Equals("DATEDIFF", StringComparison.OrdinalIgnoreCase)) &&
                node.Parameters != null &&
                node.Parameters.Count > 0 &&
                IsDatePartArgument(node.Parameters[0], out var datePartColumnReference))
            {
                _ignoredColumnReferences.Add(datePartColumnReference);
            }

            base.ExplicitVisit(node);
        }

        public override void Visit(ColumnReferenceExpression node)
        {
            if (node == null)
            {
                return;
            }

            if (_ignoredColumnReferences.Contains(node))
            {
                return;
            }

            if (node.ColumnType == ColumnType.Wildcard)
            {
                return;
            }

            var identifiers = node.MultiPartIdentifier?.Identifiers;
            if (identifiers == null || identifiers.Count == 0)
            {
                return;
            }

            if (identifiers.Count < 2)
            {
                UnqualifiedColumns.Add(node);
            }
        }

        public override void Visit(ScalarSubquery node)
        {
            // Subquery is evaluated in its own scope; skip traversing it for the outer query predicate.
        }

        private static bool IsDatePartArgument(ScalarExpression? expression, out ColumnReferenceExpression datePartColumnReference)
        {
            datePartColumnReference = null!;

            if (expression is not ColumnReferenceExpression columnReferenceExpression)
            {
                return false;
            }

            if (columnReferenceExpression.ColumnType == ColumnType.Wildcard)
            {
                return false;
            }

            var identifiers = columnReferenceExpression.MultiPartIdentifier?.Identifiers;
            if (identifiers == null || identifiers.Count != 1)
            {
                return false;
            }

            var value = identifiers[0].Value;
            if (value == null || !DateParts.Contains(value))
            {
                return false;
            }

            datePartColumnReference = columnReferenceExpression;
            return true;
        }
    }
}
