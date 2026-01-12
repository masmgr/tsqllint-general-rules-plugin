# avoid-heap-table

## Summary

Warns when CREATE TABLE defines a heap (no clustered index).

## Details

- Rule ID: `avoid-heap-table`
- Severity: `Warning`
- Message: `Avoid heap tables by defining a clustered index (or clustered primary key).`

## Examples

### Invalid

```sql
CREATE TABLE dbo.Customer
(
    Id INT,
    Name NVARCHAR(100)
);
```

### Valid (clustered PRIMARY KEY)

```sql
CREATE TABLE dbo.Customer
(
    Id INT NOT NULL,
    CONSTRAINT PK_Customer PRIMARY KEY CLUSTERED (Id)
);
```

### Valid (clustered index)

```sql
CREATE TABLE dbo.Customer
(
    Id INT NOT NULL,
    INDEX IX_Customer CLUSTERED (Id)
);
```

## Notes

- Temporary tables (names starting with `#`) are excluded.
- The rule inspects clustered indexes/constraints declared in `CREATE TABLE` or later in the same file.
- Clustered indexes or constraints added later in the same file (e.g., `CREATE CLUSTERED INDEX`, `ALTER TABLE ... ADD CONSTRAINT`) are also considered.
- You can configure rule severity to `Error` for production workloads if desired.

## References

- https://learn.microsoft.com/en-us/sql/relational-databases/indexes/heap-tables
