# Observability

Load this file when adding or reviewing logs, metrics, traces, health checks,
dashboards, or alerts.

## Signals

- Emit structured logs, metrics, and traces with consistent service and version fields.
- Propagate correlation or trace IDs across process and service boundaries.
- Measure latency with percentiles, not only averages.
- Expose health, readiness, and liveness signals that reflect real dependencies.

## Operations

- Define service-level indicators and objectives before creating alerts.
- Alert on actionable symptoms and include an owner, runbook, and severity.
- Keep dashboards focused on user impact, capacity, errors, and dependencies.
- Make retention, sampling, and cardinality choices deliberate and cost-aware.

## Privacy

- Never include secrets or unnecessary personal data in telemetry.
- Redact sensitive fields before logs leave the process.
- Test observability behavior during failure and degraded-dependency scenarios.
