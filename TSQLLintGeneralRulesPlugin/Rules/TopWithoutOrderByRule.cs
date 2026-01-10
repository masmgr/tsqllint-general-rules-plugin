using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin
{
    /// <summary>
    /// Warns when a SELECT statement uses <c>TOP</c> without an <c>ORDER BY</c> clause.
    /// </summary>
    public sealed class TopWithoutOrderByRule : TSqlFragmentVisitor, ISqlLintRule
    {
        private readonly Action<string, string, int, int> _errorCallback;

        /// <summary>
        /// Initializes the rule.
        /// </summary>
        /// <param name="errorCallback">Callback invoked when a violation is detected.</param>
        public TopWithoutOrderByRule(Action<string, string, int, int> errorCallback)
        {
            _errorCallback = errorCallback;
        }

        /// <summary>
        /// Gets the rule ID.
        /// </summary>
        public string RULE_NAME => "top-without-order-by";

        /// <summary>
        /// Gets the violation message.
        /// </summary>
        public string RULE_TEXT => "TOP requires ORDER BY.";

        /// <summary>
        /// Gets the violation severity.
        /// </summary>
        public RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

        /// <summary>
        /// Traverses SELECT clauses and reports violations when <c>TOP</c> is used without <c>ORDER BY</c>.
        /// Excludes the case where <c>TOP (1)</c> is used with a <c>WHERE</c> clause.
        /// </summary>
        /// <param name="node">The SELECT clause node to visit.</param>
        public override void Visit(QuerySpecification node)
        {
            if (node == null || node.TopRowFilter == null)
            {
                base.Visit(node);
                return;
            }

            if (node.OrderByClause != null)
            {
                base.Visit(node);
                return;
            }

            // Exclude the case where TOP (1) is used with a WHERE clause
            if (IsTopOne(node.TopRowFilter) && node.WhereClause != null)
            {
                base.Visit(node);
                return;
            }

            var line = node.TopRowFilter.StartLine > 0 ? node.TopRowFilter.StartLine : node.StartLine;
            var column = node.TopRowFilter.StartColumn > 0 ? node.TopRowFilter.StartColumn : node.StartColumn;
            _errorCallback?.Invoke(RULE_NAME, RULE_TEXT, line, column);

            base.Visit(node);
        }

        /// <summary>
        /// Determines whether the TOP filter is TOP (1).
        /// </summary>
        /// <param name="topRowFilter">The TOP filter.</param>
        /// <returns>true if TOP (1), false otherwise.</returns>
        private static bool IsTopOne(TopRowFilter topRowFilter)
        {
            if (topRowFilter?.Expression == null)
            {
                return false;
            }

            var expression = topRowFilter.Expression;

            // If wrapped in ParenthesisExpression, unwrap it
            if (expression is ParenthesisExpression parenExpr)
            {
                expression = parenExpr.Expression;
            }

            // Check if the value is the integer literal 1
            return expression is IntegerLiteral literal && literal.Value == "1";
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
    }
}
