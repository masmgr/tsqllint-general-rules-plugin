using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin;

/// <summary>
/// Warns on SELECT ... INTO because it implicitly creates a table schema.
/// </summary>
public sealed class DisallowSelectIntoRule : SqlLintRuleBase
{

    public DisallowSelectIntoRule(Action<string, string, int, int> errorCallback) : base(errorCallback)
    {
    }

    public override string RULE_NAME => "disallow-select-into";

    public override string RULE_TEXT => "Avoid SELECT ... INTO because it implicitly creates a table schema. Prefer explicit CREATE TABLE + INSERT for repeatable schema control.";

    public override RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

    public override void Visit(SelectStatement node)
    {
        if (node == null)
        {
            base.Visit(node);
            return;
        }

        if (node.Into != null)
        {
            ReportViolation(node.Into.StartLine, node.Into.StartColumn);
        }

        base.Visit(node);
    }

    public override void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
    {
        // No automatic fix is provided for this rule.
    }
}


