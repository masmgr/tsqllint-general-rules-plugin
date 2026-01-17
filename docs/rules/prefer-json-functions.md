# prefer-json-functions

## Summary

Encourages SQL Server JSON features (`OPENJSON`, `JSON_VALUE`, `FOR JSON`, etc.) instead of manual JSON string parsing/building.

## Details

- Rule ID: `prefer-json-functions`
- Severity: `Warning`
- Message: `Prefer built-in JSON support (OPENJSON, JSON_VALUE, FOR JSON, JSON_QUERY, JSON_MODIFY) over manual string parsing/building.`

## Examples

### Invalid

```sql
SELECT '{"id":' + CAST(@id AS nvarchar(10)) + '}';
```

### Valid

```sql
SELECT JSON_VALUE(@json, '$.id');
```

## Notes

- This rule uses heuristics (e.g., string concatenation or `CHARINDEX`/`PATINDEX` patterns that look JSON-like) and may not catch every case.

