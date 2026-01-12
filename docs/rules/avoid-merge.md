# avoid-merge

## Summary

Prohibits the use of MERGE statements due to their known issues, complexity, and behavior inconsistencies across SQL Server versions.

## Details

- Rule ID: `avoid-merge`
- Severity: `Warning`
- Message: `"MERGE statements should be avoided due to known issues and complexity. Consider using separate INSERT, UPDATE, or DELETE statements."`

Flags any MERGE statement found in the T-SQL code. MERGE statements are known to have subtle behavioral issues and are often considered error-prone in production environments. Using separate INSERT, UPDATE, and DELETE statements is generally more maintainable and predictable.

## Examples

### Invalid

```sql
MERGE INTO target USING source
ON target.id = source.id
WHEN MATCHED THEN
    UPDATE SET col = source.col
WHEN NOT MATCHED THEN
    INSERT (id, col) VALUES (source.id, source.col);
```

### Valid

```sql
-- Use separate statements instead
UPDATE target
SET col = source.col
FROM source
WHERE target.id = source.id;

INSERT INTO target (id, col)
SELECT source.id, source.col
FROM source
WHERE NOT EXISTS (SELECT 1 FROM target WHERE target.id = source.id);
```

## Notes

- MERGE statements have documented behaviors that differ from separate DML (for example, trigger firing order and @@ROWCOUNT semantics)
- Community guidance generally treats MERGE as risky without extra locking, such as WITH (HOLDLOCK)
- Known edge cases and bugs have been documented in real systems (for example, indexed view interactions)
- Using separate DML statements provides better control and clarity
- When replacing MERGE with separate statements, use explicit transactions and an appropriate locking strategy to avoid race conditions

## References

- https://learn.microsoft.com/en-us/sql/t-sql/statements/merge-transact-sql?view=sql-server-ver17
- https://sqlblog.org/merge
- https://michaeljswart.com/2021/08/what-to-avoid-if-you-want-to-use-merge/
