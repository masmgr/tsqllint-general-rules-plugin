using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLint.Plugin
{
    /// <summary>
    /// UPDATE/DELETE ステートメントで TOP を避けるべきことを検出するルール。
    /// </summary>
    public sealed class AvoidTopInDmlRule : TSqlFragmentVisitor, ISqlLintRule
    {
        private readonly Action<string, string, int, int> _errorCallback;

        /// <summary>
        /// ルールを初期化します。
        /// </summary>
        /// <param name="errorCallback">違反が検出されたときに呼び出されるコールバック。</param>
        public AvoidTopInDmlRule(Action<string, string, int, int> errorCallback)
        {
            _errorCallback = errorCallback;
        }

        /// <summary>
        /// ルールIDを取得します。
        /// </summary>
        public string RULE_NAME => "avoid-top-in-dml";

        /// <summary>
        /// 違反メッセージを取得します。
        /// </summary>
        public string RULE_TEXT => "TOP in UPDATE/DELETE statements without ORDER BY can produce non-deterministic results. Consider using a CTE with ORDER BY or specific WHERE clause.";

        /// <summary>
        /// 違反の重大度を取得します。
        /// </summary>
        public RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

        /// <summary>
        /// UPDATE ステートメントを訪問します。
        /// </summary>
        /// <param name="node">訪問するノード。</param>
        public override void Visit(UpdateStatement node)
        {
            if (node?.UpdateSpecification?.TopRowFilter != null)
            {
                var top = node.UpdateSpecification.TopRowFilter;
                _errorCallback?.Invoke(RULE_NAME, RULE_TEXT, top.StartLine, top.StartColumn);
            }

            base.Visit(node);
        }

        /// <summary>
        /// DELETE ステートメントを訪問します。
        /// </summary>
        /// <param name="node">訪問するノード。</param>
        public override void Visit(DeleteStatement node)
        {
            if (node?.DeleteSpecification?.TopRowFilter != null)
            {
                var top = node.DeleteSpecification.TopRowFilter;
                _errorCallback?.Invoke(RULE_NAME, RULE_TEXT, top.StartLine, top.StartColumn);
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
