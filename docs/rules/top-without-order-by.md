# top-without-order-by

## Summary

Warns when a SELECT using `TOP` is missing an `ORDER BY` clause.

## Details

- Rule ID: `top-without-order-by`
- Severity: `Warning`
- Message: `TOP requires ORDER BY.`

Traverses `QuerySpecification` and reports a violation when all of the following conditions are met:

- `TOP` is specified (`TopRowFilter` exists)
- `ORDER BY` clause is absent (`OrderByClause` is not set)
- Exception: `TOP (1)` with a `WHERE` clause is excluded

When `TOP (1)` is used with a `WHERE` clause, it is excluded even without `ORDER BY`. This is because the result is constrained by the `WHERE` clause when retrieving a single row.

## Examples

### Invalid

```sql
SELECT TOP (5) col
FROM dbo.TableName;
```

### Valid

```sql
SELECT TOP (5) col
FROM dbo.TableName
ORDER BY col;
```

### Valid (TOP (1) with WHERE)

```sql
SELECT TOP (1) col
FROM dbo.TableName
WHERE id = 100;
```

### Valid (no TOP)

```sql
SELECT col
FROM dbo.TableName;
```
