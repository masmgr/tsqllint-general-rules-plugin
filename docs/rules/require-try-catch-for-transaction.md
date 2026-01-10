# require-try-catch-for-transaction

## Summary

Requires that all explicit transactions be wrapped in TRY/CATCH blocks to ensure proper error handling and rollback on failure.

## Details

- Rule ID: `require-try-catch-for-transaction`
- Severity: `Warning`
- Message: `"BEGIN TRANSACTION should be wrapped in TRY/CATCH to ensure proper error handling and rollback."`

Detects `BEGIN TRAN` or `BEGIN TRANSACTION` statements that are not inside a TRY/CATCH block. Without TRY/CATCH, transaction rollback and error handling rely on application-level logic, which can be fragile and error-prone. A TRY/CATCH block ensures that:

1. Any error during the transaction triggers the CATCH block
2. The CATCH block can explicitly ROLLBACK the transaction
3. Errors are properly propagated with THROW

## Examples

### Invalid

```sql
-- Error handling relies on application, transaction may remain uncommitted
BEGIN TRAN;
UPDATE dbo.Users SET name = 'John' WHERE id = 1;
UPDATE dbo.Orders SET status = 'Updated' WHERE user_id = 1;
COMMIT;

-- DELETE without error handling
BEGIN TRANSACTION;
DELETE FROM dbo.AuditLog WHERE logDate < '2020-01-01';
COMMIT;
```

### Valid

```sql
-- Transaction wrapped in TRY/CATCH with proper error handling
BEGIN TRY
    BEGIN TRAN;
    UPDATE dbo.Users SET name = 'John' WHERE id = 1;
    UPDATE dbo.Orders SET status = 'Updated' WHERE user_id = 1;
    COMMIT;
END TRY
BEGIN CATCH
    ROLLBACK;
    THROW;
END CATCH;

-- DELETE with error handling
BEGIN TRY
    BEGIN TRANSACTION;
    DELETE FROM dbo.AuditLog WHERE logDate < '2020-01-01';
    COMMIT;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
        ROLLBACK;
    THROW;
END CATCH;

-- Nested transactions are allowed
BEGIN TRY
    BEGIN TRAN;
    -- ... statements ...
    BEGIN TRY
        -- nested work
    END TRY
    BEGIN CATCH
        -- handle nested error
    END CATCH;
    COMMIT;
END TRY
BEGIN CATCH
    ROLLBACK;
    THROW;
END CATCH;
```

## Best Practices

1. **Always use TRY/CATCH with transactions**
   ```sql
   BEGIN TRY
       BEGIN TRAN;
       -- Your work here
       COMMIT;
   END TRY
   BEGIN CATCH
       ROLLBACK;
       THROW;
   END CATCH;
   ```

2. **Check transaction state in CATCH**
   ```sql
   BEGIN CATCH
       IF XACT_STATE() <> 0
           ROLLBACK;
       THROW;
   END CATCH;
   ```

3. **Combine with SET XACT_ABORT ON** (see `require-xact-abort-on`)
   ```sql
   SET XACT_ABORT ON;
   BEGIN TRY
       BEGIN TRAN;
       -- Your work here
       COMMIT;
   END TRY
   BEGIN CATCH
       ROLLBACK;
       THROW;
   END CATCH;
   ```

## Notes

- TRY/CATCH was introduced in SQL Server 2005
- XACT_STATE() returns the transaction state:
  - -1 = uncommittable transaction
  - 0 = no transaction
  - 1 = committable transaction
- Use THROW (SQL Server 2012+) instead of RAISERROR for cleaner error propagation
- This rule complements `require-xact-abort-on` for comprehensive transaction safety
