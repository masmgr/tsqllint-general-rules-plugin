using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin
{
    /// <summary>
    /// NOLOCK および READ UNCOMMITTED を避けるべきことを検出するルール。
    /// </summary>
    public sealed class AvoidNolockRule : TSqlFragmentVisitor, ISqlLintRule
    {
        private readonly Action<string, string, int, int> _errorCallback;

        /// <summary>
        /// ルールを初期化します。
        /// </summary>
        /// <param name="errorCallback">違反が検出されたときに呼び出されるコールバック。</param>
        public AvoidNolockRule(Action<string, string, int, int> errorCallback)
        {
            _errorCallback = errorCallback;
        }

        /// <summary>
        /// ルールIDを取得します。
        /// </summary>
        public string RULE_NAME => "avoid-nolock";

        /// <summary>
        /// 違反メッセージを取得します。
        /// </summary>
        public string RULE_TEXT => "NOLOCK and READ UNCOMMITTED allow dirty reads which can return incorrect data. Use appropriate isolation levels or snapshot isolation instead.";

        /// <summary>
        /// 違反の重大度を取得します。
        /// </summary>
        public RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

        /// <summary>
        /// テーブルヒントを訪問します。
        /// </summary>
        /// <param name="node">訪問するノード。</param>
        public override void Visit(TableHint node)
        {
            if (node == null)
            {
                base.Visit(node);
                return;
            }

            if (node.HintKind == TableHintKind.NoLock ||
                node.HintKind == TableHintKind.ReadUncommitted)
            {
                _errorCallback?.Invoke(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
            }

            base.Visit(node);
        }

        /// <summary>
        /// トランザクション分離レベルの SET ステートメントを訪問します。
        /// </summary>
        /// <param name="node">訪問するノード。</param>
        public override void Visit(SetTransactionIsolationLevelStatement node)
        {
            if (node == null)
            {
                base.Visit(node);
                return;
            }

            if (node.Level == IsolationLevel.ReadUncommitted)
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
