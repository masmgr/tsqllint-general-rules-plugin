# require-as-for-column-alias

## Summary

Requires `AS` for column aliases in SELECT clauses.

## Details

- Rule ID: `require-as-for-column-alias`
- Severity: `Warning`
- Message: `Column aliases must use AS.`

For scalar expressions (`SelectScalarExpression`) in SELECT clauses, reports a violation when a column alias (`ColumnName`) exists but no `AS` token is found between the expression and the alias.

## Examples

### Invalid

```sql
SELECT col alias
FROM dbo.TableName;
```

### Valid

```sql
SELECT col AS alias
FROM dbo.TableName;
```

## Notes

- This rule uses the token stream (`ScriptTokenStream`) to determine the presence of `AS`.
