using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin
{
    /// <summary>
    /// 郢ｧ・ｹ郢晢ｽｩ郢昴・縺咏ｹ晢ｽ･邵ｺ・ｧ陋ｹ・ｺ陋ｻ繝ｻ・臥ｹｧ蠕娯螺隴門戟荵らｸｺ・ｪ隴鯉ｽ･闔牙･ﾎ懃ｹ昴・ﾎ帷ｹ晢ｽｫ郢ｧ蟶昶茜邵ｺ莉｣・狗ｸｺ・ｹ邵ｺ髦ｪ・・ｸｺ・ｨ郢ｧ蜻茨ｽ､諛ｷ繝ｻ邵ｺ蜷ｶ・狗ｹ晢ｽｫ郢晢ｽｼ郢晢ｽｫ邵ｲ繝ｻ    /// </summary>
    public sealed class AvoidAmbiguousDatetimeLiteralRule : SqlLintRuleBase
    {

        /// <summary>
        /// 郢晢ｽｫ郢晢ｽｼ郢晢ｽｫ郢ｧ雋槭・隴帶ｺｷ蝟ｧ邵ｺ蜉ｱ竏ｪ邵ｺ蜷ｶﾂ繝ｻ        /// </summary>
        /// <param name="errorCallback">鬩募供貂夂ｸｺ譴ｧ・､諛ｷ繝ｻ邵ｺ霈費ｽ檎ｸｺ貅倪・邵ｺ髦ｪ竊楢惱・ｼ邵ｺ・ｳ陷・ｽｺ邵ｺ霈費ｽ檎ｹｧ荵昴＆郢晢ｽｼ郢晢ｽｫ郢晁・繝｣郢ｧ・ｯ邵ｲ繝ｻ/param>
        public AvoidAmbiguousDatetimeLiteralRule(Action<string, string, int, int> errorCallback) : base(errorCallback)
        {
        }

        /// <summary>
        /// 郢晢ｽｫ郢晢ｽｼ郢晢ｽｫID郢ｧ雋槫徐陟募干・邵ｺ・ｾ邵ｺ蜷ｶﾂ繝ｻ        /// </summary>
        public override string RULE_NAME => "avoid-ambiguous-datetime-literal";

        /// <summary>
        /// 鬩募供貂夂ｹ晢ｽ｡郢昴・縺晉ｹ晢ｽｼ郢ｧ・ｸ郢ｧ雋槫徐陟募干・邵ｺ・ｾ邵ｺ蜷ｶﾂ繝ｻ        /// </summary>
        public override string RULE_TEXT => "Date literals with slash delimiters (/) are ambiguous and depend on language settings. Use ISO 8601 format (YYYY-MM-DD) or date construction functions instead.";

        /// <summary>
        /// 鬩募供貂夂ｸｺ・ｮ鬩･讎奇ｽ､・ｧ陟趣ｽｦ郢ｧ雋槫徐陟募干・邵ｺ・ｾ邵ｺ蜷ｶﾂ繝ｻ        /// </summary>
        public override RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

        /// <summary>
        /// 隴√・・ｭ諤懊・郢晢ｽｪ郢昴・ﾎ帷ｹ晢ｽｫ郢ｧ螳夲ｽｨ・ｪ陜荳奇ｼ邵ｺ・ｾ邵ｺ蜷ｶﾂ繝ｻ        /// </summary>
        /// <param name="node">髫ｪ・ｪ陜荳岩・郢ｧ荵昴Π郢晢ｽｼ郢晏ｳｨﾂ繝ｻ/param>
        public override void Visit(StringLiteral node)
        {
            if (node == null || string.IsNullOrWhiteSpace(node.Value))
            {
                base.Visit(node);
                return;
            }

            var value = node.Value.Trim('\'', '"');

            if (ContainsSlashDelimitedDate(value))
            {
                ReportViolation(node.StartLine, node.StartColumn);
            }

            base.Visit(node);
        }

        /// <summary>
        /// 隴√・・ｭ諤懊・邵ｺ蠕後○郢晢ｽｩ郢昴・縺咏ｹ晢ｽ･邵ｺ・ｧ陋ｹ・ｺ陋ｻ繝ｻ・臥ｹｧ蠕娯螺隴鯉ｽ･闔牙･繝ｱ郢ｧ・ｿ郢晢ｽｼ郢晢ｽｳ郢ｧ雋樊ｧ郢ｧﾂ邵ｺ荵昶・邵ｺ繝ｻﾂｰ郢ｧ蝣､・｢・ｺ髫ｱ髦ｪ・邵ｺ・ｾ邵ｺ蜷ｶﾂ繝ｻ        /// </summary>
        /// <param name="value">驕抵ｽｺ髫ｱ髦ｪ笘・ｹｧ蛹ｺ譫夊氛諤懊・陋滂ｽ､邵ｲ繝ｻ/param>
        /// <returns>郢ｧ・ｹ郢晢ｽｩ郢昴・縺咏ｹ晢ｽ･邵ｺ・ｧ陋ｹ・ｺ陋ｻ繝ｻ・臥ｹｧ蠕娯螺隴鯉ｽ･闔牙･繝ｱ郢ｧ・ｿ郢晢ｽｼ郢晢ｽｳ邵ｺ謔滓ｧ邵ｺ・ｾ郢ｧ蠕娯ｻ邵ｺ繝ｻ・玖撻・ｴ陷ｷ蛹ｻ繝ｻ true邵ｲ繝ｻ/returns>
        private static bool ContainsSlashDelimitedDate(string value)
        {
            if (!value.Contains('/'))
            {
                return false;
            }

            // 郢ｧ・ｹ郢晢ｽｩ郢昴・縺咏ｹ晢ｽ･邵ｺ・ｧ陋ｹ・ｺ陋ｻ繝ｻ・臥ｹｧ蠕娯螺隴鯉ｽ･闔牙･繝ｱ郢ｧ・ｿ郢晢ｽｼ郢晢ｽｳ郢ｧ蛛ｵ繝ｻ郢昴・繝｡郢晢ｽｳ郢ｧ・ｰ: YYYY/MM/DD, MM/DD/YYYY 邵ｺ・ｪ邵ｺ・ｩ
            // 郢昜ｻ｣縺｡郢晢ｽｼ郢晢ｽｳ: 1-4 隴ｯ繝ｻ/ 1-2 隴ｯ繝ｻ/ 1-4 隴ｯ繝ｻ
            var datePattern = @"^\d{1,4}/\d{1,2}/\d{1,4}";
            return Regex.IsMatch(value, datePattern);
        }

        /// <summary>
        /// 郢晢ｽｫ郢晢ｽｼ郢晢ｽｫ鬩募供貂夂ｹｧ螳壹・陷咲ｩゑｽｿ・ｮ雎・ｽ｣邵ｺ蜉ｱ竏ｪ邵ｺ蜷ｶﾂ繧・ｼ・ｸｺ・ｮ郢晢ｽｫ郢晢ｽｼ郢晢ｽｫ邵ｺ・ｧ邵ｺ・ｯ髢ｾ・ｪ陷咲ｩゑｽｿ・ｮ雎・ｽ｣郢ｧ蜻育ｽｲ關灘ｸ呻ｼ邵ｺ・ｾ邵ｺ蟶呻ｽ鍋ｸｲ繝ｻ        /// </summary>
        /// <param name="fileLines">郢晁ｼ斐＜郢ｧ・､郢晢ｽｫ邵ｺ・ｮ髯ｦ蠕後・鬩滓ｦ翫・邵ｲ繝ｻ/param>
        /// <param name="ruleViolation">郢晢ｽｫ郢晢ｽｼ郢晢ｽｫ鬩募供貂夊ｫ繝ｻ・ｰ・ｱ邵ｲ繝ｻ/param>
        /// <param name="actions">髯ｦ讙趣ｽｷ・ｨ鬮ｮ繝ｻ縺・ｹｧ・ｯ郢ｧ・ｷ郢晢ｽｧ郢晢ｽｳ邵ｲ繝ｻ/param>
        public override void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
        {
            // 邵ｺ阮吶・郢晢ｽｫ郢晢ｽｼ郢晢ｽｫ邵ｺ・ｧ邵ｺ・ｯ髢ｾ・ｪ陷咲ｩゑｽｿ・ｮ雎・ｽ｣郢ｧ蜻育ｽｲ關灘ｸ呻ｼ邵ｺ・ｾ邵ｺ蟶呻ｽ鍋ｸｲ繝ｻ        }
        }
    }


}

