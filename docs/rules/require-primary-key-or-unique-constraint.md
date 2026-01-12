# require-primary-key-or-unique-constraint

## Summary

Requires a PRIMARY KEY or UNIQUE constraint for user tables to ensure row uniqueness.

## Details

- Rule ID: `require-primary-key-or-unique-constraint`
- Severity: `Error`
- Message: `User tables must define a PRIMARY KEY or UNIQUE constraint to ensure row uniqueness.`

## Examples

### Invalid

```sql
CREATE TABLE dbo.Customer
(
    Id INT,
    Name NVARCHAR(100)
);
```

### Valid (PRIMARY KEY)

```sql
CREATE TABLE dbo.Customer
(
    Id INT NOT NULL,
    CONSTRAINT PK_Customer PRIMARY KEY (Id)
);
```

### Valid (UNIQUE)

```sql
CREATE TABLE dbo.Customer
(
    Email NVARCHAR(100) UNIQUE
);
```

## Notes

- Temporary tables (names starting with `#`) are excluded.
- Unique indexes defined in `CREATE TABLE` also satisfy this rule.
- UNIQUE indexes or constraints added later in the same file (e.g., `CREATE UNIQUE INDEX`, `ALTER TABLE ... ADD CONSTRAINT`) are also considered.

## References

- https://learn.microsoft.com/en-us/sql/relational-databases/tables/primary-and-foreign-key-constraints
