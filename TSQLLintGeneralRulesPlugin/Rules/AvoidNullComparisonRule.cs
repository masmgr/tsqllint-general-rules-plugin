using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin
{
    /// <summary>
    /// NULL との比較 (= NULL, <> NULL) を避けるべきことを検出するルール。
    /// </summary>
    public sealed class AvoidNullComparisonRule : TSqlFragmentVisitor, ISqlLintRule
    {
        private readonly Action<string, string, int, int> _errorCallback;

        /// <summary>
        /// ルールを初期化します。
        /// </summary>
        /// <param name="errorCallback">違反が検出されたときに呼び出されるコールバック。</param>
        public AvoidNullComparisonRule(Action<string, string, int, int> errorCallback)
        {
            _errorCallback = errorCallback;
        }

        /// <summary>
        /// ルールIDを取得します。
        /// </summary>
        public string RULE_NAME => "avoid-null-comparison";

        /// <summary>
        /// 違反メッセージを取得します。
        /// </summary>
        public string RULE_TEXT => "Comparing with NULL using = or <> always evaluates to UNKNOWN. Use IS NULL or IS NOT NULL instead.";

        /// <summary>
        /// 違反の重大度を取得します。
        /// </summary>
        public RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

        /// <summary>
        /// ブール比較式を訪問します。
        /// </summary>
        /// <param name="node">訪問するノード。</param>
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
                    _errorCallback?.Invoke(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
                }
            }

            base.Visit(node);
        }

        /// <summary>
        /// 式が NULL リテラルかどうかを確認します。
        /// </summary>
        /// <param name="expression">確認する式。</param>
        /// <returns>式が NULL リテラルの場合は true。</returns>
        private static bool IsNullLiteral(ScalarExpression? expression)
        {
            return expression is NullLiteral;
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
