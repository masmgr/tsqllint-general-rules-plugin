# Auto-fix Candidates and Implementation Plan

This note summarizes which rules in this repository (`tsqllint-plugin`) could reasonably support `FixViolation(...)` (auto-fix), and a proposed approach for implementing and validating those fixes.

## Current State

- For all rules under `TSQLLintGeneralRulesPlugin/Rules/*.cs`, `FixViolation(List<string> fileLines, IRuleViolation ruleViolation, FileLineActions actions)` is currently a **no-op (not implemented)**.
- Tests under `TSQLLintGeneralRulesPlugin.Tests` currently validate **detection (linting)** only; there is no test harness for applying and verifying fixes (only `TestSqlLintRunner.Lint(...)` exists today).

## Auto-fix Candidates (Prioritized)

### High confidence (generally safe, local edits)

- `require-as-for-column-alias`
    - Example: `SELECT col alias` → `SELECT col AS alias`
    - Can usually be implemented by inserting `AS ` immediately before the alias token.
- `require-as-for-table-alias`
    - Example: `FROM dbo.TableName t` → `FROM dbo.TableName AS t`
    - Same idea as above.
- `require-explicit-join-type`
    - If `INNER` is missing, insert `INNER ` before `JOIN`.
    - If `OUTER` is missing for `LEFT/RIGHT/FULL`, insert `OUTER ` before `JOIN`.
    - The join kind can be derived from the AST (`QualifiedJoinType`), and the change is localized.
- `avoid-null-comparison`
    - Example: `= NULL` → `IS NULL`, `<> NULL`/`!= NULL` → `IS NOT NULL`
    - The AST can identify the `NULL` literal side, enabling a targeted replacement.
- `order-by-in-subquery`
    - Remove the `ORDER BY` clause inside the offending subquery (when `TOP`/`OFFSET`/`FOR XML`/`FOR JSON` are not present).

### Medium to high difficulty (possible, but harder to implement/format safely)

- `require-begin-end-for-while`
    - Wrap the loop body statement with `BEGIN ... END`.
    - Tricky parts: statement boundaries, indentation, semicolons, and multi-line bodies.
- `require-begin-end-for-if-with-controlflow-exception`
    - Wrap IF/ELSE clauses with `BEGIN/END` (except single `RETURN`/`BREAK`/`CONTINUE`).
    - Tricky parts: ELSE handling and nested IF patterns.
- `prefer-concat-over-plus-when-nullable-or-convert`
    - Replace an outer `+` concatenation chain with `CONCAT(...)`.
    - Tricky parts: operand collection, parentheses, and potential behavior/typing differences.
- `avoid-atat-identity`
    - `@@IDENTITY` → `SCOPE_IDENTITY()`
    - Simple text replacement, but it can change semantics (scope of the identity value), so adopting auto-fix may require a policy decision.

### Generally not suitable for auto-fix (insufficient info / high behavioral impact)

- `require-column-list-for-insert-values` / `require-column-list-for-insert-select`
    - Generating a correct column list requires table schema; SQL text alone cannot reliably determine it.
- `top-without-order-by`
    - A correct `ORDER BY` is a design decision and cannot be safely inferred.
- DDL design rules such as `avoid-heap-table` / `require-primary-key-or-unique-constraint`
    - Choosing keys/indexes is a schema design decision.
- `avoid-magic-convert-style-for-datetime` / `avoid-ambiguous-datetime-literal`
    - There is no single universally correct replacement format; fixes become subjective.
- `avoid-nolock` / `require-xact-abort-on` / `require-try-catch-for-transaction`
    - Fixes can materially change runtime behavior (locking/transactions/error handling).

## Proposed Implementation Approach

1. Confirm the `TSQLLint.Common` API surface for `IRuleViolation` and `FileLineActions`
    - Determine what edit operations are available (insert/replace/delete and their granularity). If needed, decompile the DLL.
2. Establish a common `FixViolation` pattern
    - Ideally: re-parse SQL with ScriptDom → map `ruleViolation` line/column to a node/token span → apply edits via `actions`.
3. Start with “insertion-only” rules
    - Recommended order: `require-as-for-column-alias` → `require-as-for-table-alias` → `require-explicit-join-type`
4. Expand to “replace/delete” rules
    - `avoid-null-comparison`, then `order-by-in-subquery`
5. Extend tests to validate fixes
    - Add a helper to `TestSqlLintRunner` for “apply fix → re-lint → assert no violations”.
    - For each rule: pair “detects violation” with “fix removes violation”.
6. Update rule docs (`docs/rules/*.md`) with auto-fix support status
    - Add a consistent note such as `Auto-fix: supported / unsupported` in Summary/Notes.
