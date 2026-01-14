using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin
{
    /// <summary>
    /// 繝医Λ繝ｳ繧ｶ繧ｯ繧ｷ繝ｧ繝ｳ縺・TRY/CATCH 縺ｧ繝ｩ繝・・縺輔ｌ縺ｦ縺・ｋ縺九ｒ遒ｺ隱阪☆繧九Ν繝ｼ繝ｫ縲・    /// </summary>
    public sealed class RequireTryCatchForTransactionRule : SqlLintRuleBase
    {
        private bool _isInsideTryCatch = false;

        /// <summary>
        /// 繝ｫ繝ｼ繝ｫ繧貞・譛溷喧縺励∪縺吶・        /// </summary>
        /// <param name="errorCallback">驕募渚縺梧､懷・縺輔ｌ縺溘→縺阪↓蜻ｼ縺ｳ蜃ｺ縺輔ｌ繧九さ繝ｼ繝ｫ繝舌ャ繧ｯ縲・/param>
        public RequireTryCatchForTransactionRule(Action<string, string, int, int> errorCallback) : base(errorCallback)
        {
        }

        /// <summary>
        /// 繝ｫ繝ｼ繝ｫID繧貞叙蠕励＠縺ｾ縺吶・        /// </summary>
        public override string RULE_NAME => "require-try-catch-for-transaction";

        /// <summary>
        /// 驕募渚繝｡繝・そ繝ｼ繧ｸ繧貞叙蠕励＠縺ｾ縺吶・        /// </summary>
        public override string RULE_TEXT => "BEGIN TRANSACTION should be wrapped in TRY/CATCH to ensure proper error handling and rollback.";

        /// <summary>
        /// 驕募渚縺ｮ驥榊､ｧ蠎ｦ繧貞叙蠕励＠縺ｾ縺吶・        /// </summary>
        public override RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

        /// <summary>
        /// TRY/CATCH 繧ｹ繝・・繝医Γ繝ｳ繝医ｒ險ｪ蝠上＠縺ｾ縺吶・        /// </summary>
        /// <param name="node">險ｪ蝠上☆繧九ヮ繝ｼ繝峨・/param>
        public override void Visit(TryCatchStatement node)
        {
            var previousState = _isInsideTryCatch;
            _isInsideTryCatch = true;
            base.Visit(node);
            _isInsideTryCatch = previousState;
        }

        /// <summary>
        /// BEGIN TRANSACTION 繧ｹ繝・・繝医Γ繝ｳ繝医ｒ險ｪ蝠上＠縺ｾ縺吶・        /// </summary>
        /// <param name="node">險ｪ蝠上☆繧九ヮ繝ｼ繝峨・/param>
        public override void Visit(BeginTransactionStatement node)
        {
            if (node != null && !_isInsideTryCatch)
            {
                ReportViolation(node.StartLine, node.StartColumn);
            }

            base.Visit(node);
        }

        /// <summary>
        /// 繝ｫ繝ｼ繝ｫ驕募渚繧定・蜍穂ｿｮ豁｣縺励∪縺吶ゅ％縺ｮ繝ｫ繝ｼ繝ｫ縺ｧ縺ｯ閾ｪ蜍穂ｿｮ豁｣繧呈署萓帙＠縺ｾ縺帙ｓ縲・        /// </summary>
        /// <param name="fileLines">繝輔ぃ繧､繝ｫ縺ｮ陦後・驟榊・縲・/param>
        /// <param name="ruleViolation">繝ｫ繝ｼ繝ｫ驕募渚諠・ｱ縲・/param>
        /// <param name="actions">陦檎ｷｨ髮・い繧ｯ繧ｷ繝ｧ繝ｳ縲・/param>
        public override void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
        {
            // 縺薙・繝ｫ繝ｼ繝ｫ縺ｧ縺ｯ閾ｪ蜍穂ｿｮ豁｣繧呈署萓帙＠縺ｾ縺帙ｓ縲・        }
        }
    }


}

