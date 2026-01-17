using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin;

/// <summary>
/// Warns on STUFF(... FOR XML PATH('') ...) string-aggregation patterns in favor of STRING_AGG().
/// </summary>
public sealed class PreferStringAggOverStuffRule : SqlLintRuleBase
{

    public PreferStringAggOverStuffRule(Action<string, string, int, int> errorCallback) : base(errorCallback)
    {
    }

    public override string RULE_NAME => "prefer-string-agg-over-stuff";

    public override string RULE_TEXT => "Prefer STRING_AGG() over STUFF(... FOR XML PATH('') ...) for readability and correctness.";

    public override RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

    public override void Visit(FunctionCall node)
    {
        if (node?.FunctionName?.Value == null)
        {
            base.Visit(node);
            return;
        }

        if (!node.FunctionName.Value.Equals("STUFF", StringComparison.OrdinalIgnoreCase))
        {
            base.Visit(node);
            return;
        }

        if (node.Parameters == null || node.Parameters.Count == 0)
        {
            base.Visit(node);
            return;
        }

        var visitor = new XmlPathFinder();
        node.Parameters[0].Accept(visitor);
        if (visitor.Found)
        {
            ReportViolation(node.StartLine, node.StartColumn);
        }

        base.Visit(node);
    }

    public override void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
    {
        // No automatic fix is provided for this rule.
    }

    private sealed class XmlPathFinder : TSqlFragmentVisitor
    {
        public bool Found { get; private set; }

        public override void Visit(XmlForClause node)
        {
            if (Found || node == null)
            {
                return;
            }

            if (node.Options != null)
            {
                foreach (var option in node.Options)
                {
                    if (option?.OptionKind == XmlForClauseOptions.Path)
                    {
                        Found = true;
                        return;
                    }
                }
            }
        }
    }
}


