# forbid-top-100-percent-order-by

## Summary

Forbids `TOP 100 PERCENT ORDER BY`, which is redundant and often ignored.

## Details

- Rule ID: `forbid-top-100-percent-order-by`
- Severity: `Warning`
- Message: `Avoid TOP 100 PERCENT ORDER BY; it is redundant and often ignored by the optimizer. Remove TOP 100 PERCENT or move ORDER BY to the outer query.`

## Examples

### Invalid

```sql
SELECT TOP 100 PERCENT *
FROM dbo.Customer
ORDER BY Name;
```

### Valid

```sql
SELECT *
FROM dbo.Customer
ORDER BY Name;
```

