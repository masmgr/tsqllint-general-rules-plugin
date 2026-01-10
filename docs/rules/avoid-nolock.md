# avoid-nolock

## Summary

Prohibits the use of NOLOCK table hint and READ UNCOMMITTED isolation level, which allow dirty reads and can return inconsistent data.

## Details

- Rule ID: `avoid-nolock`
- Severity: `Warning`
- Message: `"NOLOCK and READ UNCOMMITTED allow dirty reads which can return incorrect data. Use appropriate isolation levels or snapshot isolation instead."`

Detects two patterns:

1. **Table Hint:** `WITH (NOLOCK)` or `WITH (READUNCOMMITTED)` in table references
2. **Isolation Level:** `SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED`

Both patterns allow dirty reads, which means queries can read data that has been modified but not yet committed by other transactions. This can lead to incorrect results, especially in high-concurrency environments.

## Examples

### Invalid

```sql
-- Using NOLOCK table hint (allows dirty reads)
SELECT u.id, u.name, o.total
FROM dbo.Users u WITH (NOLOCK)
JOIN dbo.Orders o WITH (NOLOCK) ON u.id = o.user_id;

-- Using READUNCOMMITTED hint
SELECT * FROM dbo.Products WITH (READUNCOMMITTED);

-- Setting READ UNCOMMITTED isolation level
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;
SELECT * FROM dbo.Users;
```

### Valid

```sql
-- Use default isolation level (READ COMMITTED) for safer reads
SELECT u.id, u.name, o.total
FROM dbo.Users u
JOIN dbo.Orders o ON u.id = o.user_id;

-- Use snapshot isolation for non-blocking reads with consistency
SET TRANSACTION ISOLATION LEVEL SNAPSHOT;
SELECT * FROM dbo.Users;

-- Or explicitly set READ COMMITTED if needed
SET TRANSACTION ISOLATION LEVEL READ COMMITTED;
SELECT * FROM dbo.Users;
```

## Notes

- NOLOCK is equivalent to READ UNCOMMITTED isolation level
- Dirty reads can occur when:
  - Reading data that another transaction is modifying
  - The other transaction rolls back (leaving you with "dirty" data)
- Alternatives to NOLOCK:
  - **Read Committed Snapshot Isolation (RCSI):** Enable at database level with `ALTER DATABASE SET READ_COMMITTED_SNAPSHOT ON`
  - **Snapshot Isolation:** Set transaction isolation level to SNAPSHOT
  - **Default READ COMMITTED:** Usually sufficient for most applications
- Performance impact from avoiding NOLOCK is typically negligible compared to data correctness risks
