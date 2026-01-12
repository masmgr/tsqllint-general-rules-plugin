# avoid-magic-convert-style-for-datetime

## Summary

Detects `CONVERT` with style numbers when converting datetime to strings, and recommends avoiding magic numbers in favor of clearer, safer alternatives.

## Details

- Rule ID: `avoid-magic-convert-style-for-datetime`
- Severity: `Warning`
- Message: `Avoid CONVERT with style numbers for datetime formatting. Prefer ISO-style conversions or date types; reserve FORMAT() for small UI output with explicit culture.`

Traverses `ConvertCall` and reports a violation when the following conditions are met:

- The target data type is a string type (varchar, nvarchar, char, nchar)
- A style parameter (3rd argument) is specified

## Examples

### Invalid

```sql
-- Style 112 (YYYYMMDD)
SELECT CONVERT(varchar(8), GETDATE(), 112);

-- Style 120 (ISO format)
SELECT CONVERT(nvarchar(19), @datetime_var, 120);

-- Style 101 (MM/DD/YYYY)
SELECT CONVERT(char(10), CURRENT_TIMESTAMP, 101);
```

### Valid (FORMAT function, limited use)

```sql
-- Limit FORMAT to small UI outputs and specify culture explicitly
SELECT FORMAT(GETDATE(), 'yyyyMMdd', 'en-US');
SELECT FORMAT(@datetime_var, 'yyyy-MM-dd HH:mm:ss', 'en-US');
```

### Valid (CONVERT to date type)

```sql
-- For date-only values, convert to date type
SELECT CONVERT(date, GETDATE());
SELECT CONVERT(date, @datetime_var);
SELECT CONVERT(date, CURRENT_TIMESTAMP);
```

### Valid (CONVERT without style)

```sql
-- Conversion without style (non-datetime types)
SELECT CONVERT(varchar(10), @int_value);
SELECT CONVERT(nvarchar(50), @decimal_value);
SELECT CONVERT(varchar, some_column);
```

## Notes

- The `FORMAT` function is available in SQL Server 2012 and later, but it is CLR-based and nondeterministic.
- `FORMAT` depends on session language when culture is omitted, which can change results across environments.
- For large result sets or performance-sensitive queries, `FORMAT` is often slower than `CONVERT`.
- Conversion to date type is suitable when only the date portion is needed.
- Style numbers are used for formatting datetime types, so their presence indicates a datetime conversion.
- If a style number must be used, prefer ISO-style formats (for example, 23/126/127) and add a short comment explaining the choice.

## References

- https://learn.microsoft.com/en-us/sql/t-sql/functions/format-transact-sql?view=sql-server-ver17
- https://www.sqlservercentral.com/articles/how-to-format-dates-in-sql-server-hint-dont-use-format

## Rationale

SQL Server offers multiple methods for datetime formatting, each with the following characteristics:

- **CONVERT with style**: A legacy approach with low readability because style numbers are numeric values
- **FORMAT**: Readable but nondeterministic and often slower; best for small UI outputs with explicit culture
- **CONVERT to date**: The most efficient approach when only the date is needed

This rule encourages avoiding the combination of `CONVERT` and style numbers when clearer or safer alternatives are available.
