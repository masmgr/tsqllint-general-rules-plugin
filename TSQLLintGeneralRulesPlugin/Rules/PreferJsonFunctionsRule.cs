using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin;

/// <summary>
/// Warns on common "hand-rolled" JSON string build/parse patterns to encourage built-in JSON functions.
/// </summary>
public sealed class PreferJsonFunctionsRule : SqlLintRuleBase
{
    private readonly List<BinaryExpression> _additionExpressions = new();

    public PreferJsonFunctionsRule(Action<string, string, int, int> errorCallback) : base(errorCallback)
    {
    }

    public override string RULE_NAME => "prefer-json-functions";

    public override string RULE_TEXT => "Prefer built-in JSON support (OPENJSON, JSON_VALUE, FOR JSON, JSON_QUERY, JSON_MODIFY) over manual string parsing/building.";

    public override RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

    public override void Visit(BinaryExpression node)
    {
        if (node?.BinaryExpressionType == BinaryExpressionType.Add)
        {
            _additionExpressions.Add(node);
        }

        base.Visit(node);
    }

    public override void Visit(FunctionCall node)
    {
        if (node?.FunctionName?.Value == null || node.Parameters == null || node.Parameters.Count == 0)
        {
            base.Visit(node);
            return;
        }

        var name = node.FunctionName.Value;
        if (!name.Equals("CHARINDEX", StringComparison.OrdinalIgnoreCase) &&
            !name.Equals("PATINDEX", StringComparison.OrdinalIgnoreCase))
        {
            base.Visit(node);
            return;
        }

        if (node.Parameters[0] is StringLiteral pattern && LooksJsonLike(pattern.Value))
        {
            ReportViolation(node.StartLine, node.StartColumn);
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

            if (LooksLikeJsonConstruction(expression))
            {
                ReportViolation(expression.StartLine, expression.StartColumn);
            }
        }
    }

    public override void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
    {
        // No automatic fix is provided for this rule.
    }

    private static bool LooksLikeJsonConstruction(BinaryExpression expression)
    {
        var finder = new JsonStringLiteralCollector();
        expression.Accept(finder);
        if (finder.Literals.Count == 0)
        {
            return false;
        }

        var hasOpenBrace = false;
        var hasCloseBrace = false;
        var hasColon = false;
        var hasQuote = false;

        foreach (var literal in finder.Literals)
        {
            if (literal.Value == null)
            {
                continue;
            }

            var value = literal.Value;
            hasOpenBrace |= value.Contains('{');
            hasCloseBrace |= value.Contains('}');
            hasColon |= value.Contains(':');
            hasQuote |= value.Contains('"');
        }

        return hasOpenBrace && hasCloseBrace && (hasColon || hasQuote);
    }

    private static bool LooksJsonLike(string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return false;
        }

        return value.Contains('{') || value.Contains('}') || value.Contains("\"") || value.Contains(":");
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

    private sealed class JsonStringLiteralCollector : TSqlFragmentVisitor
    {
        public List<StringLiteral> Literals { get; } = new();

        public override void Visit(StringLiteral node)
        {
            if (node?.Value != null && LooksJsonLike(node.Value))
            {
                Literals.Add(node);
            }
        }
    }
}


