using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin
{
    /// <summary>
    /// Ensures WHILE loops always wrap their body in BEGIN/END.
    /// </summary>
    public sealed class RequireBeginEndForWhileRule : SqlLintRuleBase
    {

        /// <summary>
        /// Initializes the rule.
        /// </summary>
        /// <param name="errorCallback">Callback invoked for violations.</param>
        public RequireBeginEndForWhileRule(Action<string, string, int, int> errorCallback) : base(errorCallback)
        {
        }

        /// <inheritdoc/>
        public override string RULE_NAME => "require-begin-end-for-while";

        /// <inheritdoc/>
        public override string RULE_TEXT => "WHILE loop bodies must be wrapped in BEGIN/END to avoid accidental single-statement traps.";

        /// <inheritdoc/>
        public override RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

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
                var line = node.StartLine > 0 ? node.StartLine : node.Statement?.StartLine ?? 0;
                var column = node.StartColumn > 0 ? node.StartColumn : node.Statement?.StartColumn ?? 0;
                ReportViolation(line, column);
            }

            base.Visit(node);
        }

        /// <inheritdoc/>
        public override void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
        {
            // No automatic fix is provided for this rule.
        }

        private static bool IsMissingBeginEnd(TSqlStatement? statement)
        {
            return statement != null && statement is not BeginEndBlockStatement;
        }
    }
}


