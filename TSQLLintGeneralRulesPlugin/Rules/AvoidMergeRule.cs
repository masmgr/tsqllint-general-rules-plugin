using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin
{
    /// <summary>
    /// Flags MERGE statements because they are prone to bugs and complexity.
    /// </summary>
    public sealed class AvoidMergeRule : SqlLintRuleBase
    {
        /// <summary>
        /// Configures the rule with the provided error callback.
        /// </summary>
        /// <param name="errorCallback">Callback invoked when a violation is detected.</param>
        public AvoidMergeRule(Action<string, string, int, int> errorCallback) : base(errorCallback)
        {
        }
        /// <summary>
        /// Rule identifier.
        /// </summary>
        public override string RULE_NAME => "avoid-merge";
        /// <summary>
        /// Warns that MERGE is discouraged in favor of separate DML statements.
        /// </summary>
        public override string RULE_TEXT => "MERGE statements should be avoided due to known issues and complexity. Consider using separate INSERT, UPDATE, or DELETE statements.";
        /// <summary>
        /// Severity level for this rule.
        /// </summary>
        public override RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;
        /// <summary>
        /// Reports whenever a MERGE statement is encountered.
        /// </summary>
        /// <param name="node">MERGE statement to inspect.</param>
        public override void Visit(MergeStatement node)
        {
            if (node != null)
            {
                ReportViolation(node.StartLine, node.StartColumn);
            }

            base.Visit(node);
        }
        /// <summary>
        /// Auto-fix is not implemented for this rule.
        /// </summary>
        /// <param name="fileLines">Source file lines available for modifications.</param>
        /// <param name="ruleViolation">Violation details to fix.</param>
        /// <param name="actions">Helper that applies line-level edits.</param>
        public override void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
        {
            // 縺薙・繝ｫ繝ｼ繝ｫ縺ｧ縺ｯ閾ｪ蜍穂ｿｮ豁｣繧呈署萓帙＠縺ｾ縺帙ｓ縲・        }
        }
    }


}

