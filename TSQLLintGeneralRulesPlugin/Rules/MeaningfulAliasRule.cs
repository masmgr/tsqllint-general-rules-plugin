using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin;

/// <summary>
/// Warns on single-character table aliases in multi-table queries.
/// </summary>
public sealed class MeaningfulAliasRule : TSqlFragmentVisitor, ISqlLintRule
{
    private readonly Action<string, string, int, int> _errorCallback;

    public MeaningfulAliasRule(Action<string, string, int, int> errorCallback)
    {
        _errorCallback = errorCallback;
    }

    public string RULE_NAME => "meaningful-alias";

    public string RULE_TEXT => "Avoid single-character table aliases in multi-table queries; use meaningful aliases to reduce ambiguity (e.g., c for Customer is okay only when context is obvious).";

    public RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

    public override void Visit(QuerySpecification node)
    {
        if (node == null)
        {
            base.Visit(node);
            return;
        }

        if (CountTableReferences(node.FromClause) < 2)
        {
            base.Visit(node);
            return;
        }

        if (node.FromClause?.TableReferences != null)
        {
            foreach (var tableReference in node.FromClause.TableReferences)
            {
                ReportSingleCharacterAliases(tableReference);
            }
        }

        base.Visit(node);
    }

    public void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
    {
        // No automatic fix is provided for this rule.
    }

    private void ReportSingleCharacterAliases(TableReference? tableReference)
    {
        if (tableReference == null)
        {
            return;
        }

        if (tableReference is TableReferenceWithAlias withAlias &&
            withAlias.Alias?.Value != null &&
            withAlias.Alias.Value.Length == 1)
        {
            _errorCallback?.Invoke(RULE_NAME, RULE_TEXT, withAlias.Alias.StartLine, withAlias.Alias.StartColumn);
        }

        switch (tableReference)
        {
            case QualifiedJoin qualifiedJoin:
                ReportSingleCharacterAliases(qualifiedJoin.FirstTableReference);
                ReportSingleCharacterAliases(qualifiedJoin.SecondTableReference);
                break;
            case UnqualifiedJoin unqualifiedJoin:
                ReportSingleCharacterAliases(unqualifiedJoin.FirstTableReference);
                ReportSingleCharacterAliases(unqualifiedJoin.SecondTableReference);
                break;
            case JoinParenthesisTableReference parenthesis:
                ReportSingleCharacterAliases(parenthesis.Join);
                break;
        }
    }

    private static int CountTableReferences(FromClause? fromClause)
    {
        if (fromClause?.TableReferences == null || fromClause.TableReferences.Count == 0)
        {
            return 0;
        }

        var total = 0;
        foreach (var tableReference in fromClause.TableReferences)
        {
            total += CountTableReferences(tableReference);
        }

        return total;
    }

    private static int CountTableReferences(TableReference? tableReference)
    {
        if (tableReference == null)
        {
            return 0;
        }

        return tableReference switch
        {
            QualifiedJoin qualifiedJoin => CountTableReferences(qualifiedJoin.FirstTableReference) +
                CountTableReferences(qualifiedJoin.SecondTableReference),
            UnqualifiedJoin unqualifiedJoin => CountTableReferences(unqualifiedJoin.FirstTableReference) +
                CountTableReferences(unqualifiedJoin.SecondTableReference),
            JoinParenthesisTableReference parenthesis => CountTableReferences(parenthesis.Join),
            _ => 1
        };
    }
}

