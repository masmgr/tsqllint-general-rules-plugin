# avoid-null-comparison

## Summary

Prohibits using = or <> operators to compare values with NULL, which always evaluates to UNKNOWN in SQL logic.

## Details

- Rule ID: `avoid-null-comparison`
- Severity: `Warning`
- Message: `"Comparing with NULL using = or <> always evaluates to UNKNOWN. Use IS NULL or IS NOT NULL instead."`

Detects boolean comparison expressions where either side is NULL and the comparison operator is:
- `=` (equals)
- `<>` (not equal)
- `!=` (not equal - alternate syntax)

In SQL's three-valued logic, `NULL = NULL` evaluates to UNKNOWN (not TRUE), and `NULL <> value` also evaluates to UNKNOWN. This means the WHERE clause filter will not work as intended.

## Examples

### Invalid

```sql
-- These comparisons always evaluate to UNKNOWN, filtering nothing or unexpected rows
SELECT * FROM dbo.Users WHERE status = NULL;

SELECT * FROM dbo.Users WHERE email <> NULL;

SELECT * FROM dbo.Orders WHERE discount != NULL;

-- In JOIN conditions
SELECT *
FROM dbo.Users u
JOIN dbo.Orders o ON u.id = o.user_id
WHERE o.notes = NULL;

-- In CASE statements
SELECT
    CASE
        WHEN status = NULL THEN 'Missing'
        ELSE 'Present'
    END AS status_check
FROM dbo.Users;
```

### Valid

```sql
-- Use IS NULL for null checks
SELECT * FROM dbo.Users WHERE status IS NULL;

SELECT * FROM dbo.Users WHERE email IS NOT NULL;

SELECT * FROM dbo.Orders WHERE discount IS NOT NULL;

-- In JOIN conditions
SELECT *
FROM dbo.Users u
JOIN dbo.Orders o ON u.id = o.user_id
WHERE o.notes IS NULL;

-- In CASE statements
SELECT
    CASE
        WHEN status IS NULL THEN 'Missing'
        ELSE 'Present'
    END AS status_check
FROM dbo.Users;

-- Non-NULL comparisons are fine
SELECT * FROM dbo.Users WHERE status = 'Active';
SELECT * FROM dbo.Users WHERE age <> 0;
```

## Notes

- This rule applies to WHERE clauses, JOIN ON clauses, CASE statements, and anywhere boolean comparisons occur
- The three-valued logic in SQL means: `NULL = value` → UNKNOWN, `NULL <> value` → UNKNOWN, `NULL AND condition` → UNKNOWN or FALSE
- IS NULL and IS NOT NULL are the correct operators for null checks
- Using = NULL is a common bug that can silently cause incorrect query results
