---
name: prepare-release
user-invocable: false
description: "Use when preparing a release candidate, release notes, artifact promotion, deployment readiness check, migration notes, or rollback plan. Defaults to read-only readiness analysis and never publishes or deploys without explicit confirmation."
---

# Prepare Release

Coordinate `release.md`, Git history, CI results, artifacts, and deployment risk.

## Parameters

- `owner`, `repository`, `pullRequest`: optional release source.
- `version`: required or inferred from project convention.
- `environment`: target promotion environment; default `staging`.
- `mode`: `notes`, `readiness`, `candidate`, `promote`, or `rollback-plan`; default `readiness`.
- `requireChecks`: default `true`.
- `format`: `summary`, `json`, or Markdown release notes; default `summary`.

## Workflow

1. Inspect commits, merged changes, version configuration, migrations, and CI status.
2. Generate notes covering behavior changes, risks, migrations, dependencies, and rollback.
3. Verify artifact identity, provenance, tests, security scans, and environment gates.
4. Identify backward-compatibility and data-migration sequencing risks.
5. For candidate or promotion modes, require explicit confirmation and verify the
   exact artifact being promoted.
6. Record deployment evidence and post-release verification requirements.

## Safety

- Do not publish tags, releases, or deployments without explicit authorization.
- Never promote an artifact that has not passed required checks.
- Redact secrets and sensitive plan or test output.

## Output

Return version, source revision, artifact identity, readiness checks, release
notes, migration requirements, rollback plan, blockers, and next steps.
