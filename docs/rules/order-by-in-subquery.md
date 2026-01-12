# order-by-in-subquery

## Summary

Disallows `ORDER BY` usage in subqueries. SQL Server returns an error (Msg 1033) when this occurs, so this linter detects it early.

## Details

- Rule ID: `order-by-in-subquery`
- Severity: `Error`
- Message: `ORDER BY in subquery without TOP, OFFSET, FOR XML, or FOR JSON is not allowed.`

Traverses `QuerySpecification` and reports a violation when all of the following conditions are met:

- Inside a non-top-level SELECT (e.g., derived table, CTE, or scalar subquery)
- `ORDER BY` clause exists (`OrderByClause` is present)
- None of the following are true:
  - `TOP` is specified (`TopRowFilter` exists)
  - `OFFSET` is specified (`OffsetClause` exists)
  - `FOR XML` clause is present
  - `FOR JSON` clause is present

## Rationale

In SQL Server, when using `ORDER BY` in a subquery, one of the following is required:

- `TOP` - to limit the result set
- `OFFSET/FETCH` - to implement pagination
- `FOR XML` - to output in XML format
- `FOR JSON` - to output in JSON format

Without these, SQL Server returns an error (Msg 1033). This rule detects such errors early during the linting phase.

Even when allowed, `ORDER BY` in subqueries only determines which rows are selected for `TOP`/`OFFSET`/`FOR XML`/`FOR JSON` and does not guarantee the final output order; use an outer `ORDER BY` for that.

## Examples

### Invalid

This query returns an error in SQL Server:

```sql
SELECT *
FROM (
  SELECT id
  FROM dbo.TableName
  ORDER BY id
) AS sub;
```

Result: Msg 1033 - "The ORDER BY clause is invalid in views, inline functions, derived tables, subqueries, and common table expressions, unless TOP, OFFSET or FOR XML is also specified."

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

### Valid (FOR JSON)

```sql
SELECT (
  SELECT id
  FROM dbo.TableName
  ORDER BY id
  FOR JSON PATH
) AS JsonResult;
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
