# avoid-atat-identity

## Summary

Disallows the use of `@@IDENTITY` and encourages using `SCOPE_IDENTITY()` or `OUTPUT`.

## Details

- Rule ID: `avoid-atat-identity`
- Severity: `Warning`
- Message: `Avoid @@IDENTITY. Use SCOPE_IDENTITY() or OUTPUT.`

Traverses `GlobalVariableExpression` and reports a violation when the following conditions are met:

- Global variable name is `@@IDENTITY`

## Examples

### Invalid

```sql
SELECT @@IDENTITY;
```

### Valid

```sql
SELECT SCOPE_IDENTITY();
```

### Valid (OUTPUT)

```sql
INSERT INTO dbo.TableName (col)
OUTPUT inserted.col
VALUES (1);
```
