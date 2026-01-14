using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin;

/// <summary>
/// Encourages CONCAT_WS() over + when building delimited strings.
/// </summary>
public sealed class PreferConcatWsRule : SqlLintRuleBase
{
    private readonly List<BinaryExpression> _additionExpressions = new();

    public PreferConcatWsRule(Action<string, string, int, int> errorCallback) : base(errorCallback)
    {
    }

    public override string RULE_NAME => "prefer-concat-ws";

    public override string RULE_TEXT => "Prefer CONCAT_WS(separator, ...) over + when concatenating values with repeated separators.";

    public override RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

    public override void Visit(BinaryExpression node)
    {
        if (node?.BinaryExpressionType == BinaryExpressionType.Add)
        {
            _additionExpressions.Add(node);
        }

        base.Visit(node);
    }

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

            if (TryMatchConcatWsCandidate(expression, out _, out _))
            {
                ReportViolation(expression.StartLine, expression.StartColumn);
            }
        }
    }

    public override void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
    {
        // No automatic fix is provided for this rule.
    }

    internal static bool TryMatchConcatWsCandidate(BinaryExpression expression, out string separatorValue, out int valueOperandCount)
    {
        separatorValue = string.Empty;
        valueOperandCount = 0;

        var operands = new List<ScalarExpression>();
        CollectConcatenationOperands(expression, operands);

        if (operands.Count < 5)
        {
            return false;
        }

        if (!TryGetSeparator(operands[1], out separatorValue))
        {
            return false;
        }

        if (string.IsNullOrEmpty(separatorValue))
        {
            return false;
        }

        var separatorsSeen = 0;
        var valuesSeen = 0;
        var anyNullHandling = false;

        for (var i = 0; i < operands.Count; i++)
        {
            if (i % 2 == 1)
            {
                if (!TryGetSeparator(operands[i], out var currentSeparator) || !string.Equals(currentSeparator, separatorValue, StringComparison.Ordinal))
                {
                    return false;
                }

                separatorsSeen++;
                continue;
            }

            valuesSeen++;
            anyNullHandling |= ContainsNullHandlingCall(operands[i]);
        }

        valueOperandCount = valuesSeen;
        return separatorsSeen >= 2 && valuesSeen >= 3 && anyNullHandling;
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

    private static bool TryGetSeparator(ScalarExpression expression, out string value)
    {
        value = string.Empty;
        expression = UnwrapParentheses(expression);

        if (expression is not StringLiteral literal || literal.Value == null)
        {
            return false;
        }

        value = literal.Value;
        return true;
    }

    private static ScalarExpression UnwrapParentheses(ScalarExpression expression)
    {
        var current = expression;
        while (current is ParenthesisExpression parenthesisExpression && parenthesisExpression.Expression != null)
        {
            current = parenthesisExpression.Expression;
        }

        return current;
    }

    private static bool ContainsNullHandlingCall(ScalarExpression expression)
    {
        var finder = new NullHandlingCallFinder();
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

    private sealed class NullHandlingCallFinder : TSqlFragmentVisitor
    {
        public bool Found { get; private set; }

        public override void Visit(FunctionCall node)
        {
            if (Found || node?.FunctionName?.Value == null)
            {
                return;
            }

            var name = node.FunctionName.Value;
            if (name.Equals("ISNULL", StringComparison.OrdinalIgnoreCase) ||
                name.Equals("COALESCE", StringComparison.OrdinalIgnoreCase))
            {
                Found = true;
                return;
            }

            base.Visit(node);
        }
    }
}


