---
name: incident-triage
user-invocable: false
description: "Use when triaging a production incident, outage, security event, or service degradation. Collects bounded evidence and coordinates incident-response guidance in read-only mode unless mitigation is explicitly authorized."
---

# Incident Triage

Provide a disciplined first response using `incident-response.md` and
`observability.md`.

## Parameters

- `service`: affected service or repository.
- `incidentId`: optional incident identifier.
- `severity`: known severity or `unknown`.
- `timeWindow`: investigation window; default last 30 minutes.
- `includeLogs`, `includeMetrics`, `includeDeployments`: default `true`.
- `mode`: `triage` or `mitigate`; default `triage`.
- `format`: `summary` or `json`; default `summary`.

## Workflow

1. Establish impact, start time, scope, and current symptoms.
2. Assign or request an incident commander and communication channel.
3. Collect bounded logs, metrics, traces, deployment history, and dependency status.
4. Build a timeline and identify plausible changes or failure domains.
5. Recommend containment and next evidence; do not make speculative changes.
6. In `mitigate` mode, require explicit authorization for each operational action.
7. Record recovery evidence and follow-up actions.

## Safety

- Preserve evidence before changing systems.
- Do not expose secrets or personal data in the incident record.
- Do not restart, roll back, scale, or modify production by implication.
- Treat security incidents as evidence-sensitive and escalate appropriately.

## Output

Return impact, severity, timeline, evidence sources, hypotheses, confidence,
recommended actions, authorized actions, owners, and unresolved questions.
