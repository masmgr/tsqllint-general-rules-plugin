using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin
{
    /// <summary>
    /// Requires the use of the <c>AS</c> keyword for column aliases.
    /// </summary>
    public sealed class RequireAsForColumnAliasRule : TSqlFragmentVisitor, ISqlLintRule
    {
        private readonly Action<string, string, int, int> _errorCallback;

        /// <summary>
        /// Initializes the rule.
        /// </summary>
        /// <param name="errorCallback">Callback invoked when a violation is detected.</param>
        public RequireAsForColumnAliasRule(Action<string, string, int, int> errorCallback)
        {
            _errorCallback = errorCallback;
        }

        /// <summary>
        /// Gets the rule ID.
        /// </summary>
        public string RULE_NAME => "require-as-for-column-alias";

        /// <summary>
        /// Gets the violation message.
        /// </summary>
        public string RULE_TEXT => "Column aliases must use AS.";

        /// <summary>
        /// Gets the violation severity.
        /// </summary>
        public RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

        /// <summary>
        /// Traverses scalar expressions in the SELECT clause and reports violations when <c>AS</c> is missing before a column alias.
        /// </summary>
        /// <param name="node">The scalar expression node to visit.</param>
        public override void Visit(SelectScalarExpression node)
        {
            if (node.ColumnName == null || node.Expression == null)
            {
                return;
            }

            var tokens = node.ScriptTokenStream;
            if (tokens == null || tokens.Count == 0)
            {
                return;
            }

            var expressionEnd = node.Expression.LastTokenIndex;
            var aliasStart = node.ColumnName.FirstTokenIndex;

            if (expressionEnd < 0 || aliasStart < 0 || aliasStart <= expressionEnd)
            {
                return;
            }

            var hasAsKeyword = false;
            for (var i = expressionEnd + 1; i < aliasStart; i++)
            {
                if (tokens[i].TokenType == TSqlTokenType.As)
                {
                    hasAsKeyword = true;
                    break;
                }
            }

            if (!hasAsKeyword)
            {
                _errorCallback?.Invoke(
                    RULE_NAME,
                    RULE_TEXT,
                    node.ColumnName.StartLine,
                    node.ColumnName.StartColumn);
            }
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
