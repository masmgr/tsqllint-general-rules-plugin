using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin
{
    /// <summary>
    /// Flags NOLOCK/READ UNCOMMITTED hints because they allow dirty reads.
    /// </summary>
    public sealed class AvoidNolockRule : SqlLintRuleBase
    {
        /// <summary>
        /// Configures the rule with the provided error callback.
        /// </summary>
        /// <param name="errorCallback">Callback invoked when a violation is detected.</param>
        public AvoidNolockRule(Action<string, string, int, int> errorCallback) : base(errorCallback)
        {
        }
        /// <summary>
        /// Rule identifier.
        /// </summary>
        public override string RULE_NAME => "avoid-nolock";
        /// <summary>
        /// Warns about table hints that request relaxed isolation.
        /// </summary>
        public override string RULE_TEXT => "NOLOCK and READ UNCOMMITTED allow dirty reads which can return incorrect data. Use appropriate isolation levels or snapshot isolation instead.";
        /// <summary>
        /// Severity level for this rule.
        /// </summary>
        public override RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;
        /// <summary>
        /// Detects table hints that specify NOLOCK or READ UNCOMMITTED.
        /// </summary>
        /// <param name="node">Table hint to examine.</param>
        public override void Visit(TableHint node)
        {
            if (node == null)
            {
                base.Visit(node);
                return;
            }

            if (node.HintKind == TableHintKind.NoLock ||
                node.HintKind == TableHintKind.ReadUncommitted)
            {
                ReportViolation(node.StartLine, node.StartColumn);
            }

            base.Visit(node);
        }
        /// <summary>
        /// Reports SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED statements.
        /// </summary>
        /// <param name="node">Isolation level statement to inspect.</param>
        public override void Visit(SetTransactionIsolationLevelStatement node)
        {
            if (node == null)
            {
                base.Visit(node);
                return;
            }

            if (node.Level == IsolationLevel.ReadUncommitted)
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

