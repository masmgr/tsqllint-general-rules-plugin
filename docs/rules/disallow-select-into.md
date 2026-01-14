# disallow-select-into

## Summary

Warns on `SELECT ... INTO` because it implicitly creates a table schema.

## Details

- Rule ID: `disallow-select-into`
- Severity: `Warning`
- Message: `Avoid SELECT ... INTO because it implicitly creates a table schema. Prefer explicit CREATE TABLE + INSERT for repeatable schema control.`

## Examples

### Invalid

```sql
SELECT CustomerId, Name INTO dbo.CustomerCopy
FROM dbo.Customer;
```

### Valid

```sql
CREATE TABLE dbo.CustomerCopy (CustomerId int NOT NULL, Name nvarchar(100) NOT NULL);
INSERT INTO dbo.CustomerCopy (CustomerId, Name)
SELECT CustomerId, Name
FROM dbo.Customer;
```

