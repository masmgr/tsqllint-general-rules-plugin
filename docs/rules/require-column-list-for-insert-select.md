# require-column-list-for-insert-select

## Summary

Requires a column list for the target table in `INSERT INTO ... SELECT` statements.

## Details

- Rule ID: `require-column-list-for-insert-select`
- Severity: `Warning`
- Message: `INSERT INTO ... SELECT requires a column list.`

Traverses `InsertStatement` and reports a violation when all of the following conditions are met:

- The insert source is `SELECT` (`SelectInsertSource`)
- The target column list (`InsertSpecification.Columns`) is not specified

## Examples

### Invalid

```sql
INSERT INTO dbo.TableName
SELECT col
FROM dbo.Source;
```

### Valid

```sql
INSERT INTO dbo.TableName (col)
SELECT col
FROM dbo.Source;
```

### Valid (INSERT ... VALUES)

```sql
INSERT INTO dbo.TableName VALUES (1);
```

## Notes

- `INSERT INTO ... VALUES` is not subject to this rule.
