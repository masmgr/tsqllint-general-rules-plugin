# require-parentheses-for-mixed-and-or

## Summary

Requires parentheses when `AND` and `OR` are mixed in a predicate, to make precedence explicit.

## Details

- Rule ID: `require-parentheses-for-mixed-and-or`
- Severity: `Warning`
- Message: `When mixing AND and OR, add parentheses to make operator precedence explicit and avoid misreads.`

## Examples

### Invalid

```sql
WHERE A = 1 AND B = 2 OR C = 3
```

### Valid

```sql
WHERE (A = 1 AND B = 2) OR C = 3
```

