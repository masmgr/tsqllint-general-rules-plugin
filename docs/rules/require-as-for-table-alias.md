# require-as-for-table-alias

## Summary

Requires `AS` for table aliases (including derived tables).

## Details

- Rule ID: `require-as-for-table-alias`
- Severity: `Warning`
- Message: `Table aliases must use AS.`

For the following nodes, reports a violation when an alias exists but no `AS` token is found between the element and the alias:

- `NamedTableReference`
- `QueryDerivedTable`

## Examples

### Invalid (named table)

```sql
SELECT *
FROM dbo.TableName t;
```

### Valid (named table)

```sql
SELECT *
FROM dbo.TableName AS t;
```

### Invalid (derived table)

```sql
SELECT *
FROM (SELECT 1 AS Value) t;
```

### Valid (derived table)

```sql
SELECT *
FROM (SELECT 1 AS Value) AS t;
```

## Notes

- This rule uses the token stream (`ScriptTokenStream`) to determine the presence of `AS`.
