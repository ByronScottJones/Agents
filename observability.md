---
title: "Observability"
description: "Load this file when adding or reviewing logs, metrics, traces, health checks, dashboards, or alerts."
---

# Observability

Load this file when adding or reviewing logs, metrics, traces, health checks,
dashboards, or alerts.

## OpenTelemetry

- Use OpenTelemetry (OTel) as the primary instrumentation and telemetry pipeline
  for logs, metrics, and traces. Prefer the OTel APIs and SDKs over
  vendor-specific instrumentation so destinations can change without code
  changes.
- Configure telemetry through the application's normal configuration system.
  Keep the configuration schema consistent across environments; deployment
  configuration may supply different values without requiring a rebuild or code
  change.
- The default destination MUST be local logging. Emit structured logs to the
  application's standard local logging destination (normally stdout/stderr for
  services) so development and local operation work without a collector.
- Allow configuration to route logs to an OTLP-compatible log collector. The
  destination, protocol, endpoint, and any required headers or credentials MUST
  be configurable through application configuration. Switching from local
  logging to a collector, or enabling both, MUST NOT require code changes.
- Keep local logging available as the default and make collector export an
  explicit configuration choice. A collector outage MUST NOT prevent the
  application from starting or performing its primary work; use bounded export
  timeouts, queues, and retry behavior appropriate to the SDK.
- Instrument logs, metrics, and traces with consistent service name, service
  version, and environment resource attributes. Propagate trace context across
  process and service boundaries, and include trace and span identifiers in
  logs when available.
- Measure latency with percentiles, not only averages. Expose health,
  readiness, and liveness signals that reflect real dependencies.
- Keep instrumentation independent of any one backend. Prefer the OpenTelemetry
  Collector as the configurable routing layer when telemetry must be sent to
  multiple backends or transformed centrally.

## Operations

- Define service-level indicators and objectives before creating alerts.
- Alert on actionable symptoms and include an owner, runbook, and severity.
- Keep dashboards focused on user impact, capacity, errors, and dependencies.
- Make retention, sampling, and cardinality choices deliberate and cost-aware.

## Configuration

- Document telemetry configuration keys, defaults, accepted values, and
  validation alongside the application's other settings.
- Do not require environment variables as the only way to select a telemetry
  destination. Environment-specific deployment settings MAY populate or
  override the application's configuration using its established precedence
  rules.
- Never place secrets directly in checked-in configuration. Reference the
  application's standard secret-management mechanism for collector credentials.
- Provide a local default that works when no collector is installed or
  reachable. Collector endpoints and credentials MUST be optional unless the
  selected export mode requires them.

## Privacy

- Never include secrets or unnecessary personal data in telemetry.
- Redact sensitive fields before logs leave the process.
- Verify telemetry behavior during failure and degraded-dependency scenarios,
  including collector unavailability and invalid configuration.
