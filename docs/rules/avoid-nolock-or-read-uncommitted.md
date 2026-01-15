# avoid-nolock-or-read-uncommitted (DEPRECATED)

**This rule has been deprecated and merged into [`avoid-nolock`](avoid-nolock.md).**

This rule was identical in functionality to `avoid-nolock` and has been consolidated to eliminate duplication.

## Migration

If your `.tsqllintrc` configuration references this rule:

```json
"avoid-nolock-or-read-uncommitted": "error"
```

Update it to:

```json
"avoid-nolock": "error"
```

The behavior is identical - no functional changes are required beyond the rule name.
