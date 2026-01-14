# prefer-concat-over-plus

## Summary

Recommends `CONCAT()` when `+`-based string concatenation uses `ISNULL`/`COALESCE`.

## Details

- Rule ID: `prefer-concat-over-plus`
- Severity: `Warning`
- Message: `Prefer CONCAT() for string concatenation when using ISNULL/COALESCE; it avoids NULL-propagation surprises and improves readability.`

## Examples

### Invalid

```sql
SELECT ISNULL(@a, '') + COALESCE(@b, '');
```

### Valid

```sql
SELECT CONCAT(ISNULL(@a, ''), COALESCE(@b, ''));
```

## Notes

- Detection requires a string literal somewhere in the expression to reduce false positives in purely arithmetic `+` expressions.

