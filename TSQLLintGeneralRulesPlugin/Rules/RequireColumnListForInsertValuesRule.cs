using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin
{
    /// <summary>
    /// Requires the specification of a column list in <c>INSERT INTO ... VALUES</c> statements.
    /// </summary>
    public sealed class RequireColumnListForInsertValuesRule : TSqlFragmentVisitor, ISqlLintRule
    {
        private readonly Action<string, string, int, int> _errorCallback;

        /// <summary>
        /// Initializes the rule.
        /// </summary>
        /// <param name="errorCallback">Callback invoked when a violation is detected.</param>
        public RequireColumnListForInsertValuesRule(Action<string, string, int, int> errorCallback)
        {
            _errorCallback = errorCallback;
        }

        /// <summary>
        /// Gets the rule ID.
        /// </summary>
        public string RULE_NAME => "require-column-list-for-insert-values";

        /// <summary>
        /// Gets the violation message.
        /// </summary>
        public string RULE_TEXT => "INSERT INTO ... VALUES requires a column list.";

        /// <summary>
        /// Gets the violation severity.
        /// </summary>
        public RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

        /// <summary>
        /// Traverses INSERT statements and reports violations when VALUES is used without a column list.
        /// </summary>
        /// <param name="node">The INSERT statement node to visit.</param>
        public override void Visit(InsertStatement node)
        {
            var specification = node?.InsertSpecification;
            if (specification == null)
            {
                base.Visit(node);
                return;
            }

            if (specification.InsertSource is not ValuesInsertSource)
            {
                base.Visit(node);
                return;
            }

            if (specification.Columns != null && specification.Columns.Count > 0)
            {
                base.Visit(node);
                return;
            }

            var target = specification.Target;
            var line = target?.StartLine ?? node.StartLine;
            var column = target?.StartColumn ?? node.StartColumn;
            _errorCallback?.Invoke(RULE_NAME, RULE_TEXT, line, column);

            base.Visit(node);
        }

        /// <summary>
        /// Automatically fixes rule violations (no automatic fix is provided for this rule).
        /// </summary>
        /// <param name="fileLines">Array of lines in the file.</param>
        /// <param name="ruleViolation">The rule violation information.</param>
        /// <param name="actions">Line edit actions.</param>
        public void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
        {
            // No automatic fix is provided for this rule.
        }
    }
}
