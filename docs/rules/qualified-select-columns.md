# qualified-select-columns

## Summary

Disallows unqualified column references in SELECT lists when multiple tables are referenced.

## Details

- Rule ID: `qualified-select-columns`
- Severity: `Warning`
- Message: `Unqualified column reference in SELECT list when multiple tables are referenced. Qualify it with table alias (e.g., t.id).`

For SELECT statements with 2 or more table references (including FROM and JOIN), requires that column references in the SELECT list be qualified as `alias.column` / `table.column` / `schema.table.column`.

## Examples

### Invalid

```sql
SELECT id
FROM users u
JOIN orders o ON u.id = o.user_id;
```

```sql
SELECT CASE WHEN id > 0 THEN 1 END
FROM users u
JOIN orders o ON u.id = o.user_id;
```

### Valid

```sql
SELECT u.id, o.id
FROM users u
JOIN orders o ON u.id = o.user_id;
```

```sql
SELECT COALESCE(u.name, '') AS n
FROM users u
JOIN orders o ON u.id = o.user_id;
```

```sql
SELECT *
FROM users u
JOIN orders o ON u.id = o.user_id;
```

## Notes

- SELECT statements with a single table reference are not subject to this rule.
- Subqueries are evaluated independently; the outer FROM context is not inherited.
- This rule only applies to SELECT lists; `ORDER BY`/`GROUP BY` are not subject to this rule.
