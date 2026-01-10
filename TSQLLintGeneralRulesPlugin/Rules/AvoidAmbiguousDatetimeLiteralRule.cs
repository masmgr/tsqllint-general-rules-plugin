using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLint.Plugin
{
    /// <summary>
    /// スラッシュで区切られた曖昧な日付リテラルを避けるべきことを検出するルール。
    /// </summary>
    public sealed class AvoidAmbiguousDatetimeLiteralRule : TSqlFragmentVisitor, ISqlLintRule
    {
        private readonly Action<string, string, int, int> _errorCallback;

        /// <summary>
        /// ルールを初期化します。
        /// </summary>
        /// <param name="errorCallback">違反が検出されたときに呼び出されるコールバック。</param>
        public AvoidAmbiguousDatetimeLiteralRule(Action<string, string, int, int> errorCallback)
        {
            _errorCallback = errorCallback;
        }

        /// <summary>
        /// ルールIDを取得します。
        /// </summary>
        public string RULE_NAME => "avoid-ambiguous-datetime-literal";

        /// <summary>
        /// 違反メッセージを取得します。
        /// </summary>
        public string RULE_TEXT => "Date literals with slash delimiters (/) are ambiguous and depend on language settings. Use ISO 8601 format (YYYY-MM-DD) or date construction functions instead.";

        /// <summary>
        /// 違反の重大度を取得します。
        /// </summary>
        public RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

        /// <summary>
        /// 文字列リテラルを訪問します。
        /// </summary>
        /// <param name="node">訪問するノード。</param>
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
                _errorCallback?.Invoke(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
            }

            base.Visit(node);
        }

        /// <summary>
        /// 文字列がスラッシュで区切られた日付パターンを含むかどうかを確認します。
        /// </summary>
        /// <param name="value">確認する文字列値。</param>
        /// <returns>スラッシュで区切られた日付パターンが含まれている場合は true。</returns>
        private static bool ContainsSlashDelimitedDate(string value)
        {
            if (!value.Contains('/'))
            {
                return false;
            }

            // スラッシュで区切られた日付パターンをマッチング: YYYY/MM/DD, MM/DD/YYYY など
            // パターン: 1-4 桁 / 1-2 桁 / 1-4 桁
            var datePattern = @"^\d{1,4}/\d{1,2}/\d{1,4}";
            return Regex.IsMatch(value, datePattern);
        }

        /// <summary>
        /// ルール違反を自動修正します。このルールでは自動修正を提供しません。
        /// </summary>
        /// <param name="fileLines">ファイルの行の配列。</param>
        /// <param name="ruleViolation">ルール違反情報。</param>
        /// <param name="actions">行編集アクション。</param>
        public void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
        {
            // このルールでは自動修正を提供しません。
        }
    }
}
