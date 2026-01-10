using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLint.Plugin
{
    /// <summary>
    /// Requires the explicit specification of JOIN types (INNER, LEFT OUTER, RIGHT OUTER, or FULL OUTER).
    /// </summary>
    public sealed class RequireExplicitJoinTypeRule : TSqlFragmentVisitor, ISqlLintRule
    {
        private readonly Action<string, string, int, int> _errorCallback;

        /// <summary>
        /// Initializes the rule.
        /// </summary>
        /// <param name="errorCallback">Callback invoked when a violation is detected.</param>
        public RequireExplicitJoinTypeRule(Action<string, string, int, int> errorCallback)
        {
            _errorCallback = errorCallback;
        }

        /// <summary>
        /// Gets the rule ID.
        /// </summary>
        public string RULE_NAME => "require-explicit-join-type";

        /// <summary>
        /// Gets the violation message.
        /// </summary>
        public string RULE_TEXT => "JOIN must be explicit: use INNER JOIN, LEFT OUTER JOIN, RIGHT OUTER JOIN, or FULL OUTER JOIN.";

        /// <summary>
        /// Gets the violation severity.
        /// </summary>
        public RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

        /// <summary>
        /// Traverses JOIN clauses and reports violations when INNER or OUTER keywords are omitted.
        /// </summary>
        /// <param name="node">The JOIN clause node to visit.</param>
        public override void Visit(QualifiedJoin node)
        {
            if (node == null)
            {
                base.Visit(node);
                return;
            }

            var tokens = node.ScriptTokenStream;
            if (tokens == null || tokens.Count == 0)
            {
                base.Visit(node);
                return;
            }

            if (!TryGetJoinTokenRange(node, out var startIndex, out var endIndex))
            {
                base.Visit(node);
                return;
            }

            var requiresInner = node.QualifiedJoinType == QualifiedJoinType.Inner;
            var requiresOuter = node.QualifiedJoinType == QualifiedJoinType.LeftOuter ||
                node.QualifiedJoinType == QualifiedJoinType.RightOuter ||
                node.QualifiedJoinType == QualifiedJoinType.FullOuter;

            if ((requiresInner && !HasToken(tokens, startIndex, endIndex, TSqlTokenType.Inner)) ||
                (requiresOuter && !HasToken(tokens, startIndex, endIndex, TSqlTokenType.Outer)))
            {
                var (line, column) = GetJoinTokenPosition(tokens, startIndex, endIndex, node);
                _errorCallback?.Invoke(RULE_NAME, RULE_TEXT, line, column);
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

        private static bool TryGetJoinTokenRange(QualifiedJoin node, out int startIndex, out int endIndex)
        {
            startIndex = -1;
            endIndex = -1;

            var firstEnd = node.FirstTableReference?.LastTokenIndex ?? -1;
            var secondStart = node.SecondTableReference?.FirstTokenIndex ?? -1;
            if (firstEnd < 0 || secondStart < 0 || secondStart <= firstEnd)
            {
                return false;
            }

            startIndex = firstEnd + 1;
            endIndex = secondStart;
            return true;
        }

        private static bool HasToken(IList<TSqlParserToken> tokens, int startIndex, int endIndex, TSqlTokenType tokenType)
        {
            for (var i = startIndex; i < endIndex && i < tokens.Count; i++)
            {
                if (tokens[i].TokenType == tokenType)
                {
                    return true;
                }
            }

            return false;
        }

        private static (int Line, int Column) GetJoinTokenPosition(
            IList<TSqlParserToken> tokens,
            int startIndex,
            int endIndex,
            QualifiedJoin node)
        {
            for (var i = startIndex; i < endIndex && i < tokens.Count; i++)
            {
                if (tokens[i].TokenType == TSqlTokenType.Join)
                {
                    var token = tokens[i];
                    if (token.Line > 0 && token.Column > 0)
                    {
                        return (token.Line, token.Column);
                    }

                    break;
                }
            }

            var line = node.StartLine > 0 ? node.StartLine : node.FirstTableReference.StartLine;
            var column = node.StartColumn > 0 ? node.StartColumn : node.FirstTableReference.StartColumn;
            return (line, column);
        }
    }
}
