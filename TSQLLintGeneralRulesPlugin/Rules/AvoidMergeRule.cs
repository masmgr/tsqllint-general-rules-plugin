using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin
{
    /// <summary>
    /// MERGE ステートメントを避けるべきことを検出するルール。
    /// </summary>
    public sealed class AvoidMergeRule : TSqlFragmentVisitor, ISqlLintRule
    {
        private readonly Action<string, string, int, int> _errorCallback;

        /// <summary>
        /// ルールを初期化します。
        /// </summary>
        /// <param name="errorCallback">違反が検出されたときに呼び出されるコールバック。</param>
        public AvoidMergeRule(Action<string, string, int, int> errorCallback)
        {
            _errorCallback = errorCallback;
        }

        /// <summary>
        /// ルールIDを取得します。
        /// </summary>
        public string RULE_NAME => "avoid-merge";

        /// <summary>
        /// 違反メッセージを取得します。
        /// </summary>
        public string RULE_TEXT => "MERGE statements should be avoided due to known issues and complexity. Consider using separate INSERT, UPDATE, or DELETE statements.";

        /// <summary>
        /// 違反の重大度を取得します。
        /// </summary>
        public RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

        /// <summary>
        /// MERGE ステートメントを訪問します。
        /// </summary>
        /// <param name="node">訪問するノード。</param>
        public override void Visit(MergeStatement node)
        {
            if (node != null)
            {
                _errorCallback?.Invoke(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
            }

            base.Visit(node);
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
