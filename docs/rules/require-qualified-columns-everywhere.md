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

## Notes

- This rule extends [`qualified-select-columns`](qualified-select-columns.md) to cover WHERE/JOIN/ORDER BY clauses
- `qualified-select-columns` only applies to SELECT lists
- Enable both rules for comprehensive column qualification enforcement
- Or use only this rule if you want consistent qualification everywhere

