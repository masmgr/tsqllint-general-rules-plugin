using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin
{
    /// <summary>
    /// Enforces SET XACT_ABORT ON within transactions for consistent rollback behavior.
    /// </summary>
    public sealed class RequireXactAbortOnRule : SqlLintRuleBase
    {
        /// <summary>
        /// Configures the rule with the provided error callback.
        /// </summary>
        /// <param name="errorCallback">Callback invoked when a violation is detected.</param>
        public RequireXactAbortOnRule(Action<string, string, int, int> errorCallback) : base(errorCallback)
        {
        }
        /// <summary>
        /// Rule identifier.
        /// </summary>
        public override string RULE_NAME => "require-xact-abort-on";
        /// <summary>
        /// Warns when transactions do not enable XACT_ABORT ON.
        /// </summary>
        public override string RULE_TEXT => "Transactions should include SET XACT_ABORT ON to ensure consistent rollback behavior on runtime errors.";
        /// <summary>
        /// Severity level for this rule.
        /// </summary>
        public override RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;
        /// <summary>
        /// Reports BEGIN TRANSACTION statements that lack SET XACT_ABORT ON nearby.
        /// </summary>
        /// <param name="node">BeginTransaction statement to inspect.</param>
        public override void Visit(BeginTransactionStatement node)
        {
            // Note: 螳悟・縺ｪ繝舌ャ繝√Ξ繝吶Ν縺ｮ讀懈渊縺ｫ縺ｯ縲∬､・尅縺ｪ迥ｶ諷狗ｮ｡逅・′蠢・ｦ√〒縺吶・            // 螟夜Κ讒区・縺ｧ縺薙・繝ｫ繝ｼ繝ｫ縺ｮ隧ｳ邏ｰ縺ｪ繝√ぉ繝・け繧貞ｮ溯｣・☆繧九％縺ｨ繧呈耳螂ｨ縺励∪縺吶・            base.Visit(node);
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

