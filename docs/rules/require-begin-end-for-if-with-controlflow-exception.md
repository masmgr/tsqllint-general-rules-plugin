# require-begin-end-for-if-with-controlflow-exception

## Summary

Requires `BEGIN/END` blocks for `IF` and `ELSE` clauses but permits naked control-flow statements when a clause contains only that statement.

## Details

- Rule ID: `require-begin-end-for-if-with-controlflow-exception`
- Severity: `Warning`
- Message: `Wrap IF/ELSE clauses in BEGIN/END unless they consist of a single RETURN, BREAK, or CONTINUE.`
- AST nodes: `IfStatement`, `BeginEndBlock`, `ReturnStatement`, `BreakStatement`, `ContinueStatement`

Violations are reported when the `THEN` or `ELSE` body is not a `BEGIN/END` block and it is more than a single allowed control-flow statement.

## Examples

### Invalid

```sql
IF @flag = 1
    SET @x = 1;
ELSE
    SET @x = 0;
```

### Valid (block)

```sql
IF @flag = 1
BEGIN
    SET @x = 1;
END
ELSE
BEGIN
    SET @x = 0;
END
```

### Valid (control-flow exception)

```sql
IF @flag = 1
    RETURN;
ELSE
    BREAK;
```

## Notes

- The exception is intentionally narrow; only `RETURN`, `BREAK`, and `CONTINUE` may avoid the block.
- `ELSE IF` chains are evaluated independently, so each nested `IF` is subject to the same rule.
