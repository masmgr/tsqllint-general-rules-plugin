# prefer-try-convert-patterns

## Summary

Encourages `TRY_CONVERT`/`TRY_CAST` instead of `CASE` expressions gated by `ISNUMERIC`/`ISDATE`.

## Details

- Rule ID: `prefer-try-convert-patterns`
- Severity: `Warning`
- Message: `Prefer TRY_CONVERT/TRY_CAST over CASE WHEN ISNUMERIC/ISDATE(...) THEN CONVERT/CAST(...) patterns for clarity and correctness.`

## Examples

### Invalid

```sql
SELECT CASE WHEN ISNUMERIC(@x) = 1 THEN CONVERT(int, @x) END;
```

### Valid

```sql
SELECT TRY_CONVERT(int, @x);
```

