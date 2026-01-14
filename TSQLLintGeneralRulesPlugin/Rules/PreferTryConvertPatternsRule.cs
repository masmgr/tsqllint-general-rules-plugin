using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin;

/// <summary>
/// Encourages TRY_CONVERT/TRY_CAST over CASE + ISNUMERIC/ISDATE gating patterns.
/// </summary>
public sealed class PreferTryConvertPatternsRule : SqlLintRuleBase
{

    public PreferTryConvertPatternsRule(Action<string, string, int, int> errorCallback) : base(errorCallback)
    {
    }

    public override string RULE_NAME => "prefer-try-convert-patterns";

    public override string RULE_TEXT => "Prefer TRY_CONVERT/TRY_CAST over CASE WHEN ISNUMERIC/ISDATE(...) THEN CONVERT/CAST(...) patterns for clarity and correctness.";

    public override RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

    public override void Visit(SearchedCaseExpression node)
    {
        if (node?.WhenClauses == null || node.WhenClauses.Count == 0)
        {
            base.Visit(node);
            return;
        }

        foreach (var whenClause in node.WhenClauses)
        {
            if (whenClause?.WhenExpression == null || whenClause.ThenExpression == null)
            {
                continue;
            }

            if (!TryGetGuardedExpressionKey(whenClause.WhenExpression, out var guardedKey))
            {
                continue;
            }

            if (IsConvertOrCastOfKey(whenClause.ThenExpression, guardedKey))
            {
                ReportViolation(node.StartLine, node.StartColumn);
                break;
            }
        }

        base.Visit(node);
    }

    public override void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
    {
        // No automatic fix is provided for this rule.
    }

    private static bool TryGetGuardedExpressionKey(BooleanExpression booleanExpression, out string key)
    {
        key = string.Empty;

        var finder = new NumericDateGuardFinder();
        booleanExpression.Accept(finder);

        if (finder.GuardedExpression == null)
        {
            return false;
        }

        var guardedKey = TryGetExpressionKey(finder.GuardedExpression);
        if (guardedKey == null)
        {
            return false;
        }

        key = guardedKey;
        return true;
    }

    private static bool IsConvertOrCastOfKey(ScalarExpression expression, string key)
    {
        expression = UnwrapParentheses(expression) ?? expression;

        switch (expression)
        {
            case ConvertCall convertCall:
                return TryGetExpressionKey(convertCall.Parameter) == key;
            case CastCall castCall:
                return TryGetExpressionKey(castCall.Parameter) == key;
            case FunctionCall functionCall when functionCall.FunctionName?.Value != null:
                var name = functionCall.FunctionName.Value;
                if (name.Equals("CONVERT", StringComparison.OrdinalIgnoreCase) ||
                    name.Equals("CAST", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var parameter in functionCall.Parameters)
                    {
                        if (TryGetExpressionKey(parameter) == key)
                        {
                            return true;
                        }
                    }
                }

                return false;
            default:
                return false;
        }
    }

    private static ScalarExpression? UnwrapParentheses(ScalarExpression? expression)
    {
        var current = expression;
        if (current == null)
        {
            return null;
        }

        while (current is ParenthesisExpression parenthesisExpression && parenthesisExpression.Expression != null)
        {
            current = parenthesisExpression.Expression;
        }

        return current;
    }

    private static string? TryGetExpressionKey(ScalarExpression? expression)
    {
        expression = UnwrapParentheses(expression);
        if (expression == null)
        {
            return null;
        }

        return expression switch
        {
            VariableReference variableReference => variableReference.Name,
            ColumnReferenceExpression columnReferenceExpression => TryGetColumnKey(columnReferenceExpression),
            _ => null
        };
    }

    private static string? TryGetColumnKey(ColumnReferenceExpression columnReferenceExpression)
    {
        var identifiers = columnReferenceExpression.MultiPartIdentifier?.Identifiers;
        if (identifiers == null || identifiers.Count == 0)
        {
            return null;
        }

        var parts = new List<string>(identifiers.Count);
        foreach (var identifier in identifiers)
        {
            if (identifier?.Value == null)
            {
                return null;
            }

            parts.Add(identifier.Value);
        }

        return string.Join(".", parts);
    }

    private sealed class NumericDateGuardFinder : TSqlFragmentVisitor
    {
        public ScalarExpression? GuardedExpression { get; private set; }

        public override void Visit(FunctionCall node)
        {
            if (GuardedExpression != null)
            {
                return;
            }

            if (node?.FunctionName?.Value == null || node.Parameters == null || node.Parameters.Count != 1)
            {
                base.Visit(node);
                return;
            }

            var name = node.FunctionName.Value;
            if (name.Equals("ISNUMERIC", StringComparison.OrdinalIgnoreCase) ||
                name.Equals("ISDATE", StringComparison.OrdinalIgnoreCase))
            {
                GuardedExpression = node.Parameters[0];
                return;
            }

            base.Visit(node);
        }
    }
}


