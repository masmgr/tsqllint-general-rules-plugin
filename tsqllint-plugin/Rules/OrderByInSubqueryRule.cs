using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLint.Plugin
{
    /// <summary>
    /// Prohibits the use of <c>ORDER BY</c> in subqueries unless <c>TOP</c>, <c>OFFSET</c>, or <c>FOR XML</c> is present.
    /// </summary>
    public sealed class OrderByInSubqueryRule : TSqlFragmentVisitor, ISqlLintRule
    {
        private readonly Action<string, string, int, int> _errorCallback;
        private int _queryDepth = 0;

        /// <summary>
        /// Initializes the rule.
        /// </summary>
        /// <param name="errorCallback">Callback invoked when a violation is detected.</param>
        public OrderByInSubqueryRule(Action<string, string, int, int> errorCallback)
        {
            _errorCallback = errorCallback;
        }

        /// <summary>
        /// Gets the rule ID.
        /// </summary>
        public string RULE_NAME => "order-by-in-subquery";

        /// <summary>
        /// Gets the violation message.
        /// </summary>
        public string RULE_TEXT => "ORDER BY in subquery without TOP, OFFSET, or FOR XML is not allowed.";

        /// <summary>
        /// Gets the violation severity.
        /// </summary>
        public RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Error;

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

            _queryDepth++;

            // ORDER BY exists in a subquery (depth >= 2)
            if (_queryDepth >= 2 && node.OrderByClause != null)
            {
                // Exclude if TOP is present
                if (node.TopRowFilter != null)
                {
                    _queryDepth--;
                    base.Visit(node);
                    return;
                }

                // Exclude if OFFSET is present
                if (node.OffsetClause != null)
                {
                    _queryDepth--;
                    base.Visit(node);
                    return;
                }

                // Exclude if FOR XML is present
                if (HasForXmlClause(node))
                {
                    _queryDepth--;
                    base.Visit(node);
                    return;
                }

                // Report violation if none of the above apply
                _errorCallback?.Invoke(
                    RULE_NAME,
                    RULE_TEXT,
                    node.OrderByClause.StartLine,
                    node.OrderByClause.StartColumn);
            }

            base.Visit(node);
            _queryDepth--;
        }

        /// <summary>
        /// Traverses SELECT statements and tracks nesting levels.
        /// </summary>
        /// <param name="node">The SELECT statement node to visit.</param>
        public override void Visit(SelectStatement node)
        {
            if (node == null)
            {
                base.Visit(node);
                return;
            }

            _queryDepth++;
            base.Visit(node);
            _queryDepth--;
        }

        /// <summary>
        /// Determines whether the node has a FOR XML clause.
        /// </summary>
        /// <param name="node">The node to check.</param>
        /// <returns>true if the node has a FOR XML clause, false otherwise.</returns>
        private static bool HasForXmlClause(QuerySpecification node)
        {
            // To check if a QuerySpecification has FOR XML through its parent SelectStatement,
            // examine the tokens to look for the FOR XML keywords.
            if (node?.ScriptTokenStream == null || node.ScriptTokenStream.Count == 0)
            {
                return false;
            }

            var tokens = node.ScriptTokenStream;
            for (var i = node.LastTokenIndex; i >= 0 && i < tokens.Count; i++)
            {
                // Check if FOR keyword is followed by XML keyword
                if (tokens[i].TokenType == TSqlTokenType.For &&
                    i + 1 < tokens.Count &&
                    tokens[i + 1].Text?.Equals("XML", StringComparison.OrdinalIgnoreCase) == true)
                {
                    return true;
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
        public void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
        {
            // No automatic fix is provided for this rule.
        }
    }
}
