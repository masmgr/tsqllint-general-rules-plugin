using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin
{
    /// <summary>
    /// Ensures BEGIN TRANSACTION is enclosed in TRY/CATCH for proper rollback.
    /// </summary>
    public sealed class RequireTryCatchForTransactionRule : SqlLintRuleBase
    {
        private bool _isInsideTryCatch = false;
        /// <summary>
        /// Configures the rule with the provided error callback.
        /// </summary>
        /// <param name="errorCallback">Callback invoked when a violation is detected.</param>
        public RequireTryCatchForTransactionRule(Action<string, string, int, int> errorCallback) : base(errorCallback)
        {
        }
        /// <summary>
        /// Rule identifier.
        /// </summary>
        public override string RULE_NAME => "require-try-catch-for-transaction";
        /// <summary>
        /// Warns when BEGIN TRANSACTION is not protected by TRY/CATCH.
        /// </summary>
        public override string RULE_TEXT => "BEGIN TRANSACTION should be wrapped in TRY/CATCH to ensure proper error handling and rollback.";
        /// <summary>
        /// Severity level for this rule.
        /// </summary>
        public override RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;
        /// <summary>
        /// Tracks entry into TRY/CATCH blocks to avoid false positives.
        /// </summary>
        /// <param name="node">TRY/CATCH statement node.</param>
        public override void Visit(TryCatchStatement node)
        {
            var previousState = _isInsideTryCatch;
            _isInsideTryCatch = true;
            base.Visit(node);
            _isInsideTryCatch = previousState;
        }
        /// <summary>
        /// Reports BEGIN TRANSACTION statements that are not inside TRY/CATCH.
        /// </summary>
        /// <param name="node">BeginTransaction statement to inspect.</param>
        public override void Visit(BeginTransactionStatement node)
        {
            if (node != null && !_isInsideTryCatch)
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

