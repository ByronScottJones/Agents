---
name: verify-service-health
user-invocable: false
description: "Use when verifying a service deployment, readiness, dependencies, smoke flows, logs, metrics, or rollback signals. Runs bounded read-only checks by default and never triggers rollback without explicit confirmation."
---

# Verify Service Health

Turn deployment and observability guidance into a repeatable health gate.

## Parameters

- `serviceURL`: required unless checks use a local target.
- `checksPath`: optional check definitions; default `./.service-checks`.
- `mode`: `quick`, `standard`, or `comprehensive`; default `standard`.
- `checkDependencies`: default `true`.
- `includeMetrics`: default `true`.
- `rollbackOnFailure`: default `false`.
- `timeout`: default `5m`.
- `format`: `summary` or `json`; default `summary`.

## Workflow

1. Validate the target URL and check definitions without exposing credentials.
2. Run connectivity, readiness, dependency, and smoke checks in that order.
3. Capture bounded latency, status, error, and recent-log evidence.
4. Compare results with declared SLOs when available.
5. Report failures and likely ownership; never infer recovery from one signal.
6. Trigger rollback only when explicitly enabled and a documented rollback path exists.

## Safety

- Use timeouts and bounded retries.
- Do not send destructive requests as health checks.
- Redact authorization headers and sensitive response fields.
- Do not treat a passing health endpoint as proof that user workflows work.

## Output

Return target, check results, latency, dependency status, evidence timestamps,
failed checks, likely owners, rollback recommendation, and limitations.
