using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin
{
    /// <summary>
    /// Warns when CREATE TABLE defines a heap (no clustered index).
    /// </summary>
    public sealed class AvoidHeapTableRule : TSqlFragmentVisitor, ISqlLintRule
    {
        private readonly Action<string, string, int, int> _errorCallback;
        private readonly Dictionary<string, List<CreateTableStatement>> _createTables =
            new(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _tablesWithClusteredIndex =
            new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initializes the rule.
        /// </summary>
        /// <param name="errorCallback">Callback invoked when a violation is detected.</param>
        public AvoidHeapTableRule(Action<string, string, int, int> errorCallback)
        {
            _errorCallback = errorCallback;
        }

        /// <summary>
        /// Gets the rule ID.
        /// </summary>
        public string RULE_NAME => "avoid-heap-table";

        /// <summary>
        /// Gets the violation message.
        /// </summary>
        public string RULE_TEXT => "Avoid heap tables by defining a clustered index (or clustered primary key).";

        /// <summary>
        /// Gets the violation severity.
        /// </summary>
        public RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

        /// <summary>
        /// Visits CREATE TABLE statements and collects inline clustered indexes.
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

                if (HasClusteredIndex(node.Definition))
                {
                    _tablesWithClusteredIndex.Add(tableKey);
                }
            }

            base.Visit(node);
        }

        /// <summary>
        /// Visits ALTER TABLE statements to collect clustered indexes or constraints.
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
            if (!string.IsNullOrWhiteSpace(tableKey) && HasClusteredIndex(node.Definition))
            {
                _tablesWithClusteredIndex.Add(tableKey);
            }

            base.Visit(node);
        }

        /// <summary>
        /// Visits CREATE INDEX statements to collect clustered indexes.
        /// </summary>
        /// <param name="node">The CREATE INDEX statement.</param>
        public override void Visit(CreateIndexStatement node)
        {
            if (node == null || node.Clustered != true)
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
                _tablesWithClusteredIndex.Add(tableKey);
            }

            base.Visit(node);
        }

        /// <summary>
        /// Visits CREATE COLUMNSTORE INDEX statements to collect clustered columnstore indexes.
        /// </summary>
        /// <param name="node">The CREATE COLUMNSTORE INDEX statement.</param>
        public override void Visit(CreateColumnStoreIndexStatement node)
        {
            if (node == null || node.Clustered != true)
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
                _tablesWithClusteredIndex.Add(tableKey);
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
                if (_tablesWithClusteredIndex.Contains(entry.Key))
                {
                    continue;
                }

                foreach (var createTable in entry.Value)
                {
                    var location = createTable.SchemaObjectName?.BaseIdentifier ?? (TSqlFragment)createTable;
                    _errorCallback?.Invoke(RULE_NAME, RULE_TEXT, location.StartLine, location.StartColumn);
                }
            }
        }

        /// <summary>
        /// Automatically fixes rule violations (no automatic fix is provided for this rule).
        /// </summary>
        /// <param name="fileLines">Array of lines in the file.</param>
        /// <param name="ruleViolation">The rule violation information.</param>
        /// <param name="actions">Line edit actions.</param>
        public void FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)
        {
            // No automatic fix is provided for this rule.
        }

        private static bool HasClusteredIndex(TableDefinition? definition)
        {
            if (definition == null)
            {
                return false;
            }

            if (definition.Indexes != null && definition.Indexes.Any(IsClusteredIndex))
            {
                return true;
            }

            if (definition.TableConstraints != null
                && definition.TableConstraints.OfType<UniqueConstraintDefinition>().Any(IsClusteredConstraint))
            {
                return true;
            }

            if (definition.ColumnDefinitions == null)
            {
                return false;
            }

            foreach (var column in definition.ColumnDefinitions)
            {
                if (column.Index != null && IsClusteredIndex(column.Index))
                {
                    return true;
                }

                if (column.Constraints != null
                    && column.Constraints.OfType<UniqueConstraintDefinition>().Any(IsClusteredConstraint))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsClusteredConstraint(UniqueConstraintDefinition constraint)
        {
            if (constraint.Clustered.HasValue)
            {
                return constraint.Clustered.Value;
            }

            if (constraint.IsPrimaryKey)
            {
                return true;
            }

            return IsClusteredIndexType(constraint.IndexType);
        }

        private static bool IsClusteredIndex(IndexDefinition index)
        {
            return IsClusteredIndexType(index.IndexType);
        }

        private static bool IsClusteredIndexType(IndexType? indexType)
        {
            var kind = indexType?.IndexTypeKind;
            return kind == IndexTypeKind.Clustered || kind == IndexTypeKind.ClusteredColumnStore;
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
