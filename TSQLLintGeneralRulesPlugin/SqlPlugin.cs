using System;
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
        public IDictionary<string, ISqlLintRule> GetRules()
        {
            var rules = new Dictionary<string, ISqlLintRule>(StringComparer.Ordinal);

            foreach (var ruleFactory in RuleFactories)
            {
                var rule = ruleFactory(null!);
                rules.Add(rule.RULE_NAME, rule);
            }

            return rules;
        }

        private static readonly Func<Action<string, string, int, int>, ISqlLintRule>[] RuleFactories =
        {
            errorCallback => new PreferStringAggOverStuffRule(errorCallback),
            errorCallback => new PreferConcatOverPlusRule(errorCallback),
            errorCallback => new PreferConcatWsRule(errorCallback),
            errorCallback => new PreferTrimOverLtrimRtrimRule(errorCallback),
            errorCallback => new PreferJsonFunctionsRule(errorCallback),
            errorCallback => new PreferTryConvertPatternsRule(errorCallback),
            errorCallback => new DisallowSelectStarRule(errorCallback),
            errorCallback => new RequireQualifiedColumnsEverywhereRule(errorCallback),
            errorCallback => new RequireParenthesesForMixedAndOrRule(errorCallback),
            errorCallback => new MeaningfulAliasRule(errorCallback),
            errorCallback => new ForbidTop100PercentOrderByRule(errorCallback),
            errorCallback => new DisallowSelectIntoRule(errorCallback),
            errorCallback => new AvoidImplicitConversionInPredicateRule(errorCallback),
            errorCallback => new AvoidNolockOrReadUncommittedRule(errorCallback),
            errorCallback => new RequireAsForColumnAliasRule(errorCallback),
            errorCallback => new RequireAsForTableAliasRule(errorCallback),
            errorCallback => new RequireExplicitJoinTypeRule(errorCallback),
            errorCallback => new RequireQualifiedSelectColumnsRule(errorCallback),
            errorCallback => new RequireBeginEndForIfWithControlFlowExceptionRule(errorCallback),
            errorCallback => new PreferCoalesceOverNestedIsNullRule(errorCallback),
            errorCallback => new RequireBeginEndForWhileRule(errorCallback),
            errorCallback => new RequireColumnListForInsertSelectRule(errorCallback),
            errorCallback => new TopWithoutOrderByRule(errorCallback),
            errorCallback => new AvoidAtAtIdentityRule(errorCallback),
            errorCallback => new AvoidExecDynamicSqlRule(errorCallback),
            errorCallback => new OrderByInSubqueryRule(errorCallback),
            errorCallback => new AvoidMagicConvertStyleForDatetimeRule(errorCallback),
            errorCallback => new AvoidMergeRule(errorCallback),
            errorCallback => new AvoidTopInDmlRule(errorCallback),
            errorCallback => new RequireColumnListForInsertValuesRule(errorCallback),
            errorCallback => new AvoidNolockRule(errorCallback),
            errorCallback => new AvoidNullComparisonRule(errorCallback),
            errorCallback => new AvoidAmbiguousDatetimeLiteralRule(errorCallback),
            errorCallback => new PreferConcatOverPlusWhenNullableOrConvertRule(errorCallback),
            errorCallback => new RequireTryCatchForTransactionRule(errorCallback),
            errorCallback => new RequireXactAbortOnRule(errorCallback),
            errorCallback => new RequirePrimaryKeyOrUniqueConstraintRule(errorCallback),
            errorCallback => new RequireMsDescriptionForTableDefinitionFileRule(errorCallback),
            errorCallback => new AvoidHeapTableRule(errorCallback)
        };
    }
}
