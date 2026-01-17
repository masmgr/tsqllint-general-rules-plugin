# require-ms-description-for-table-definition-file

## Summary

Ensures table definition files include an `MS_Description` extended property for the owning table to keep metadata in sync with the schema.

## Details

- Rule ID: `require-ms-description-for-table-definition-file`
- Severity: `Warning`
- Message: `Table definition files must include an MS_Description extended property for the table.`
- AST nodes: `CreateTableStatement`, `ExecuteStatement`, `ExecutableProcedureReference`, `ProcedureReferenceName`, `ExecuteParameter`

Reports a violation when a `CREATE TABLE` statement in the file does not have a corresponding `sp_addextendedproperty`/`sp_updateextendedproperty` call for the same schema/table with `@name='MS_Description'`, `@level0type='SCHEMA'`, and `@level1type='TABLE'`.

## Examples

### Invalid

```sql
CREATE TABLE dbo.Customer (
    CustomerId INT NOT NULL,
    Name NVARCHAR(100) NOT NULL
);
```

### Valid

```sql
CREATE TABLE dbo.Customer (
    CustomerId INT NOT NULL,
    Name NVARCHAR(100) NOT NULL
);
EXEC sys.sp_addextendedproperty
  @name = N'MS_Description', @value = N'Customer master table',
  @level0type = N'SCHEMA', @level0name = N'dbo',
  @level1type = N'TABLE',  @level1name = N'Customer';
```

## Notes

- This rule assumes one table definition per file and that the MS_Description property is declared alongside the table.
- Named parameters are supported. Positional arguments are also supported when the required values are passed as literals (for example: `EXEC sys.sp_addextendedproperty N'MS_Description', ...`).
- Both `sp_addextendedproperty` and `sp_updateextendedproperty` calls count as fulfilling the requirement.
