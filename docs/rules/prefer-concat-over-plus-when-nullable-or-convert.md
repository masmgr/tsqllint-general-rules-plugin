# prefer-concat-over-plus-when-nullable-or-convert

## Summary

Recommends `CONCAT` whenever string concatenation mixes `ISNULL`, `CONVERT`, or `CAST` with a literal to avoid silent `NULL` propagation and improve readability.

## Details

- Rule ID: `prefer-concat-over-plus-when-nullable-or-convert`
- Severity: `Warning`
- Message: `Use CONCAT when string building involves ISNULL/CONVERT/CAST to avoid NULL propagation or readability issues.`
- AST nodes: `BinaryExpression`, `FunctionCall`, `ConvertCall`, `CastCall`, `StringLiteral`

Reports violations when a `+` expression (flattened across nested additions) contains a string literal and at least one `ISNULL`, `CONVERT`, or `CAST` invocation.

## Examples

### Invalid

```sql
SET @s = ISNULL(@a, '') + ',' + CONVERT(varchar(10), @d, 120);
```

### Valid

```sql
SET @s = CONCAT(ISNULL(@a, ''), ',', CONVERT(varchar(10), @d, 120));
```

## Notes

- Detection requires a literal (e.g., `' ,'` or `N'...'`) to reduce false positives in purely arithmetic expressions.
- The rule intentionally ignores concatenations that do not mix with `ISNULL`/`CONVERT`/`CAST`.
- This rule extends [`prefer-concat-over-plus`](prefer-concat-over-plus.md) to include type conversion functions (`CONVERT`/`CAST`)
- Enable this rule instead of `prefer-concat-over-plus` if you want stricter concatenation linting
- These rules are mutually exclusive - choose one based on your strictness preference
