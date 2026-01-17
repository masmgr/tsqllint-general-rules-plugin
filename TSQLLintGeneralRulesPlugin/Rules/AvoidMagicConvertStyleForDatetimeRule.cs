using System;
using System.Collections.Generic;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin
{
    /// <summary>
    /// Detects styled <c>CONVERT</c> calls when converting datetime to string and recommends avoiding magic style numbers.
    /// </summary>
    public sealed class AvoidMagicConvertStyleForDatetimeRule : SqlLintRuleBase
    {

        /// <summary>
        /// Initializes the rule.
        /// </summary>
        /// <param name="errorCallback">Callback invoked when a violation is detected.</param>
        public AvoidMagicConvertStyleForDatetimeRule(Action<string, string, int, int> errorCallback) : base(errorCallback)
        {
        }

        /// <summary>
        /// Gets the rule ID.
        /// </summary>
        public override string RULE_NAME => "avoid-magic-convert-style-for-datetime";

        /// <summary>
        /// Gets the violation message.
        /// </summary>
        public override string RULE_TEXT => "Avoid CONVERT with style numbers for datetime formatting. Prefer ISO-style conversions or date types; reserve FORMAT() for small UI output with explicit culture.";

        /// <summary>
        /// Gets the violation severity.
        /// </summary>
        public override RuleViolationSeverity RULE_SEVERITY => RuleViolationSeverity.Warning;

        /// <summary>
        /// Traverses CONVERT calls and reports violations when a style number is used to convert to a string type.
        /// </summary>
        /// <param name="node">The CONVERT call node to visit.</param>
        public override void Visit(ConvertCall node)
        {
            if (node == null)
            {
                base.Visit(node);
                return;
            }

            // Check if Style parameter exists (3rd argument)
            if (node.Style == null)
            {
                base.Visit(node);
                return;
            }

            // Check if target DataType is a string type
            if (!IsStringDataType(node.DataType))
            {
                base.Visit(node);
                return;
            }

            // Report violation
            ReportViolation(node.StartLine, node.StartColumn);

            base.Visit(node);
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

        /// <summary>
        /// Determines whether the data type is a string type.
        /// </summary>
        /// <param name="dataType">The data type reference to check.</param>
        /// <returns>true if the data type is a string type, false otherwise.</returns>
        private static bool IsStringDataType(DataTypeReference? dataType)
        {
            if (dataType is not SqlDataTypeReference sqlDataType)
            {
                return false;
            }

            return sqlDataType.SqlDataTypeOption switch
            {
                SqlDataTypeOption.VarChar => true,
                SqlDataTypeOption.NVarChar => true,
                SqlDataTypeOption.Char => true,
                SqlDataTypeOption.NChar => true,
                _ => false
            };
        }
    }
}


