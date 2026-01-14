using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin;

/// <summary>
/// Warns on SELECT ... INTO because it implicitly creates a table schema.
/// </summary>
public sealed class DisallowSelectIntoRule : TSqlFragmentVisitor, ISqlLintRule
{
    private readonly Action<string, string, int, int> _errorCallback;

    public DisallowSelectIntoRule(Action<string, string, int, int> errorCallback)
    {
        _errorCallback = errorCallback;
    }

    public string RULE_NAME => "disallow-select-into";

    public string RULE_TEXT => "Avoid SELECT ... INTO because it implicitly creates a table schema. Prefer explicit CREATE TABLE + INSERT for repeatable schema control.";

    public RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

    public override void Visit(SelectStatement node)
    {
        if (node == null)
        {
            base.Visit(node);
            return;
        }

        if (node.Into != null)
        {
            _errorCallback?.Invoke(RULE_NAME, RULE_TEXT, node.Into.StartLine, node.Into.StartColumn);
        }

        base.Visit(node);
    }

    public void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
    {
        // No automatic fix is provided for this rule.
    }
}
