# avoid-exec-dynamic-sql

## Summary

Warns about `EXEC` execution for dynamic SQL and recommends using `sp_executesql` with parameters.

## Details

- Rule ID: `avoid-exec-dynamic-sql`
- Severity: `Warning`
- Message: `Avoid EXEC for dynamic SQL. Use sp_executesql with parameters.`

Traverses `ExecuteSpecification` and reports a violation when the following conditions are met:

- `ExecutableEntity` is `ExecutableStringList`
- The string being executed is not a literal (variable or expression)

## Examples

### Invalid

```sql
DECLARE @sql nvarchar(max) = N'SELECT * FROM dbo.TableName WHERE Id = @Id';
EXEC(@sql);
```

### Valid

```sql
DECLARE @sql nvarchar(max) = N'SELECT * FROM dbo.TableName WHERE Id = @Id';
EXEC sp_executesql @sql, N'@Id int', @Id = 1;
```

### Valid

```sql
EXEC dbo.ProcName @Id = 1;
```
