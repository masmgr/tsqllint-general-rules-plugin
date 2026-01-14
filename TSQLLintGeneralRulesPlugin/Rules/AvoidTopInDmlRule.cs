using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin
{
    /// <summary>
    /// Detects TOP filters in UPDATE or DELETE statements without ORDER BY that can be non-deterministic.
    /// </summary>
    public sealed class AvoidTopInDmlRule : SqlLintRuleBase
    {
        /// <summary>
        /// Configures the rule with the provided error callback.
        /// </summary>
        /// <param name="errorCallback">Callback invoked when a violation is detected.</param>
        public AvoidTopInDmlRule(Action<string, string, int, int> errorCallback) : base(errorCallback)
        {
        }
        /// <summary>
        /// Rule identifier.
        /// </summary>
        public override string RULE_NAME => "avoid-top-in-dml";
        /// <summary>
        /// Warns about TOP usage in UPDATE/DELETE without deterministic ordering.
        /// </summary>
        public override string RULE_TEXT => "TOP in UPDATE/DELETE statements without ORDER BY can produce non-deterministic results. Consider using a CTE with ORDER BY or specific WHERE clause.";
        /// <summary>
        /// Severity level for this rule.
        /// </summary>
        public override RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;
        /// <summary>
        /// Reports UPDATE statements that include a TOP clause.
        /// </summary>
        /// <param name="node">UPDATE statement to inspect.</param>
        public override void Visit(UpdateStatement node)
        {
            if (node?.UpdateSpecification?.TopRowFilter != null)
            {
                var top = node.UpdateSpecification.TopRowFilter;
                ReportViolation(top.StartLine, top.StartColumn);
            }

            base.Visit(node);
        }
        /// <summary>
        /// Reports DELETE statements that include a TOP clause.
        /// </summary>
        /// <param name="node">DELETE statement to inspect.</param>
        public override void Visit(DeleteStatement node)
        {
            if (node?.DeleteSpecification?.TopRowFilter != null)
            {
                var top = node.DeleteSpecification.TopRowFilter;
                ReportViolation(top.StartLine, top.StartColumn);
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

