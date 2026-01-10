# avoid-ambiguous-datetime-literal

## Summary

Prohibits date string literals with slash delimiters, which are ambiguous and depend on language/locale settings.

## Details

- Rule ID: `avoid-ambiguous-datetime-literal`
- Severity: `Warning`
- Message: `"Date literals with slash delimiters (/) are ambiguous and depend on language settings. Use ISO 8601 format (YYYY-MM-DD) or date construction functions instead."`

Detects string literals that match the pattern of slash-delimited dates (e.g., `YYYY/MM/DD`, `MM/DD/YYYY`, `M/D/YY`). Slash-delimited date formats are ambiguous because:

1. Different SQL Server language settings interpret dates differently
2. The same date string may mean different dates on different servers
3. Month/day order is not always clear (01/02/2026 could be January 2nd or February 1st)

## Examples

### Invalid

```sql
-- Ambiguous: could be January 15th or 15th of January depending on language setting
SELECT * FROM dbo.Orders WHERE orderDate = '01/15/2026';

-- Ambiguous: YYYY/MM/DD format also depends on language settings
SELECT * FROM dbo.Orders WHERE orderDate = '2026/01/15';

-- Multiple ambiguous dates
SELECT *
FROM dbo.Orders
WHERE orderDate >= '2026/01/01'
  AND orderDate < '2026/02/01';

-- In INSERT
INSERT INTO dbo.Orders (orderDate) VALUES ('12/25/2026');
```

### Valid

```sql
-- ISO 8601 format is unambiguous and reliable
SELECT * FROM dbo.Orders WHERE orderDate = '2026-01-15';

-- Date without delimiters (also unambiguous)
SELECT * FROM dbo.Orders WHERE orderDate = '20260115';

-- ISO 8601 datetime format
SELECT * FROM dbo.Orders WHERE orderDate = '2026-01-15T10:30:00';

-- Using date construction functions (preferred)
SELECT * FROM dbo.Orders
WHERE orderDate >= DATEFROMPARTS(2026, 1, 1)
  AND orderDate < DATEFROMPARTS(2026, 2, 1);

-- Using CAST with ISO format
SELECT * FROM dbo.Orders
WHERE orderDate = CAST('2026-01-15' AS DATE);
```

## Notes

- This rule detects the pattern `\d{1,4}/\d{1,2}/\d{1,4}` at the start of a string literal
- Valid slash-delimited strings like file paths (e.g., `'C:/Users/john/file.txt'`) are not flagged
- ISO 8601 date format (`YYYY-MM-DD`) is the only unambiguous format that works across all SQL Server language settings
- The `DATEFROMPARTS`, `DATETIMEFROMPARTS`, and similar functions are clearer and recommended for date literals
- Consider using the `CAST` function with ISO format for maximum clarity

## Language Setting Impact

SQL Server's `SET LANGUAGE` or default language setting affects how dates are interpreted:
- Some settings prefer MM/DD/YYYY
- Others prefer DD/MM/YYYY
- This ambiguity can cause silent data corruption

Always use ISO 8601 format (`YYYY-MM-DD`) for portability.
