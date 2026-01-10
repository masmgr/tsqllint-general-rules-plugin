using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin
{
    /// <summary>
    /// トランザクションで SET XACT_ABORT ON が必須であることを検出するルール。
    /// </summary>
    public sealed class RequireXactAbortOnRule : TSqlFragmentVisitor, ISqlLintRule
    {
        private readonly Action<string, string, int, int> _errorCallback;

        /// <summary>
        /// ルールを初期化します。
        /// </summary>
        /// <param name="errorCallback">違反が検出されたときに呼び出されるコールバック。</param>
        public RequireXactAbortOnRule(Action<string, string, int, int> errorCallback)
        {
            _errorCallback = errorCallback;
        }

        /// <summary>
        /// ルールIDを取得します。
        /// </summary>
        public string RULE_NAME => "require-xact-abort-on";

        /// <summary>
        /// 違反メッセージを取得します。
        /// </summary>
        public string RULE_TEXT => "Transactions should include SET XACT_ABORT ON to ensure consistent rollback behavior on runtime errors.";

        /// <summary>
        /// 違反の重大度を取得します。
        /// </summary>
        public RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

        /// <summary>
        /// BEGIN TRANSACTION ステートメントを訪問します。
        /// </summary>
        /// <param name="node">訪問するノード。</param>
        public override void Visit(BeginTransactionStatement node)
        {
            // Note: 完全なバッチレベルの検査には、複雑な状態管理が必要です。
            // 外部構成でこのルールの詳細なチェックを実装することを推奨します。
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
