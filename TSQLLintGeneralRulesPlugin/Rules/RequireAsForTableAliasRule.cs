using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin
{
    /// <summary>
    /// Requires the use of the <c>AS</c> keyword for table aliases, including derived tables.
    /// </summary>
    public sealed class RequireAsForTableAliasRule : TSqlFragmentVisitor, ISqlLintRule
    {
        private readonly Action<string, string, int, int> _errorCallback;

        /// <summary>
        /// Initializes the rule.
        /// </summary>
        /// <param name="errorCallback">Callback invoked when a violation is detected.</param>
        public RequireAsForTableAliasRule(Action<string, string, int, int> errorCallback)
        {
            _errorCallback = errorCallback;
        }

        /// <summary>
        /// Gets the rule ID.
        /// </summary>
        public string RULE_NAME => "require-as-for-table-alias";

        /// <summary>
        /// Gets the violation message.
        /// </summary>
        public string RULE_TEXT => "Table aliases must use AS.";

        /// <summary>
        /// Gets the violation severity.
        /// </summary>
        public RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

        /// <summary>
        /// Traverses table references and reports violations when <c>AS</c> is missing before an alias.
        /// </summary>
        /// <param name="node">The table reference node to visit.</param>
        public override void Visit(NamedTableReference node)
        {
            if (node.Alias == null)
            {
                return;
            }

            var tokens = node.ScriptTokenStream;
            if (tokens == null || tokens.Count == 0)
            {
                return;
            }

            var tableEnd = node.SchemaObject?.LastTokenIndex ?? -1;
            var aliasStart = node.Alias.FirstTokenIndex;

            if (tableEnd < 0 || aliasStart < 0 || aliasStart <= tableEnd)
            {
                return;
            }

            if (!HasAsKeyword(tokens, tableEnd + 1, aliasStart))
            {
                _errorCallback?.Invoke(
                    RULE_NAME,
                    RULE_TEXT,
                    node.Alias.StartLine,
                    node.Alias.StartColumn);
            }
        }

        /// <summary>
        /// Traverses derived tables and reports violations when <c>AS</c> is missing before an alias.
        /// </summary>
        /// <param name="node">The derived table node to visit.</param>
        public override void Visit(QueryDerivedTable node)
        {
            if (node.Alias == null)
            {
                return;
            }

            var tokens = node.ScriptTokenStream;
            if (tokens == null || tokens.Count == 0)
            {
                return;
            }

            var queryEnd = node.QueryExpression?.LastTokenIndex ?? -1;
            var aliasStart = node.Alias.FirstTokenIndex;

            if (queryEnd < 0 || aliasStart < 0 || aliasStart <= queryEnd)
            {
                return;
            }

            if (!HasAsKeyword(tokens, queryEnd + 1, aliasStart))
            {
                _errorCallback?.Invoke(
                    RULE_NAME,
                    RULE_TEXT,
                    node.Alias.StartLine,
                    node.Alias.StartColumn);
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

        private static bool HasAsKeyword(IList<TSqlParserToken> tokens, int start, int end)
        {
            for (var i = start; i < end && i < tokens.Count; i++)
            {
                if (tokens[i].TokenType == TSqlTokenType.As)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
