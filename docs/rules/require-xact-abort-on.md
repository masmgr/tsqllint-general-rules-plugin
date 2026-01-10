# require-xact-abort-on

## Summary

Requires SET XACT_ABORT ON for all explicit transactions to ensure consistent rollback behavior on runtime errors.

## Details

- Rule ID: `require-xact-abort-on`
- Severity: `Warning`
- Message: `"Transactions should include SET XACT_ABORT ON to ensure consistent rollback behavior on runtime errors."`

Detects `BEGIN TRAN` or `BEGIN TRANSACTION` statements in a batch that are not preceded by `SET XACT_ABORT ON`. When `SET XACT_ABORT ON` is not enabled:

1. Runtime errors in the batch do not automatically rollback the transaction
2. The batch continues executing after errors (if possible)
3. Partial/incomplete transactions may be committed
4. Inconsistent state can develop if the application doesn't properly handle errors

With `SET XACT_ABORT ON`:
- Any runtime error causes the entire batch to stop
- Any active transaction is rolled back automatically
- Errors are more predictable and safer

## Examples

### Invalid

```sql
-- Missing SET XACT_ABORT ON - unsafe transaction handling
BEGIN TRAN;
UPDATE dbo.Users SET name = 'John' WHERE id = 1;
INSERT INTO dbo.Orders (user_id, amount) VALUES (1, 100);
COMMIT;

-- Multiple transactions without XACT_ABORT
BEGIN TRAN;
DELETE FROM dbo.AuditLog WHERE logDate < '2020-01-01';
COMMIT;

BEGIN TRAN;
UPDATE dbo.Config SET value = 'processed' WHERE key = 'LastRun';
COMMIT;
```

### Valid

```sql
-- SET XACT_ABORT ON at batch start ensures all transactions are safe
SET XACT_ABORT ON;

BEGIN TRAN;
UPDATE dbo.Users SET name = 'John' WHERE id = 1;
INSERT INTO dbo.Orders (user_id, amount) VALUES (1, 100);
COMMIT;

-- All subsequent transactions in this batch are covered
BEGIN TRAN;
DELETE FROM dbo.AuditLog WHERE logDate < '2020-01-01';
COMMIT;

BEGIN TRAN;
UPDATE dbo.Config SET value = 'processed' WHERE key = 'LastRun';
COMMIT;
```

## Best Practices

### 1. Always set at batch start

```sql
SET XACT_ABORT ON;
-- All transactions in this batch are safe
```

### 2. Combine with TRY/CATCH

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

### 3. In stored procedures

```sql
CREATE PROCEDURE sp_UpdateUser
    @id INT,
    @name NVARCHAR(255)
AS
BEGIN
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRAN;
        UPDATE dbo.Users SET name = @name WHERE id = @id;
        COMMIT;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK;
        THROW;
    END CATCH;
END;
```

## Behavior Comparison

| Setting | Error Occurs | Transaction | Batch Continues |
|---------|-------------|-------------|-----------------|
| XACT_ABORT OFF | During DML | Not rolled back | Yes (tries to continue) |
| XACT_ABORT OFF | Parse error | N/A | No (parse error stops batch) |
| XACT_ABORT ON | Any error | Auto rolled back | No (batch stops) |

## Notes

- This rule checks at the batch level (statements between GO delimiters)
- `SET XACT_ABORT ON` applies to the entire batch after it's set
- The default is `XACT_ABORT OFF`
- This rule complements `require-try-catch-for-transaction`
- For stored procedures in SSMS, include `SET XACT_ABORT ON` as the first statement
- This applies to SQL Server 2005 and later
