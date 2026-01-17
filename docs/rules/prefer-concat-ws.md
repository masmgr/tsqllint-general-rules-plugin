# prefer-concat-ws

## Summary

Recommends `CONCAT_WS(separator, ...)` when `+`-based string concatenation repeats the same separator literal.

## Details

- Rule ID: `prefer-concat-ws`
- Severity: `Warning`
- Message: `Prefer CONCAT_WS(separator, ...) over + when concatenating values with repeated separators.`

## Examples

### Invalid

```sql
SELECT ISNULL(@a, '') + ',' + ISNULL(@b, '') + ',' + COALESCE(@c, '');
```

### Valid

```sql
SELECT CONCAT_WS(',', @a, @b, @c);
```

