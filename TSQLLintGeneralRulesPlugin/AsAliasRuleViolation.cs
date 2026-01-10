using TSQLLint.Common;

namespace TSQLLintGeneralRulesPlugin
{
    /// <summary>
    /// Represents an <see cref="IRuleViolation"/> implementation for alias specification rule violations involving the <c>AS</c> keyword.
    /// </summary>
    public sealed class AsAliasRuleViolation : IRuleViolation
    {
        /// <summary>
        /// Gets or sets the column number (1-based) where the rule violation occurred.
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// Gets or sets the file name where the rule violation occurred.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Gets or sets the line number (1-based) where the rule violation occurred.
        /// </summary>
        public int Line { get; set; }

        /// <summary>
        /// Gets or sets the name of the rule that was violated.
        /// </summary>
        public string RuleName { get; private set; }

        /// <summary>
        /// Gets or sets the severity of the rule violation.
        /// </summary>
        public RuleViolationSeverity Severity { get; private set; }

        /// <summary>
        /// Gets or sets the violation message.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsAliasRuleViolation"/> class.
        /// </summary>
        /// <param name="fileName">The file name where the violation occurred.</param>
        /// <param name="ruleName">The name of the rule that was violated.</param>
        /// <param name="text">The violation message.</param>
        /// <param name="lineNumber">The line number (1-based) where the violation occurred.</param>
        /// <param name="column">The column number (1-based) where the violation occurred.</param>
        /// <param name="ruleViolationSeverity">The severity of the violation.</param>
        public AsAliasRuleViolation(
            string fileName,
            string ruleName,
            string text,
            int lineNumber,
            int column,
            RuleViolationSeverity ruleViolationSeverity)
        {
            FileName = fileName;
            RuleName = ruleName;
            Text = text;
            Line = lineNumber;
            Column = column;
            Severity = ruleViolationSeverity;
        }
    }
}
