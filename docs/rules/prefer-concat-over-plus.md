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
- This rule focuses on NULL-handling functions (`ISNULL`/`COALESCE`) used in string concatenation
- For broader detection that includes type conversion functions (`CONVERT`/`CAST`), see [`prefer-concat-over-plus-when-nullable-or-convert`](prefer-concat-over-plus-when-nullable-or-convert.md)
- You should enable one or the other, not both, to avoid redundant warnings

