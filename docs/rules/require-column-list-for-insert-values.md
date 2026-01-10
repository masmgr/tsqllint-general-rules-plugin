# require-column-list-for-insert-values

## Summary

Requires explicit column list in INSERT...VALUES statements to prevent errors when table structure changes.

## Details

- Rule ID: `require-column-list-for-insert-values`
- Severity: `Warning`
- Message: `"INSERT INTO ... VALUES requires a column list."`

Traverses INSERT statements using VALUES and reports a violation when columns are not explicitly specified. Without an explicit column list, the INSERT statement depends on the exact column order in the table. If columns are added, removed, or reordered, the INSERT will either fail or insert data into wrong columns.

## Examples

### Invalid

```sql
-- Dangerous: depends on exact column order
INSERT INTO dbo.Users VALUES (1, 'John', 'john@example.com');

-- Multiple rows version
INSERT INTO dbo.Users VALUES
    (1, 'John', 'john@example.com'),
    (2, 'Jane', 'jane@example.com');
```

### Valid

```sql
-- Explicit column list is safer
INSERT INTO dbo.Users (id, name, email)
VALUES (1, 'John', 'john@example.com');

-- Multiple rows version
INSERT INTO dbo.Users (id, name, email) VALUES
    (1, 'John', 'john@example.com'),
    (2, 'Jane', 'jane@example.com');

-- INSERT...SELECT has similar requirement (different rule)
INSERT INTO dbo.Users (id, name, email)
SELECT user_id, user_name, user_email
FROM dbo.StagingUsers;
```

## Notes

- This rule applies only to INSERT...VALUES statements
- INSERT...SELECT statements are handled by the `require-column-list-for-insert-select` rule
- Explicit column lists make code more maintainable and resilient to schema changes
- When using INSERT with multiple VALUE rows, all rows should have the same number of values as there are columns
