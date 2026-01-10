using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin
{
    /// <summary>
    /// Detects usage of <c>@@IDENTITY</c> and recommends using <c>SCOPE_IDENTITY()</c> or <c>OUTPUT</c> instead.
    /// </summary>
    public sealed class AvoidAtAtIdentityRule : TSqlFragmentVisitor, ISqlLintRule
    {
        private readonly Action<string, string, int, int> _errorCallback;

        /// <summary>
        /// Initializes the rule.
        /// </summary>
        /// <param name="errorCallback">Callback invoked when a violation is detected.</param>
        public AvoidAtAtIdentityRule(Action<string, string, int, int> errorCallback)
        {
            _errorCallback = errorCallback;
        }

        /// <summary>
        /// Gets the rule ID.
        /// </summary>
        public string RULE_NAME => "avoid-atat-identity";

        /// <summary>
        /// Gets the violation message.
        /// </summary>
        public string RULE_TEXT => "Avoid @@IDENTITY. Use SCOPE_IDENTITY() or OUTPUT.";

        /// <summary>
        /// Gets the violation severity.
        /// </summary>
        public RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

        /// <summary>
        /// Traverses global variable references and reports violations for <c>@@IDENTITY</c>.
        /// </summary>
        /// <param name="node">The global variable reference node to visit.</param>
        public override void Visit(GlobalVariableExpression node)
        {
            if (node == null)
            {
                base.Visit(node);
                return;
            }

            if (string.Equals(node.Name, "@@IDENTITY", StringComparison.OrdinalIgnoreCase))
            {
                var line = node.StartLine;
                var column = node.StartColumn;
                _errorCallback?.Invoke(RULE_NAME, RULE_TEXT, line, column);
            }

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
