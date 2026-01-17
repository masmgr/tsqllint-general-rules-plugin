# Changelog

## [1.2.0] - 2026-01-17

### Added
- New lint rules with tests and documentation:
  - `avoid-implicit-conversion-in-predicate`
  - `avoid-nolock-or-read-uncommitted`
  - `disallow-select-into`
  - `disallow-select-star`
  - `forbid-top-100-percent-order-by`
  - `meaningful-alias`
  - `prefer-concat-over-plus`
  - `prefer-concat-ws`
  - `prefer-json-functions`
  - `prefer-string-agg-over-stuff`
  - `prefer-trim-over-ltrim-rtrim`
  - `prefer-try-convert-patterns`
  - `require-parentheses-for-mixed-and-or`
  - `require-qualified-columns-everywhere`

### Changed
- All SQL lint rules now inherit from `SqlLintRuleBase` for improved code structure.
- `GetRules` method refactored to use rule factories for better maintainability.
- Enhanced rule descriptions and documentation for improved clarity.
- README.md expanded with detailed rule descriptions and clarifications.

### Enhanced
- `require-ms-description-for-table-definition-file` now supports positional parameters.
- `require-qualified-select-columns` now supports DATEADD/DATEDIFF datepart arguments.

### Deprecated
- `avoid-nolock-or-read-uncommitted` rule is now deprecated.

### Improved
- Added line and column assertions to RequireBeginEndForIf and While rule tests for better test coverage.

## [1.1.0] - 2026-01-13

### Added
- New lint rules with tests and documentation:
  - `avoid-heap-table`
  - `prefer-concat-over-plus-when-nullable-or-convert`
  - `require-begin-end-for-if-with-controlflow-exception`
  - `require-begin-end-for-while`
  - `require-ms-description-for-table-definition-file`
  - `require-primary-key-or-unique-constraint`

### Changed
- `order-by-in-subquery` now allows `FOR JSON` in subqueries.
- Rule ID renamed: `prefer-format-or-date-for-datetime-conversion` -> `avoid-magic-convert-style-for-datetime` (tests/docs updated accordingly).
- Documentation updates/clarifications for `avoid-merge` and `require-as-for-table-alias`.
- Bumped package/assembly version to 1.1.0.

## [1.0.0] - 2026-01-11

### Added
- GitHub release workflow that builds, tests, and publishes tagged artifacts.
- Version metadata in `TSQLLintGeneralRulesPlugin.csproj` for assembly/file versioning.
