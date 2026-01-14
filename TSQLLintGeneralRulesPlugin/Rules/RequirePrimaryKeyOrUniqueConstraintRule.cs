using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin
{
    /// <summary>
    /// Requires a PRIMARY KEY or UNIQUE constraint on user tables to ensure row uniqueness.
    /// </summary>
    public sealed class RequirePrimaryKeyOrUniqueConstraintRule : SqlLintRuleBase
    {
        private readonly Dictionary<string, List<CreateTableStatement>> _createTables =
    new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _tablesWithUniqueness =
            new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initializes the rule.
        /// </summary>
        /// <param name="errorCallback">Callback invoked when a violation is detected.</param>
        public RequirePrimaryKeyOrUniqueConstraintRule(Action<string, string, int, int> errorCallback) : base(errorCallback)
        {
        }

        /// <summary>
        /// Gets the rule ID.
        /// </summary>
        public override string RULE_NAME => "require-primary-key-or-unique-constraint";

        /// <summary>
        /// Gets the violation message.
        /// </summary>
        public override string RULE_TEXT => "User tables must define a PRIMARY KEY or UNIQUE constraint to ensure row uniqueness.";

        /// <summary>
        /// Gets the violation severity.
        /// </summary>
        public override RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Error;

        /// <summary>
        /// Visits CREATE TABLE statements to collect table definitions and inline constraints.
        /// </summary>
        /// <param name="node">The CREATE TABLE statement.</param>
        public override void Visit(CreateTableStatement node)
        {
            if (node == null)
            {
                return;
            }

            if (IsTemporaryTable(node.SchemaObjectName))
            {
                return;
            }

            var tableKey = GetTableKey(node.SchemaObjectName);
            if (!string.IsNullOrWhiteSpace(tableKey))
            {
                if (!_createTables.TryGetValue(tableKey, out var tableStatements))
                {
                    tableStatements = new List<CreateTableStatement>();
                    _createTables[tableKey] = tableStatements;
                }

                tableStatements.Add(node);

                if (HasPrimaryKeyOrUnique(node.Definition))
                {
                    _tablesWithUniqueness.Add(tableKey);
                }
            }

            base.Visit(node);
        }

        /// <summary>
        /// Visits ALTER TABLE statements to collect PRIMARY KEY or UNIQUE constraints.
        /// </summary>
        /// <param name="node">The ALTER TABLE ADD TABLE ELEMENT statement.</param>
        public override void Visit(AlterTableAddTableElementStatement node)
        {
            if (node == null)
            {
                return;
            }

            if (IsTemporaryTable(node.SchemaObjectName))
            {
                return;
            }

            var tableKey = GetTableKey(node.SchemaObjectName);
            if (!string.IsNullOrWhiteSpace(tableKey) && HasPrimaryKeyOrUnique(node.Definition))
            {
                _tablesWithUniqueness.Add(tableKey);
            }

            base.Visit(node);
        }

        /// <summary>
        /// Visits CREATE INDEX statements to collect UNIQUE indexes.
        /// </summary>
        /// <param name="node">The CREATE INDEX statement.</param>
        public override void Visit(CreateIndexStatement node)
        {
            if (node == null || !node.Unique)
            {
                return;
            }

            if (IsTemporaryTable(node.OnName))
            {
                return;
            }

            var tableKey = GetTableKey(node.OnName);
            if (!string.IsNullOrWhiteSpace(tableKey))
            {
                _tablesWithUniqueness.Add(tableKey);
            }

            base.Visit(node);
        }

        /// <summary>
        /// Visits the script to emit violations after collecting all table-related definitions.
        /// </summary>
        /// <param name="node">The T-SQL script.</param>
        public override void ExplicitVisit(TSqlScript node)
        {
            if (node == null)
            {
                return;
            }

            base.ExplicitVisit(node);

            foreach (var entry in _createTables)
            {
                if (_tablesWithUniqueness.Contains(entry.Key))
                {
                    continue;
                }

                foreach (var createTable in entry.Value)
                {
                    var location = createTable.SchemaObjectName?.BaseIdentifier ?? (TSqlFragment)createTable;
                    ReportViolation(location.StartLine, location.StartColumn);
                }
            }
        }

        /// <summary>
        /// Automatically fixes rule violations (no automatic fix is provided for this rule).
        /// </summary>
        /// <param name="fileLines">Array of lines in the file.</param>
        /// <param name="ruleViolation">The rule violation information.</param>
        /// <param name="actions">Line edit actions.</param>
        public override void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
        {
            // No automatic fix is provided for this rule.
        }

        private static bool HasPrimaryKeyOrUnique(TableDefinition? definition)
        {
            if (definition == null)
            {
                return false;
            }

            if (definition.TableConstraints != null
                && definition.TableConstraints.OfType<UniqueConstraintDefinition>().Any())
            {
                return true;
            }

            if (definition.Indexes != null && definition.Indexes.Any(index => index.Unique))
            {
                return true;
            }

            if (definition.ColumnDefinitions == null)
            {
                return false;
            }

            foreach (var column in definition.ColumnDefinitions)
            {
                if (column.Constraints != null
                    && column.Constraints.OfType<UniqueConstraintDefinition>().Any())
                {
                    return true;
                }

                if (column.Index != null && column.Index.Unique)
                {
                    return true;
                }
            }

            return false;
        }

        private static string? GetTableKey(SchemaObjectName? name)
        {
            if (name?.Identifiers == null || name.Identifiers.Count == 0)
            {
                return null;
            }

            return string.Join(".", name.Identifiers.Select(identifier => identifier.Value));
        }

        private static bool IsTemporaryTable(SchemaObjectName? name)
        {
            var baseName = name?.BaseIdentifier?.Value;
            return !string.IsNullOrWhiteSpace(baseName)
                && baseName.StartsWith("#", StringComparison.Ordinal);
        }
    }
}


