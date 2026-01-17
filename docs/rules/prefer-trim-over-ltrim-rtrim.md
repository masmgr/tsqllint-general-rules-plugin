# prefer-trim-over-ltrim-rtrim

## Summary

Recommends `TRIM(x)` instead of nested `LTRIM(RTRIM(x))` or `RTRIM(LTRIM(x))`.

## Details

- Rule ID: `prefer-trim-over-ltrim-rtrim`
- Severity: `Warning`
- Message: `Prefer TRIM(x) over nested LTRIM(RTRIM(x)) for readability and standardization.`

## Examples

### Invalid

```sql
SELECT LTRIM(RTRIM(@s));
```

### Valid

```sql
SELECT TRIM(@s);
```

