# require-explicit-join-type

## Summary

Disallows omitting INNER/OUTER keywords in JOIN clauses.

## Details

- Rule ID: `require-explicit-join-type`
- Severity: `Warning`
- Message: `JOIN must be explicit: use INNER JOIN, LEFT OUTER JOIN, RIGHT OUTER JOIN, or FULL OUTER JOIN.`

Reports a violation when the following JOIN types are used without explicitly specifying INNER/OUTER keywords:

- `JOIN`
- `LEFT JOIN`
- `RIGHT JOIN`
- `FULL JOIN`

## Examples

### Invalid (implicit INNER)

```sql
SELECT *
FROM dbo.TableA
JOIN dbo.TableB ON TableA.Id = TableB.Id;
```

### Invalid (implicit OUTER)

```sql
SELECT *
FROM dbo.TableA
LEFT JOIN dbo.TableB ON TableA.Id = TableB.Id;
```

### Valid

```sql
SELECT *
FROM dbo.TableA
INNER JOIN dbo.TableB ON TableA.Id = TableB.Id
LEFT OUTER JOIN dbo.TableC ON TableA.Id = TableC.Id
RIGHT OUTER JOIN dbo.TableD ON TableA.Id = TableD.Id
FULL OUTER JOIN dbo.TableE ON TableA.Id = TableE.Id;
```

## Notes

- This rule uses the token stream (`ScriptTokenStream`) within the JOIN clause to determine the presence of `INNER`/`OUTER`.
