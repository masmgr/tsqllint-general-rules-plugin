using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin
{
    /// <summary>
    /// Detects comparisons that use = or <> with NULL, which evaluate to UNKNOWN.
    /// </summary>
    public sealed class AvoidNullComparisonRule : SqlLintRuleBase
    {
        /// <summary>
        /// Configures the rule with the provided error callback.
        /// </summary>
        /// <param name="errorCallback">Callback invoked when a violation is detected.</param>
        public AvoidNullComparisonRule(Action<string, string, int, int> errorCallback) : base(errorCallback)
        {
        }
        /// <summary>
        /// Rule identifier.
        /// </summary>
        public override string RULE_NAME => "avoid-null-comparison";
        /// <summary>
        /// Warns against equality comparisons with NULL because they return UNKNOWN.
        /// </summary>
        public override string RULE_TEXT => "Comparing with NULL using = or <> always evaluates to UNKNOWN. Use IS NULL or IS NOT NULL instead.";
        /// <summary>
        /// Severity level for this rule.
        /// </summary>
        public override RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;
        /// <summary>
        /// Identifies comparisons that include NULL literals.
        /// </summary>
        /// <param name="node">Boolean comparison expression to inspect.</param>
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
        /// Determines whether the provided expression is a NULL literal.
        /// </summary>
        /// <param name="expression">Scalar expression to check.</param>
        /// <returns>True when the expression is a NullLiteral instance.</returns>
        private static bool IsNullLiteral(ScalarExpression? expression)
        {
            return expression is NullLiteral;
        }
        /// <summary>
        /// Auto-fix is not implemented for this rule.
        /// </summary>
        /// <param name="fileLines">Source file lines available for modifications.</param>
        /// <param name="ruleViolation">Violation details to fix.</param>
        /// <param name="actions">Helper that applies line-level edits.</param>
        public override void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
        {
            // 縺薙・繝ｫ繝ｼ繝ｫ縺ｧ縺ｯ閾ｪ蜍穂ｿｮ豁｣繧呈署萓帙＠縺ｾ縺帙ｓ縲・        }
        }
    }


}

