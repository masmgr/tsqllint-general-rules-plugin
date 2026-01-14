using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin
{
    /// <summary>
    /// Prohibits the use of <c>ORDER BY</c> in subqueries unless <c>TOP</c>, <c>OFFSET</c>, <c>FOR XML</c>, or <c>FOR JSON</c> is present.
    /// </summary>
    public sealed class OrderByInSubqueryRule : SqlLintRuleBase
    {
        private int _subqueryContextDepth = 0;

        /// <summary>
        /// Initializes the rule.
        /// </summary>
        /// <param name="errorCallback">Callback invoked when a violation is detected.</param>
        public OrderByInSubqueryRule(Action<string, string, int, int> errorCallback) : base(errorCallback)
        {
        }

        /// <summary>
        /// Gets the rule ID.
        /// </summary>
        public override string RULE_NAME => "order-by-in-subquery";

        /// <summary>
        /// Gets the violation message.
        /// </summary>
        public override string RULE_TEXT => "ORDER BY in subquery without TOP, OFFSET, FOR XML, or FOR JSON is not allowed.";

        /// <summary>
        /// Gets the violation severity.
        /// </summary>
        public override RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Error;

        /// <summary>
        /// Traverses SELECT clauses and detects usage of <c>ORDER BY</c> in subqueries.
        /// </summary>
        /// <param name="node">The SELECT clause node to visit.</param>
        public override void Visit(QuerySpecification node)
        {
            if (node == null)
            {
                base.Visit(node);
                return;
            }

            if (_subqueryContextDepth > 0 && node.OrderByClause != null)
            {
                // Exclude if TOP is present
                if (node.TopRowFilter != null)
                {
                    base.Visit(node);
                    return;
                }

                // Exclude if OFFSET is present
                if (node.OffsetClause != null)
                {
                    base.Visit(node);
                    return;
                }

                // Exclude if FOR XML or FOR JSON is present
                if (HasForXmlOrJsonClause(node))
                {
                    base.Visit(node);
                    return;
                }

                // Report violation if none of the above apply
                ReportViolation(node.OrderByClause.StartLine,
                    node.OrderByClause.StartColumn);
            }

            base.Visit(node);
        }

        /// <summary>
        /// Traverses scalar subqueries and marks their SELECTs as subqueries.
        /// </summary>
        /// <param name="node">The scalar subquery node to visit.</param>
        public override void Visit(ScalarSubquery node)
        {
            if (node == null)
            {
                base.Visit(node);
                return;
            }

            _subqueryContextDepth++;
            base.Visit(node);
            _subqueryContextDepth--;
        }

        /// <summary>
        /// Traverses derived tables and marks their SELECTs as subqueries.
        /// </summary>
        /// <param name="node">The derived table node to visit.</param>
        public override void Visit(QueryDerivedTable node)
        {
            if (node == null)
            {
                base.Visit(node);
                return;
            }

            _subqueryContextDepth++;
            base.Visit(node);
            _subqueryContextDepth--;
        }

        /// <summary>
        /// Traverses CTEs and marks their SELECTs as subqueries.
        /// </summary>
        /// <param name="node">The CTE node to visit.</param>
        public override void Visit(CommonTableExpression node)
        {
            if (node == null)
            {
                base.Visit(node);
                return;
            }

            _subqueryContextDepth++;
            base.Visit(node);
            _subqueryContextDepth--;
        }

        /// <summary>
        /// Determines whether the node has a FOR XML or FOR JSON clause.
        /// </summary>
        /// <param name="node">The node to check.</param>
        /// <returns>true if the node has a FOR XML or FOR JSON clause, false otherwise.</returns>
        private static bool HasForXmlOrJsonClause(QuerySpecification node)
        {
            // To check if a QuerySpecification has FOR XML or FOR JSON through its parent SelectStatement,
            // examine the tokens to look for the FOR XML/FOR JSON keywords.
            if (node?.ScriptTokenStream == null || node.ScriptTokenStream.Count == 0)
            {
                return false;
            }

            var tokens = node.ScriptTokenStream;
            for (var i = node.LastTokenIndex; i >= 0 && i < tokens.Count; i++)
            {
                // Check if FOR keyword is followed by XML keyword
                if (tokens[i].TokenType == TSqlTokenType.For &&
                    i + 1 < tokens.Count)
                {
                    var nextText = tokens[i + 1].Text;
                    if (nextText?.Equals("XML", StringComparison.OrdinalIgnoreCase) == true ||
                        nextText?.Equals("JSON", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Automatically fixes rule violations (no automatic fix is provided for this rule).
        /// </summary>
        /// <param name="fileLines">Array of lines in the file.</param>
        /// <param name="ruleViolation">The rule violation information.</param>
        /// <param name="actions">Line edit actions.</param>
        public override void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
        {
            // No automatic fix is provided for this rule.
        }
    }
}


