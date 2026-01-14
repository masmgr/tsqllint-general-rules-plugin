# require-qualified-columns-everywhere

## Summary

Requires column qualification in `WHERE` / `JOIN` / `ORDER BY` when a query references multiple tables.

## Details

- Rule ID: `require-qualified-columns-everywhere`
- Severity: `Warning`
- Message: `Unqualified column reference in a multi-table query. Qualify it with a table alias (e.g., t.id) in WHERE/JOIN/ORDER BY to avoid ambiguity.`

## Examples

### Invalid

```sql
SELECT a.CustomerId
FROM dbo.Customer a
JOIN dbo.[Order] b ON a.CustomerId = b.CustomerId
WHERE CustomerId = 1
ORDER BY CreatedAt;
```

### Valid

```sql
SELECT a.CustomerId
FROM dbo.Customer a
JOIN dbo.[Order] b ON a.CustomerId = b.CustomerId
WHERE a.CustomerId = 1
ORDER BY b.CreatedAt;
```

