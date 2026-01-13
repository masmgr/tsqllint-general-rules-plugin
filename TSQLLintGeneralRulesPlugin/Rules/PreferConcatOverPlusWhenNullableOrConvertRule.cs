using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin
{
    /// <summary>
    /// Encourages CONCAT for string building when NULL-aware or conversion helpers are present.
    /// </summary>
    public sealed class PreferConcatOverPlusWhenNullableOrConvertRule : TSqlFragmentVisitor, ISqlLintRule
    {
        private readonly Action<string, string, int, int> _errorCallback;
        private readonly List<BinaryExpression> _additionExpressions = new();

        /// <summary>
        /// Initializes the rule.
        /// </summary>
        /// <param name="errorCallback">Callback invoked for violations.</param>
        public PreferConcatOverPlusWhenNullableOrConvertRule(Action<string, string, int, int> errorCallback)
        {
            _errorCallback = errorCallback;
        }

        /// <inheritdoc/>
        public string RULE_NAME => "prefer-concat-over-plus-when-nullable-or-convert";

        /// <inheritdoc/>
        public string RULE_TEXT => "Use CONCAT when string building involves ISNULL/CONVERT/CAST to avoid NULL propagation or readability issues.";

        /// <inheritdoc/>
        public RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

        /// <inheritdoc/>
        public override void Visit(BinaryExpression node)
        {
            if (node?.BinaryExpressionType == BinaryExpressionType.Add)
            {
                _additionExpressions.Add(node);
            }

            base.Visit(node);
        }

        /// <inheritdoc/>
        public override void ExplicitVisit(TSqlScript node)
        {
            if (node == null)
            {
                return;
            }

            base.ExplicitVisit(node);

            foreach (var expression in _additionExpressions)
            {
                if (!IsOutermostAddition(expression))
                {
                    continue;
                }

                if (ShouldReport(expression))
                {
                    _errorCallback?.Invoke(RULE_NAME, RULE_TEXT, expression.StartLine, expression.StartColumn);
                }
            }
        }

        /// <inheritdoc/>
        public void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
        {
            // No automatic fix is provided for this rule.
        }

        private static bool ShouldReport(BinaryExpression expression)
        {
            var operands = new List<ScalarExpression>();
            CollectConcatenationOperands(expression, operands);

            if (!ContainsStringLiteral(operands))
            {
                return false;
            }

            foreach (var operand in operands)
            {
                if (operand == null)
                {
                    continue;
                }

                if (ContainsProblematicCall(operand))
                {
                    return true;
                }
            }

            return false;
        }

        private static void CollectConcatenationOperands(ScalarExpression? expression, List<ScalarExpression> operands)
        {
            if (expression == null)
            {
                return;
            }

            if (expression is BinaryExpression binary && binary.BinaryExpressionType == BinaryExpressionType.Add)
            {
                CollectConcatenationOperands(binary.FirstExpression, operands);
                CollectConcatenationOperands(binary.SecondExpression, operands);
                return;
            }

            operands.Add(expression);
        }

        private static bool ContainsStringLiteral(IEnumerable<ScalarExpression> operands)
        {
            foreach (var operand in operands)
            {
                if (operand is StringLiteral)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool ContainsProblematicCall(ScalarExpression expression)
        {
            var finder = new ProblematicCallFinder();
            expression.Accept(finder);
            return finder.Found;
        }

        private bool IsOutermostAddition(BinaryExpression candidate)
        {
            var candidateStart = candidate.StartOffset;
            var candidateEnd = candidateStart + candidate.FragmentLength;

            foreach (var other in _additionExpressions)
            {
                if (ReferenceEquals(other, candidate))
                {
                    continue;
                }

                if (other.BinaryExpressionType != BinaryExpressionType.Add)
                {
                    continue;
                }

                var otherStart = other.StartOffset;
                var otherEnd = otherStart + other.FragmentLength;

                var contains = otherStart <= candidateStart && otherEnd >= candidateEnd;
                var strictlyLarger = otherStart < candidateStart || otherEnd > candidateEnd;

                if (contains && strictlyLarger)
                {
                    return false;
                }
            }

            return true;
        }

        private sealed class ProblematicCallFinder : TSqlFragmentVisitor
        {
            public bool Found { get; private set; }

            public override void Visit(FunctionCall node)
            {
                if (Found)
                {
                    return;
                }

                if (node != null && string.Equals(node.FunctionName?.Value, "ISNULL", StringComparison.OrdinalIgnoreCase))
                {
                    Found = true;
                    return;
                }

                base.Visit(node);
            }

            public override void Visit(ConvertCall node)
            {
                Found = true;
            }

            public override void Visit(CastCall node)
            {
                Found = true;
            }
        }
    }
}
