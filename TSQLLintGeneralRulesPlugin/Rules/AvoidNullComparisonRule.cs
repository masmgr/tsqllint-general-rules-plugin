using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin
{
    /// <summary>
    /// NULL 縺ｨ縺ｮ豈碑ｼ・(= NULL, <> NULL) 繧帝∩縺代ｋ縺ｹ縺阪％縺ｨ繧呈､懷・縺吶ｋ繝ｫ繝ｼ繝ｫ縲・    /// </summary>
    public sealed class AvoidNullComparisonRule : SqlLintRuleBase
    {

        /// <summary>
        /// 繝ｫ繝ｼ繝ｫ繧貞・譛溷喧縺励∪縺吶・        /// </summary>
        /// <param name="errorCallback">驕募渚縺梧､懷・縺輔ｌ縺溘→縺阪↓蜻ｼ縺ｳ蜃ｺ縺輔ｌ繧九さ繝ｼ繝ｫ繝舌ャ繧ｯ縲・/param>
        public AvoidNullComparisonRule(Action<string, string, int, int> errorCallback) : base(errorCallback)
        {
        }

        /// <summary>
        /// 繝ｫ繝ｼ繝ｫID繧貞叙蠕励＠縺ｾ縺吶・        /// </summary>
        public override string RULE_NAME => "avoid-null-comparison";

        /// <summary>
        /// 驕募渚繝｡繝・そ繝ｼ繧ｸ繧貞叙蠕励＠縺ｾ縺吶・        /// </summary>
        public override string RULE_TEXT => "Comparing with NULL using = or <> always evaluates to UNKNOWN. Use IS NULL or IS NOT NULL instead.";

        /// <summary>
        /// 驕募渚縺ｮ驥榊､ｧ蠎ｦ繧貞叙蠕励＠縺ｾ縺吶・        /// </summary>
        public override RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

        /// <summary>
        /// 繝悶・繝ｫ豈碑ｼ・ｼ上ｒ險ｪ蝠上＠縺ｾ縺吶・        /// </summary>
        /// <param name="node">險ｪ蝠上☆繧九ヮ繝ｼ繝峨・/param>
        public override void Visit(BooleanComparisonExpression node)
        {
            if (node == null)
            {
                base.Visit(node);
                return;
            }

            if (node.ComparisonType == BooleanComparisonType.Equals ||
                node.ComparisonType == BooleanComparisonType.NotEqualToBrackets ||
                node.ComparisonType == BooleanComparisonType.NotEqualToExclamation)
            {
                if (IsNullLiteral(node.FirstExpression) || IsNullLiteral(node.SecondExpression))
                {
                    ReportViolation(node.StartLine, node.StartColumn);
                }
            }

            base.Visit(node);
        }

        /// <summary>
        /// 蠑上′ NULL 繝ｪ繝・Λ繝ｫ縺九←縺・°繧堤｢ｺ隱阪＠縺ｾ縺吶・        /// </summary>
        /// <param name="expression">遒ｺ隱阪☆繧句ｼ上・/param>
        /// <returns>蠑上′ NULL 繝ｪ繝・Λ繝ｫ縺ｮ蝣ｴ蜷医・ true縲・/returns>
        private static bool IsNullLiteral(ScalarExpression? expression)
        {
            return expression is NullLiteral;
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

