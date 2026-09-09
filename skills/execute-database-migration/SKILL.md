---
name: execute-database-migration
user-invocable: false
description: "Use when planning, validating, testing, executing, or rolling back a database schema migration. Defaults to read-only validation and dry-run behavior; production mutation requires explicit confirmation and a recovery plan."
---

# Execute Database Migration

Implement `database.md` and `disaster-recovery.md` as a safe migration workflow.

## Parameters

- `database`: target identifier; never require a secret in chat.
- `migrationsPath`: default `db/migrations/`.
- `currentVersion`, `targetVersion`: detected unless supplied.
- `mode`: `validate`, `plan`, `test`, `execute`, or `rollback`; default `validate`.
- `dryRun`: default `true`.
- `backupRequired`: default `true` for execute and rollback.
- `checkBackwardCompatibility`: default `true`.
- `timeout`: default `30m`.
- `format`: `summary` or `json`; default `summary`.

## Workflow

1. Detect the migration framework and current schema version.
2. Validate syntax, ordering, checksums, permissions, and dependencies.
3. Analyze expand/migrate/contract compatibility with old and new application versions.
4. Test against representative sanitized data where possible.
5. For execute, require confirmation, verify backup/recovery readiness, apply the
   reviewed migration, and run schema and application smoke checks.
6. For rollback, prefer a tested down migration or restore procedure; report when
   rollback is impossible or destructive.

## Safety

- Never ask for or print database credentials.
- Never execute against production without explicit confirmation.
- Do not claim rollback support unless it has been verified.
- Capture timing, locks, affected rows, and validation results without sensitive data.

## Output

Return current and target versions, schema changes, compatibility findings,
backup status, execution status, smoke checks, rollback limits, and next steps.
