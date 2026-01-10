using System.Collections.Generic;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin
{
    /// <summary>
    /// A plugin that registers general-purpose rules with TSQLLint.
    /// </summary>
    public sealed class SqlPlugin : IPlugin
    {
        /// <summary>
        /// Performs additional processing (no-op for this plugin).
        /// </summary>
        /// <param name="context">The execution context provided by TSQLLint.</param>
        /// <param name="reporter">The reporter provided by TSQLLint.</param>
        public void PerformAction(IPluginContext context, IReporter reporter)
        {
            // Intentionally no-op when using rules returned by GetRules.
        }

        /// <summary>
        /// Gets a dictionary mapping rule IDs to rule implementations.
        /// </summary>
        /// <returns>A dictionary with rule IDs as keys and rule implementations as values.</returns>
        public IDictionary<string, ISqlLintRule> GetRules() => new Dictionary<string, ISqlLintRule>
        {
            ["require-as-for-column-alias"] = new RequireAsForColumnAliasRule(null!),
            ["require-as-for-table-alias"] = new RequireAsForTableAliasRule(null!),
            ["require-explicit-join-type"] = new RequireExplicitJoinTypeRule(null!),
            ["qualified-select-columns"] = new RequireQualifiedSelectColumnsRule(null!),
            ["prefer-coalesce-over-nested-isnull"] = new PreferCoalesceOverNestedIsNullRule(null!),
            ["require-column-list-for-insert-select"] = new RequireColumnListForInsertSelectRule(null!),
            ["top-without-order-by"] = new TopWithoutOrderByRule(null!),
            ["avoid-atat-identity"] = new AvoidAtAtIdentityRule(null!),
            ["avoid-exec-dynamic-sql"] = new AvoidExecDynamicSqlRule(null!),
            ["order-by-in-subquery"] = new OrderByInSubqueryRule(null!),
            ["prefer-format-or-date-for-datetime-conversion"] = new PreferFormatOrDateForDatetimeConversionRule(null!),
            ["avoid-merge"] = new AvoidMergeRule(null!),
            ["avoid-top-in-dml"] = new AvoidTopInDmlRule(null!),
            ["require-column-list-for-insert-values"] = new RequireColumnListForInsertValuesRule(null!),
            ["avoid-nolock"] = new AvoidNolockRule(null!),
            ["avoid-null-comparison"] = new AvoidNullComparisonRule(null!),
            ["avoid-ambiguous-datetime-literal"] = new AvoidAmbiguousDatetimeLiteralRule(null!),
            ["require-try-catch-for-transaction"] = new RequireTryCatchForTransactionRule(null!),
            ["require-xact-abort-on"] = new RequireXactAbortOnRule(null!)
        };
    }
}
