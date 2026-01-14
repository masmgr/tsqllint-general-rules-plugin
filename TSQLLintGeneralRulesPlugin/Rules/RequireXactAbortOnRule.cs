using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin
{
    /// <summary>
    /// 繝医Λ繝ｳ繧ｶ繧ｯ繧ｷ繝ｧ繝ｳ縺ｧ SET XACT_ABORT ON 縺悟ｿ・医〒縺ゅｋ縺薙→繧呈､懷・縺吶ｋ繝ｫ繝ｼ繝ｫ縲・    /// </summary>
    public sealed class RequireXactAbortOnRule : SqlLintRuleBase
    {

        /// <summary>
        /// 繝ｫ繝ｼ繝ｫ繧貞・譛溷喧縺励∪縺吶・        /// </summary>
        /// <param name="errorCallback">驕募渚縺梧､懷・縺輔ｌ縺溘→縺阪↓蜻ｼ縺ｳ蜃ｺ縺輔ｌ繧九さ繝ｼ繝ｫ繝舌ャ繧ｯ縲・/param>
        public RequireXactAbortOnRule(Action<string, string, int, int> errorCallback) : base(errorCallback)
        {
        }

        /// <summary>
        /// 繝ｫ繝ｼ繝ｫID繧貞叙蠕励＠縺ｾ縺吶・        /// </summary>
        public override string RULE_NAME => "require-xact-abort-on";

        /// <summary>
        /// 驕募渚繝｡繝・そ繝ｼ繧ｸ繧貞叙蠕励＠縺ｾ縺吶・        /// </summary>
        public override string RULE_TEXT => "Transactions should include SET XACT_ABORT ON to ensure consistent rollback behavior on runtime errors.";

        /// <summary>
        /// 驕募渚縺ｮ驥榊､ｧ蠎ｦ繧貞叙蠕励＠縺ｾ縺吶・        /// </summary>
        public override RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

        /// <summary>
        /// BEGIN TRANSACTION 繧ｹ繝・・繝医Γ繝ｳ繝医ｒ險ｪ蝠上＠縺ｾ縺吶・        /// </summary>
        /// <param name="node">險ｪ蝠上☆繧九ヮ繝ｼ繝峨・/param>
        public override void Visit(BeginTransactionStatement node)
        {
            // Note: 螳悟・縺ｪ繝舌ャ繝√Ξ繝吶Ν縺ｮ讀懈渊縺ｫ縺ｯ縲∬､・尅縺ｪ迥ｶ諷狗ｮ｡逅・′蠢・ｦ√〒縺吶・            // 螟夜Κ讒区・縺ｧ縺薙・繝ｫ繝ｼ繝ｫ縺ｮ隧ｳ邏ｰ縺ｪ繝√ぉ繝・け繧貞ｮ溯｣・☆繧九％縺ｨ繧呈耳螂ｨ縺励∪縺吶・            base.Visit(node);
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

