using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin
{
    /// <summary>
    /// Requires MS_Description extended properties alongside CREATE TABLE definitions when table and schema names are provided inline.
    /// </summary>
    public sealed class RequireMsDescriptionForTableDefinitionFileRule : TSqlFragmentVisitor, ISqlLintRule
    {
        private static readonly HashSet<string> ValidProcedures = new(StringComparer.OrdinalIgnoreCase)
        {
            "sp_addextendedproperty",
            "sp_updateextendedproperty"
        };

        private readonly Action<string, string, int, int> _errorCallback;
        private readonly Dictionary<string, CreateTableStatement> _tableDefinitions =
            new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _tablesWithDescriptions =
            new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initializes the rule.
        /// </summary>
        /// <param name="errorCallback">Callback invoked for violations.</param>
        public RequireMsDescriptionForTableDefinitionFileRule(Action<string, string, int, int> errorCallback)
        {
            _errorCallback = errorCallback;
        }

        /// <inheritdoc/>
        public string RULE_NAME => "require-ms-description-for-table-definition-file";

        /// <inheritdoc/>
        public string RULE_TEXT => "Table definition files must include an MS_Description extended property for the table.";

        /// <inheritdoc/>
        public RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

        /// <inheritdoc/>
        public override void Visit(CreateTableStatement node)
        {
            if (node == null)
            {
                base.Visit(node);
                return;
            }

            if (IsTemporaryTable(node.SchemaObjectName))
            {
                base.Visit(node);
                return;
            }

            var tableKey = GetTableKey(node.SchemaObjectName);
            if (!string.IsNullOrWhiteSpace(tableKey) && !_tableDefinitions.ContainsKey(tableKey))
            {
                _tableDefinitions[tableKey] = node;
            }

            base.Visit(node);
        }

        /// <inheritdoc/>
        public override void Visit(ExecuteStatement node)
        {
            if (node == null)
            {
                base.Visit(node);
                return;
            }

            var specification = node.ExecuteSpecification;
            if (specification?.ExecutableEntity is ExecutableProcedureReference proc)
            {
                var procedureName = GetProcedureName(proc);
                if (IsTargetProcedure(procedureName))
                {
                    var parameters = BuildParameterMap(proc.Parameters);
                    if (IsMsDescriptionCall(parameters, out var schemaName, out var tableName))
                    {
                        var key = CreateKey(schemaName, tableName);
                        if (!string.IsNullOrWhiteSpace(key))
                        {
                            _tablesWithDescriptions.Add(key);
                        }
                    }
                }
            }

            base.Visit(node);
        }

        /// <inheritdoc/>
        public override void ExplicitVisit(TSqlScript node)
        {
            if (node == null)
            {
                return;
            }

            base.ExplicitVisit(node);

            foreach (var entry in _tableDefinitions)
            {
                if (_tablesWithDescriptions.Contains(entry.Key))
                {
                    continue;
                }

                var location = entry.Value.SchemaObjectName?.BaseIdentifier ?? (TSqlFragment)entry.Value;
                _errorCallback?.Invoke(RULE_NAME, RULE_TEXT, location.StartLine, location.StartColumn);
            }
        }

        /// <inheritdoc/>
        public void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
        {
            // No automatic fix is provided for this rule.
        }

        private static bool IsMsDescriptionCall(
            Dictionary<string, string?> parameters,
            out string? schemaName,
            out string? tableName)
        {
            schemaName = null;
            tableName = null;

            if (!parameters.TryGetValue("@name", out var propertyName) ||
                !string.Equals(propertyName, "MS_Description", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (!parameters.TryGetValue("@level0type", out var level0Type) ||
                !string.Equals(level0Type, "SCHEMA", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (!parameters.TryGetValue("@level1type", out var level1Type) ||
                !string.Equals(level1Type, "TABLE", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            if (!parameters.TryGetValue("@level0name", out var schema) ||
                string.IsNullOrWhiteSpace(schema) ||
                !parameters.TryGetValue("@level1name", out var table) ||
                string.IsNullOrWhiteSpace(table))
            {
                return false;
            }

            schemaName = schema!.Trim();
            tableName = table!.Trim();
            return true;
        }

        private static Dictionary<string, string?> BuildParameterMap(IList<ExecuteParameter>? parameters)
        {
            var map = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            if (parameters == null)
            {
                return map;
            }

            foreach (var parameter in parameters)
            {
                if (parameter?.Variable == null || string.IsNullOrWhiteSpace(parameter.Variable.Name))
                {
                    continue;
                }

                var parameterName = "@" + parameter.Variable.Name.TrimStart('@');
                map[parameterName] = ExtractLiteralValue(parameter.ParameterValue);
            }

            return map;
        }

        private static string? ExtractLiteralValue(ScalarExpression? expression)
        {
            if (expression is Literal literal && !string.IsNullOrWhiteSpace(literal.Value))
            {
                return literal.Value.Trim();
            }

            return null;
        }

        private static string GetProcedureName(ExecutableProcedureReference procedure)
        {
            if (procedure?.ProcedureReference?.ProcedureReference?.Name == null)
            {
                return string.Empty;
            }

            return GetTableKey(procedure.ProcedureReference.ProcedureReference.Name) ?? string.Empty;
        }

        private static bool IsTargetProcedure(string procedureName)
        {
            if (string.IsNullOrWhiteSpace(procedureName))
            {
                return false;
            }

            var trimmed = procedureName.Trim();
            var parts = trimmed.Split('.');
            var simpleName = parts.Length > 0 ? parts[^1] : trimmed;
            return ValidProcedures.Contains(simpleName);
        }

        private static string? GetTableKey(SchemaObjectName? name)
        {
            if (name == null || name.Identifiers == null || name.Identifiers.Count == 0)
            {
                return null;
            }

            var parts = new List<string>();
            foreach (var identifier in name.Identifiers)
            {
                if (identifier != null && !string.IsNullOrWhiteSpace(identifier.Value))
                {
                    parts.Add(identifier.Value);
                }
            }

            if (parts.Count == 0)
            {
                return null;
            }

            return string.Join('.', parts);
        }

        private static string CreateKey(string? schema, string? table)
        {
            if (string.IsNullOrWhiteSpace(schema) || string.IsNullOrWhiteSpace(table))
            {
                return string.Empty;
            }

            return schema.Trim() + "." + table.Trim();
        }

        private static bool IsTemporaryTable(SchemaObjectName? name)
        {
            var baseName = name?.BaseIdentifier?.Value;
            return !string.IsNullOrWhiteSpace(baseName) && baseName.StartsWith("#", StringComparison.Ordinal);
        }
    }
}
