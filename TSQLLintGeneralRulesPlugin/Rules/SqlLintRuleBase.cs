using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin;

public abstract class SqlLintRuleBase : TSqlFragmentVisitor, ISqlLintRule
{
    private readonly Action<string, string, int, int>? _errorCallback;

    protected SqlLintRuleBase(Action<string, string, int, int>? errorCallback)
    {
        _errorCallback = errorCallback;
    }

    public abstract string RULE_NAME { get; }

    public abstract string RULE_TEXT { get; }

    public abstract RuleViolationSeverity RULE_SEVERITY { get; }

    public virtual void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
    {
        // No automatic fix is provided by default.
    }

    protected void ReportViolation(int line, int column)
    {
        _errorCallback?.Invoke(RULE_NAME, RULE_TEXT, line, column);
    }

    protected void ReportViolation(TSqlFragment fragment)
    {
        if (fragment == null)
        {
            return;
        }

        ReportViolation(fragment.StartLine, fragment.StartColumn);
    }
}

