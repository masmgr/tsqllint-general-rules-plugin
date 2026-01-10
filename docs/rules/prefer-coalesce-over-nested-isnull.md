# prefer-coalesce-over-nested-isnull

## Summary

Detects nested `ISNULL` calls and recommends using `COALESCE`.

## Details

- Rule ID: `prefer-coalesce-over-nested-isnull`
- Severity: `Warning`
- Message: `Use COALESCE instead of nested ISNULL calls.`

Traverses `FunctionCall` and reports a violation when the function name is `ISNULL` and its arguments contain another `ISNULL(...)` call.

## Examples

### Invalid

```sql
SELECT ISNULL(ISNULL(a, b), c)
FROM dbo.TableName;
```

### Valid

```sql
SELECT COALESCE(a, b, c)
FROM dbo.TableName;
```

### Valid (single ISNULL)

```sql
SELECT ISNULL(a, b)
FROM dbo.TableName;
```

## Notes

- This rule determines if the function is nested. A single `ISNULL(a, b)` is not subject to this rule.
