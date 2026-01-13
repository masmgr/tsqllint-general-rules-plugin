# require-begin-end-for-while

## Summary

Enforces a `BEGIN/END` block for every `WHILE` loop body to prevent accidental single-statement semantics.

## Details

- Rule ID: `require-begin-end-for-while`
- Severity: `Warning`
- Message: `WHILE loop bodies must be wrapped in BEGIN/END to avoid accidental single-statement traps.`
- AST nodes: `WhileStatement`

Reports a violation whenever a `WHILE` statement immediately executes a single statement without an enclosing `BEGIN/END` block.

## Examples

### Invalid

```sql
WHILE @i < 10
    SET @i += 1;
```

### Valid

```sql
WHILE @i < 10
BEGIN
    SET @i += 1;
END
```

## Notes

- This rule does not whitelist single statements such as `PRINT` or `SET`; every logical block should be explicit.
