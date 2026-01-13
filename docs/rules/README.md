# Rules List

This is a list of rules provided by this plugin.

## Code Style & Naming Conventions

- [`require-as-for-column-alias`](require-as-for-column-alias.md): Requires `AS` for column aliases.
- [`require-as-for-table-alias`](require-as-for-table-alias.md): Requires `AS` for table/derived table aliases.

## Control Flow Safety

- [`require-begin-end-for-while`](require-begin-end-for-while.md): Enforces `BEGIN/END` for `WHILE` loop bodies.
- [`require-begin-end-for-if-with-controlflow-exception`](require-begin-end-for-if-with-controlflow-exception.md): Enforces blocks for `IF`/`ELSE` clauses unless they contain a single control-flow statement.

## Query Structure & Clarity

- [`qualified-select-columns`](qualified-select-columns.md): Disallows unqualified column references in SELECT lists when multiple tables are referenced.
- [`require-explicit-join-type`](require-explicit-join-type.md): Disallows omitting INNER/OUTER keywords in JOIN clauses.
- [`require-column-list-for-insert-select`](require-column-list-for-insert-select.md): Requires column list for `INSERT INTO ... SELECT`.
- [`require-column-list-for-insert-values`](require-column-list-for-insert-values.md): Requires column list for `INSERT INTO ... VALUES`.
- [`order-by-in-subquery`](order-by-in-subquery.md): Disallows `ORDER BY` usage in subqueries.

## Schema Design

- [`require-primary-key-or-unique-constraint`](require-primary-key-or-unique-constraint.md): Requires a PRIMARY KEY or UNIQUE constraint for user tables.
- [`avoid-heap-table`](avoid-heap-table.md): Warns when tables are created without a clustered index (heap tables).
- [`require-ms-description-for-table-definition-file`](require-ms-description-for-table-definition-file.md): Encourages tables to declare an `MS_Description` extended property alongside their definition.

## Performance & Correctness

- [`top-without-order-by`](top-without-order-by.md): Requires `ORDER BY` when using `TOP` in SELECT.
- [`avoid-top-in-dml`](avoid-top-in-dml.md): Disallows `TOP` in UPDATE/DELETE statements (non-deterministic behavior).
- [`prefer-coalesce-over-nested-isnull`](prefer-coalesce-over-nested-isnull.md): Detects nested `ISNULL` and recommends `COALESCE`.
- [`avoid-atat-identity`](avoid-atat-identity.md): Disallows `@@IDENTITY` and recommends `SCOPE_IDENTITY()`/`OUTPUT`.
- [`avoid-null-comparison`](avoid-null-comparison.md): Detects `= NULL` / `<> NULL` comparisons and recommends `IS NULL` / `IS NOT NULL`.
- [`avoid-ambiguous-datetime-literal`](avoid-ambiguous-datetime-literal.md): Detects slash-delimited date literals and recommends ISO 8601 format.

## Security

- [`avoid-exec-dynamic-sql`](avoid-exec-dynamic-sql.md): Warns about dynamic SQL `EXEC` execution and recommends `sp_executesql` with parameters.
- [`avoid-merge`](avoid-merge.md): Disallows `MERGE` statements due to known issues and recommends separate INSERT/UPDATE/DELETE.

## Data Access & Isolation

- [`avoid-nolock`](avoid-nolock.md): Disallows `NOLOCK` hint and `READ UNCOMMITTED` isolation level (allows dirty reads).

## Transaction Safety

- [`require-try-catch-for-transaction`](require-try-catch-for-transaction.md): Requires `TRY/CATCH` blocks around explicit transactions for proper error handling.
- [`require-xact-abort-on`](require-xact-abort-on.md): Documents the need for `SET XACT_ABORT ON` with explicit transactions to ensure consistent rollback behavior.

## Functions & Built-in Utilities

- [`avoid-magic-convert-style-for-datetime`](avoid-magic-convert-style-for-datetime.md): Detects `CONVERT` with style numbers for datetime conversion and warns on magic numbers in favor of clearer alternatives.
- [`prefer-concat-over-plus-when-nullable-or-convert`](prefer-concat-over-plus-when-nullable-or-convert.md): Encourages `CONCAT` when `ISNULL`/`CONVERT`/`CAST` are mixed into `+` string building.
