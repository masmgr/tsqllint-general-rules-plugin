using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin
{
    /// <summary>
    /// Requires column references in the SELECT list to be qualified when multiple tables are referenced.
    /// </summary>
    public sealed class RequireQualifiedSelectColumnsRule : TSqlFragmentVisitor, ISqlLintRule
    {
        private readonly Action<string, string, int, int> _errorCallback;

        /// <summary>
        /// Initializes the rule.
        /// </summary>
        /// <param name="errorCallback">Callback invoked when a violation is detected.</param>
        public RequireQualifiedSelectColumnsRule(Action<string, string, int, int> errorCallback)
        {
            _errorCallback = errorCallback;
        }

        /// <summary>
        /// Gets the rule ID.
        /// </summary>
        public string RULE_NAME => "qualified-select-columns";

        /// <summary>
        /// Gets the violation message.
        /// </summary>
        public string RULE_TEXT =>
            "Unqualified column reference in SELECT list when multiple tables are referenced. Qualify it with table alias (e.g., t.id).";

        /// <summary>
        /// Gets the violation severity.
        /// </summary>
        public RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

        /// <summary>
        /// Traverses the SELECT clause and detects unqualified columns when multiple tables are referenced.
        /// </summary>
        /// <param name="node">The SELECT clause node to visit.</param>
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

            if (node.SelectElements == null || node.SelectElements.Count == 0)
            {
                base.Visit(node);
                return;
            }

            foreach (var element in node.SelectElements)
            {
                switch (element)
                {
                    case SelectStarExpression:
                        continue;
                    case SelectScalarExpression scalarExpression:
                        ReportUnqualifiedColumns(scalarExpression.Expression);
                        break;
                    case SelectSetVariable setVariable:
                        ReportUnqualifiedColumns(setVariable.Expression);
                        break;
                }
            }

            base.Visit(node);
        }

        /// <summary>
        /// Automatically fixes rule violations (no automatic fix is provided for this rule).
        /// </summary>
        /// <param name="fileLines">Array of lines in the file.</param>
        /// <param name="ruleViolation">The rule violation information.</param>
        /// <param name="actions">Line edit actions.</param>
        public void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
        {
            // No automatic fix is provided for this rule.
        }

        private void ReportUnqualifiedColumns(ScalarExpression? expression)
        {
            if (expression == null)
            {
                return;
            }

            var visitor = new UnqualifiedColumnVisitor();
            expression.Accept(visitor);

            foreach (var column in visitor.UnqualifiedColumns)
            {
                var line = column.StartLine;
                var columnPosition = column.StartColumn;
                _errorCallback?.Invoke(RULE_NAME, RULE_TEXT, line, columnPosition);
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
                // Subquery is evaluated in its own scope; skip traversing it for the outer SELECT list.
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
}
