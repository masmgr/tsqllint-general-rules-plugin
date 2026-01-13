# Changelog

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
