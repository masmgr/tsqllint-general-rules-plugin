using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin;

/// <summary>
/// Warns on NOLOCK/READUNCOMMITTED table hints and READ UNCOMMITTED transaction isolation level.
/// </summary>
public sealed class AvoidNolockOrReadUncommittedRule : TSqlFragmentVisitor, ISqlLintRule
{
    private readonly Action<string, string, int, int> _errorCallback;

    public AvoidNolockOrReadUncommittedRule(Action<string, string, int, int> errorCallback)
    {
        _errorCallback = errorCallback;
    }

    public string RULE_NAME => "avoid-nolock-or-read-uncommitted";

    public string RULE_TEXT => "NOLOCK and READ UNCOMMITTED allow dirty reads which can return inconsistent or incorrect data. Prefer appropriate isolation levels or snapshot isolation.";

    public RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

    public override void Visit(TableHint node)
    {
        if (node == null)
        {
            base.Visit(node);
            return;
        }

        if (node.HintKind == TableHintKind.NoLock ||
            node.HintKind == TableHintKind.ReadUncommitted)
        {
            _errorCallback?.Invoke(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
        }

        base.Visit(node);
    }

    public override void Visit(SetTransactionIsolationLevelStatement node)
    {
        if (node == null)
        {
            base.Visit(node);
            return;
        }

        if (node.Level == IsolationLevel.ReadUncommitted)
        {
            _errorCallback?.Invoke(RULE_NAME, RULE_TEXT, node.StartLine, node.StartColumn);
        }

        base.Visit(node);
    }

    public void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
    {
        // No automatic fix is provided for this rule.
    }
}

