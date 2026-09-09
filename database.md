# Databases and Data

Load this file when designing persistence, schemas, queries, or data migrations.

## Data Design

- Identify data ownership, classification, retention, and deletion requirements.
- Choose transaction boundaries, consistency guarantees, and isolation deliberately.
- Review indexes, query plans, connection pooling, and expected growth.
- Keep production data out of development and test fixtures unless it is sanitized.

## Migrations

- Make schema changes backward-compatible when old and new application versions may coexist.
- Separate expand, migrate, and contract steps for risky changes.
- Test migrations on representative data and document rollback or recovery limits.
- Track migration order and ensure each deployment can identify its schema version.

## Resilience

- Define backup, restore, retention, and recovery testing requirements.
- Handle transient database failures with bounded retries and clear failure behavior.
