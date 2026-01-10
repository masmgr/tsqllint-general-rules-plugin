# avoid-top-in-dml

## Summary

Prohibits the use of TOP clause in UPDATE and DELETE statements, which can produce non-deterministic results when not paired with ORDER BY.

## Details

- Rule ID: `avoid-top-in-dml`
- Severity: `Warning`
- Message: `"TOP in UPDATE/DELETE statements without ORDER BY can produce non-deterministic results. Consider using a CTE with ORDER BY or specific WHERE clause."`

Traverses UPDATE and DELETE statements and flags any TopRowFilter found in their specifications. Using TOP in DML statements without an ORDER BY clause can result in unpredictable behavior, as the rows to be affected are not deterministically selected.

## Examples

### Invalid

```sql
-- Unpredictable which users get updated
UPDATE TOP (10) dbo.Users
SET name = 'Updated'
WHERE status = 'inactive';

-- Unpredictable which records are deleted
DELETE TOP (100) FROM dbo.AuditLog
WHERE createdDate < DATEADD(YEAR, -1, GETDATE());
```

### Valid

```sql
-- Use CTE with ORDER BY for deterministic behavior
WITH CTE AS (
    SELECT TOP (10) id
    FROM dbo.Users
    WHERE status = 'inactive'
    ORDER BY lastModified ASC
)
UPDATE u
SET name = 'Updated'
FROM dbo.Users u
WHERE u.id IN (SELECT id FROM CTE);

-- Or use a specific WHERE clause instead of TOP
DELETE FROM dbo.AuditLog
WHERE createdDate < DATEADD(YEAR, -1, GETDATE())
  AND id NOT IN (
      SELECT TOP (100) id
      FROM dbo.AuditLog
      WHERE createdDate < DATEADD(YEAR, -1, GETDATE())
      ORDER BY id DESC
  );
```

## Notes

- TOP in SELECT statements is allowed and managed by the `TopWithoutOrderByRule`
- TOP in UPDATE/DELETE is particularly dangerous because it's non-deterministic
- Always use ORDER BY with TOP in CTEs to ensure deterministic row selection
- Consider using WHERE clauses with specific identifiers instead of TOP for more predictable behavior
