using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin
{
    /// <summary>
    /// Flags slash-delimited date literals that depend on session dateformat settings.
    /// </summary>
    public sealed class AvoidAmbiguousDatetimeLiteralRule : SqlLintRuleBase
    {
        /// <summary>
        /// Configures the rule with the provided error callback.
        /// </summary>
        /// <param name="errorCallback">Callback invoked when a violation is detected.</param>
        public AvoidAmbiguousDatetimeLiteralRule(Action<string, string, int, int> errorCallback) : base(errorCallback)
        {
        }
        /// <summary>
        /// Rule identifier.
        /// </summary>
        public override string RULE_NAME => "avoid-ambiguous-datetime-literal";
        /// <summary>
        /// Warns when slash-delimited date literals are used instead of ISO formats.
        /// </summary>
        public override string RULE_TEXT => "Date literals with slash delimiters (/) are ambiguous and depend on language settings. Use ISO 8601 format (YYYY-MM-DD) or date construction functions instead.";
        /// <summary>
        /// Severity level for this rule.
        /// </summary>
        public override RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;
        /// <summary>
        /// Inspects string literals for slash-delimited date patterns.
        /// </summary>
        /// <param name="node">String literal to examine.</param>
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
        /// Determines whether the literal looks like a slash-delimited date.
        /// </summary>
        /// <param name="value">Literal text without surrounding quotes.</param>
        /// <returns>True if the literal matches a slash-delimited date pattern.</returns>
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
        /// Auto-fix is not implemented for this rule.
        /// </summary>
        /// <param name="fileLines">Source file lines available for modifications.</param>
        /// <param name="ruleViolation">Violation details to fix.</param>
        /// <param name="actions">Helper that applies line-level edits.</param>
        public override void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
        {
            // 邵ｺ阮吶・郢晢ｽｫ郢晢ｽｼ郢晢ｽｫ邵ｺ・ｧ邵ｺ・ｯ髢ｾ・ｪ陷咲ｩゑｽｿ・ｮ雎・ｽ｣郢ｧ蜻育ｽｲ關灘ｸ呻ｼ邵ｺ・ｾ邵ｺ蟶呻ｽ鍋ｸｲ繝ｻ        }
        }
    }


}

