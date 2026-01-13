using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin
{
    /// <summary>
    /// Requires BEGIN/END for IF/ELSE clauses except when the clause only contains a single RETURN/BREAK/CONTINUE.
    /// </summary>
    public sealed class RequireBeginEndForIfWithControlFlowExceptionRule : TSqlFragmentVisitor, ISqlLintRule
    {
        private readonly Action<string, string, int, int> _errorCallback;

        /// <summary>
        /// Initializes the rule.
        /// </summary>
        /// <param name="errorCallback">Callback invoked for violations.</param>
        public RequireBeginEndForIfWithControlFlowExceptionRule(Action<string, string, int, int> errorCallback)
        {
            _errorCallback = errorCallback;
        }

        /// <inheritdoc/>
        public string RULE_NAME => "require-begin-end-for-if-with-controlflow-exception";

        /// <inheritdoc/>
        public string RULE_TEXT => "Wrap IF/ELSE clauses in BEGIN/END unless they consist of a single RETURN, BREAK, or CONTINUE.";

        /// <inheritdoc/>
        public RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

        /// <inheritdoc/>
        public override void Visit(IfStatement node)
        {
            if (node == null)
            {
                base.Visit(node);
                return;
            }

            EvaluateClause(node.ThenStatement, node.StartLine, node.StartColumn);

            if (node.ElseStatement != null && node.ElseStatement is not IfStatement)
            {
                EvaluateClause(node.ElseStatement, node.ElseStatement.StartLine, node.ElseStatement.StartColumn);
            }

            base.Visit(node);
        }

        /// <inheritdoc/>
        public void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
        {
            // No automatic fix is provided for this rule.
        }

        private void EvaluateClause(TSqlStatement? statement, int fallbackLine, int fallbackColumn)
        {
            if (statement == null)
            {
                return;
            }

            if (!NeedsBeginEnd(statement))
            {
                return;
            }

            var line = statement.StartLine > 0 ? statement.StartLine : fallbackLine;
            var column = statement.StartColumn > 0 ? statement.StartColumn : fallbackColumn;
            _errorCallback?.Invoke(RULE_NAME, RULE_TEXT, line, column);
        }

        private static bool NeedsBeginEnd(TSqlStatement statement)
        {
            if (statement is BeginEndBlockStatement)
            {
                return false;
            }

            return statement switch
            {
                ReturnStatement _ => false,
                BreakStatement _ => false,
                ContinueStatement _ => false,
                _ => true
            };
        }
    }
}
