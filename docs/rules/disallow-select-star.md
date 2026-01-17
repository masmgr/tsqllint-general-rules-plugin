# disallow-select-star

## Summary

Disallows `SELECT *` (including `t.*`) to enforce explicit column lists.

## Details

- Rule ID: `disallow-select-star`
- Severity: `Warning`
- Message: `Avoid SELECT *; explicitly list columns to prevent breaking changes and improve readability.`

## Examples

### Invalid

```sql
SELECT * FROM dbo.Customer;
```

### Valid

```sql
SELECT CustomerId, Name FROM dbo.Customer;
```

