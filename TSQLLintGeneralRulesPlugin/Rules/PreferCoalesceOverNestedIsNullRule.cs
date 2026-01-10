using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin
{
    /// <summary>
    /// Detects nested <c>ISNULL</c> calls and recommends using <c>COALESCE</c> instead.
    /// </summary>
    public sealed class PreferCoalesceOverNestedIsNullRule : TSqlFragmentVisitor, ISqlLintRule
    {
        private readonly Action<string, string, int, int> _errorCallback;

        /// <summary>
        /// Initializes the rule.
        /// </summary>
        /// <param name="errorCallback">Callback invoked when a violation is detected.</param>
        public PreferCoalesceOverNestedIsNullRule(Action<string, string, int, int> errorCallback)
        {
            _errorCallback = errorCallback;
        }

        /// <summary>
        /// Gets the rule ID.
        /// </summary>
        public string RULE_NAME => "prefer-coalesce-over-nested-isnull";

        /// <summary>
        /// Gets the violation message.
        /// </summary>
        public string RULE_TEXT => "Use COALESCE instead of nested ISNULL calls.";

        /// <summary>
        /// Gets the violation severity.
        /// </summary>
        public RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

        /// <summary>
        /// Traverses function calls and reports violations if nested <c>ISNULL</c> calls are found.
        /// </summary>
        /// <param name="node">The function call node to visit.</param>
        public override void Visit(FunctionCall node)
        {
            if (!IsIsNullFunction(node))
            {
                base.Visit(node);
                return;
            }

            if (node.Parameters == null || node.Parameters.Count == 0)
            {
                base.Visit(node);
                return;
            }

            foreach (var parameter in node.Parameters)
            {
                if (parameter == null)
                {
                    continue;
                }

                if (ContainsIsNullCall(parameter))
                {
                    var line = node.FunctionName?.StartLine ?? node.StartLine;
                    var column = node.FunctionName?.StartColumn ?? node.StartColumn;
                    _errorCallback?.Invoke(RULE_NAME, RULE_TEXT, line, column);
                    break;
                }
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

        private static bool IsIsNullFunction(FunctionCall node)
        {
            return string.Equals(node.FunctionName?.Value, "ISNULL", StringComparison.OrdinalIgnoreCase);
        }

        private static bool ContainsIsNullCall(TSqlFragment fragment)
        {
            var finder = new IsNullCallFinder();
            fragment.Accept(finder);
            return finder.Found;
        }

        private sealed class IsNullCallFinder : TSqlFragmentVisitor
        {
            public bool Found { get; private set; }

            public override void Visit(FunctionCall node)
            {
                if (Found)
                {
                    return;
                }

                if (IsIsNullFunction(node))
                {
                    Found = true;
                    return;
                }

                base.Visit(node);
            }
        }
    }
}
