# avoid-nolock-or-read-uncommitted

## Summary

Warns on `NOLOCK` / `READUNCOMMITTED` table hints and `SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED`.

## Details

- Rule ID: `avoid-nolock-or-read-uncommitted`
- Severity: `Warning`
- Message: `NOLOCK and READ UNCOMMITTED allow dirty reads which can return inconsistent or incorrect data. Prefer appropriate isolation levels or snapshot isolation.`

## Examples

### Invalid

```sql
SELECT * FROM dbo.Customer WITH (NOLOCK);
```

### Valid

```sql
SELECT * FROM dbo.Customer;
```

