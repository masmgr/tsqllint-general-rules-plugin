# order-by-in-subquery

## Summary

Disallows `ORDER BY` usage in subqueries. SQL Server raises a runtime error (error 46047) when this occurs, so this linter detects it early.

## Details

- Rule ID: `order-by-in-subquery`
- Severity: `Error`
- Message: `ORDER BY in subquery without TOP, OFFSET, or FOR XML is not allowed.`

Traverses `QuerySpecification` and reports a violation when all of the following conditions are met:

- Inside a subquery (nesting level 2 or higher)
- `ORDER BY` clause exists (`OrderByClause` is present)
- None of the following are true:
  - `TOP` is specified (`TopRowFilter` exists)
  - `OFFSET` is specified (`OffsetClause` exists)
  - `FOR XML` clause is present

## Rationale

In SQL Server, when using `ORDER BY` in a subquery, one of the following is required:

- `TOP` - to limit the result set
- `OFFSET/FETCH` - to implement pagination
- `FOR XML` - to output in XML format

Without these, SQL Server raises a runtime error. This rule detects such errors early during the linting phase.

## Examples

### Invalid

This query raises a runtime error in SQL Server:

```sql
SELECT *
FROM (
  SELECT id
  FROM dbo.TableName
  ORDER BY id
) AS sub;
```

Result: Error 46047 - "The ORDER BY clause is invalid in views, inline functions, derived tables, subqueries, and common table expressions, unless TOP, OFFSET or FOR XML is also specified."

### Valid (TOP)

```sql
SELECT *
FROM (
  SELECT TOP (10) id
  FROM dbo.TableName
  ORDER BY id
) AS sub;
```

### Valid (OFFSET/FETCH)

```sql
SELECT *
FROM (
  SELECT id
  FROM dbo.TableName
  ORDER BY id
  OFFSET 10 ROWS
  FETCH NEXT 5 ROWS ONLY
) AS sub;
```

### Valid (FOR XML)

```sql
SELECT (
  SELECT id
  FROM dbo.TableName
  ORDER BY id
  FOR XML PATH('')
) AS XmlResult;
```

### Valid (no ORDER BY in subquery)

```sql
SELECT *
FROM (
  SELECT id
  FROM dbo.TableName
) AS sub
ORDER BY id;
```

## Related Rules

- [`top-without-order-by`](top-without-order-by.md) - Requires `ORDER BY` when using `TOP` in SELECT
