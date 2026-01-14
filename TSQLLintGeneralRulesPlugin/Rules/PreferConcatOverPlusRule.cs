using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin;

/// <summary>
/// Encourages CONCAT() over + when string concatenation uses NULL-handling helpers (ISNULL/COALESCE).
/// </summary>
public sealed class PreferConcatOverPlusRule : SqlLintRuleBase
{
    private readonly List<BinaryExpression> _additionExpressions = new();

    public PreferConcatOverPlusRule(Action<string, string, int, int> errorCallback) : base(errorCallback)
    {
    }

    public override string RULE_NAME => "prefer-concat-over-plus";

    public override string RULE_TEXT => "Prefer CONCAT() for string concatenation when using ISNULL/COALESCE; it avoids NULL-propagation surprises and improves readability.";

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

            if (PreferConcatWsRule.TryMatchConcatWsCandidate(expression, out _, out _))
            {
                continue;
            }

            if (ShouldReport(expression))
            {
                ReportViolation(expression.StartLine, expression.StartColumn);
            }
        }
    }

    public override void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
    {
        // No automatic fix is provided for this rule.
    }

    private static bool ShouldReport(BinaryExpression expression)
    {
        var operands = new List<ScalarExpression>();
        CollectConcatenationOperands(expression, operands);

        if (!ContainsStringLiteralDeep(operands))
        {
            return false;
        }

        foreach (var operand in operands)
        {
            if (operand == null)
            {
                continue;
            }

            if (ContainsNullHandlingCall(operand))
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

    private static bool ContainsStringLiteralDeep(IEnumerable<ScalarExpression> operands)
    {
        foreach (var operand in operands)
        {
            if (operand == null)
            {
                continue;
            }

            var finder = new StringLiteralFinder();
            operand.Accept(finder);
            if (finder.Found)
            {
                return true;
            }
        }

        return false;
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

    private sealed class StringLiteralFinder : TSqlFragmentVisitor
    {
        public bool Found { get; private set; }

        public override void Visit(StringLiteral node)
        {
            Found = true;
        }
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


