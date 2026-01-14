# prefer-string-agg-over-stuff

## Summary

Recommends `STRING_AGG()` instead of the legacy `STUFF(... FOR XML PATH('') ...)` string aggregation pattern.

## Details

- Rule ID: `prefer-string-agg-over-stuff`
- Severity: `Warning`
- Message: `Prefer STRING_AGG() over STUFF(... FOR XML PATH('') ...) for readability and correctness.`

## Examples

### Invalid

```sql
SELECT STUFF((
    SELECT ',' + t.name
    FROM sys.tables t
    FOR XML PATH('')
), 1, 1, '');
```

### Valid

```sql
SELECT STRING_AGG(t.name, ',')
FROM sys.tables t;
```

