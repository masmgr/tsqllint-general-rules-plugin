# meaningful-alias

## Summary

Warns on single-character table aliases in multi-table queries.

## Details

- Rule ID: `meaningful-alias`
- Severity: `Warning`
- Message: `Avoid single-character table aliases in multi-table queries; use meaningful aliases to reduce ambiguity (e.g., c for Customer is okay only when context is obvious).`

## Examples

### Invalid

```sql
SELECT c.CustomerId
FROM dbo.Customer c
JOIN dbo.[Order] o ON c.CustomerId = o.CustomerId;
```

### Valid

```sql
SELECT cust.CustomerId
FROM dbo.Customer cust
JOIN dbo.[Order] ord ON cust.CustomerId = ord.CustomerId;
```

