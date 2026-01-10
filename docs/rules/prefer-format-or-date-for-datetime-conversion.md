# prefer-format-or-date-for-datetime-conversion

## Summary

Detects `CONVERT` with style numbers when converting datetime to strings, and recommends using `FORMAT` or conversion to date type.

## Details

- Rule ID: `prefer-format-or-date-for-datetime-conversion`
- Severity: `Warning`
- Message: `Avoid CONVERT with style numbers for datetime formatting. Use FORMAT() for readable formatting or CONVERT to date type for date-only values.`

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

### Valid (FORMAT function)

```sql
-- Use the more readable FORMAT function
SELECT FORMAT(GETDATE(), 'yyyyMMdd');
SELECT FORMAT(@datetime_var, 'yyyy-MM-dd HH:mm:ss');
SELECT FORMAT(CURRENT_TIMESTAMP, 'MM/dd/yyyy');
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

- The `FORMAT` function is available in SQL Server 2012 and later.
- Conversion to date type is suitable when only the date portion is needed.
- Style numbers are used for formatting datetime types, so their presence indicates a datetime conversion.
- The use of the `FORMAT` function is strongly recommended for readability and maintainability.

## Rationale

SQL Server offers multiple methods for datetime formatting, each with the following characteristics:

- **CONVERT with style**: A legacy approach with low readability because style numbers are numeric values
- **FORMAT**: The most readable option, supporting .NET format strings
- **CONVERT to date**: The most efficient approach when only the date is needed

This rule encourages avoiding the combination of `CONVERT` and style numbers when better alternatives are available.
