using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin;

/// <summary>
/// Warns on NOLOCK/READUNCOMMITTED table hints and READ UNCOMMITTED transaction isolation level.
/// </summary>
public sealed class AvoidNolockOrReadUncommittedRule : SqlLintRuleBase
{

    public AvoidNolockOrReadUncommittedRule(Action<string, string, int, int> errorCallback) : base(errorCallback)
    {
    }

    public override string RULE_NAME => "avoid-nolock-or-read-uncommitted";

    public override string RULE_TEXT => "NOLOCK and READ UNCOMMITTED allow dirty reads which can return inconsistent or incorrect data. Prefer appropriate isolation levels or snapshot isolation.";

    public override RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

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
            ReportViolation(node.StartLine, node.StartColumn);
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
            ReportViolation(node.StartLine, node.StartColumn);
        }

        base.Visit(node);
    }

    public override void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
    {
        // No automatic fix is provided for this rule.
    }
}


