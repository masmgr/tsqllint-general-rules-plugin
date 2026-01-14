# avoid-implicit-conversion-in-predicate

## Summary

Warns when predicates apply conversions to column expressions (a common cause of implicit conversions and scans).

## Details

- Rule ID: `avoid-implicit-conversion-in-predicate`
- Severity: `Warning`
- Message: `Avoid conversions on columns in WHERE/JOIN predicates (e.g., CAST(col AS ...) = ...); they can force scans by preventing index seeks. Prefer correctly typed parameters/literals instead.`

## Examples

### Invalid

```sql
WHERE CAST(o.CreatedAt AS date) = @d
```

### Valid

```sql
WHERE o.CreatedAt >= @d AND o.CreatedAt < DATEADD(day, 1, @d)
```

