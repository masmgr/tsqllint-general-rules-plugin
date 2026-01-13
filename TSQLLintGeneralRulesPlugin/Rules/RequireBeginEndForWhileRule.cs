using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin
{
    /// <summary>
    /// Ensures WHILE loops always wrap their body in BEGIN/END.
    /// </summary>
    public sealed class RequireBeginEndForWhileRule : TSqlFragmentVisitor, ISqlLintRule
    {
        private readonly Action<string, string, int, int> _errorCallback;

        /// <summary>
        /// Initializes the rule.
        /// </summary>
        /// <param name="errorCallback">Callback invoked for violations.</param>
        public RequireBeginEndForWhileRule(Action<string, string, int, int> errorCallback)
        {
            _errorCallback = errorCallback;
        }

        /// <inheritdoc/>
        public string RULE_NAME => "require-begin-end-for-while";

        /// <inheritdoc/>
        public string RULE_TEXT => "WHILE loop bodies must be wrapped in BEGIN/END to avoid accidental single-statement traps.";

        /// <inheritdoc/>
        public RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

        /// <inheritdoc/>
        public override void Visit(WhileStatement node)
        {
            if (node == null)
            {
                base.Visit(node);
                return;
            }

            if (IsMissingBeginEnd(node.Statement))
            {
                var location = node.Statement ?? (TSqlFragment)node;
                _errorCallback?.Invoke(RULE_NAME, RULE_TEXT, location.StartLine, location.StartColumn);
            }

            base.Visit(node);
        }

        /// <inheritdoc/>
        public void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
        {
            // No automatic fix is provided for this rule.
        }

        private static bool IsMissingBeginEnd(TSqlStatement? statement)
        {
            return statement != null && statement is not BeginEndBlockStatement;
        }
    }
}
