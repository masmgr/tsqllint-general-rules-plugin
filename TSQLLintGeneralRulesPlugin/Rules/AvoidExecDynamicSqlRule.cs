using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin
{
    /// <summary>
    /// Detects execution of dynamic SQL using <c>EXEC</c> and recommends using <c>sp_executesql</c> with parameters.
    /// </summary>
    public sealed class AvoidExecDynamicSqlRule : TSqlFragmentVisitor, ISqlLintRule
    {
        private readonly Action<string, string, int, int> _errorCallback;

        /// <summary>
        /// Initializes the rule.
        /// </summary>
        /// <param name="errorCallback">Callback invoked when a violation is detected.</param>
        public AvoidExecDynamicSqlRule(Action<string, string, int, int> errorCallback)
        {
            _errorCallback = errorCallback;
        }

        /// <summary>
        /// Gets the rule ID.
        /// </summary>
        public string RULE_NAME => "avoid-exec-dynamic-sql";

        /// <summary>
        /// Gets the violation message.
        /// </summary>
        public string RULE_TEXT => "Avoid EXEC for dynamic SQL. Use sp_executesql with parameters.";

        /// <summary>
        /// Gets the violation severity.
        /// </summary>
        public RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

        /// <summary>
        /// Traverses EXECUTE statements and reports violations when dynamic SQL execution is detected.
        /// </summary>
        /// <param name="node">The EXECUTE statement node to visit.</param>
        public override void Visit(ExecuteStatement node)
        {
            if (node == null)
            {
                base.Visit(node);
                return;
            }

            var specification = node.ExecuteSpecification;
            if (specification?.ExecutableEntity is ExecutableStringList stringList && ContainsDynamicExpression(stringList))
            {
                GetViolationLocation(node, specification, stringList, out var line, out var column);
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

        private static bool ContainsDynamicExpression(ExecutableStringList stringList)
        {
            if (stringList.Strings == null || stringList.Strings.Count == 0)
            {
                return false;
            }

            foreach (var valueExpression in stringList.Strings)
            {
                if (valueExpression is StringLiteral)
                {
                    continue;
                }

                return true;
            }

            return false;
        }

        private static void GetViolationLocation(
            ExecuteStatement statement,
            ExecuteSpecification specification,
            ExecutableStringList stringList,
            out int line,
            out int column)
        {
            line = statement.StartLine;
            column = statement.StartColumn;

            if (specification.StartLine > 0)
            {
                line = specification.StartLine;
                column = specification.StartColumn;
            }

            if (stringList.Strings == null)
            {
                return;
            }

            foreach (var valueExpression in stringList.Strings)
            {
                if (valueExpression is StringLiteral)
                {
                    continue;
                }

                if (valueExpression.StartLine > 0)
                {
                    line = valueExpression.StartLine;
                    column = valueExpression.StartColumn;
                    return;
                }
            }

            if (stringList.StartLine > 0)
            {
                line = stringList.StartLine;
                column = stringList.StartColumn;
            }
        }
    }
}
