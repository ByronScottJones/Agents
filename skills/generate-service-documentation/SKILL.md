---
name: generate-service-documentation
user-invocable: false
description: "Use when creating or refreshing a service record, ownership document, runbook index, or operational documentation from repository and deployment evidence. Produces a reviewable draft and never invents owners, SLOs, or dependencies."
---

# Generate Service Documentation

Build a service record from code, deployment, ownership, and observability evidence.

## Parameters

- `projectPath`: service repository; default workspace root.
- `outputPath`: destination document; default `SERVICE.md`.
- `sources`: optional files or directories to inspect.
- `includeRunbook`: default `true`.
- `format`: Markdown or JSON; default Markdown.
- `failOnUnknown`: default `false`.

## Workflow

1. Inspect repository metadata, deployment files, CI workflows, routes, manifests,
   dashboards, and existing documentation.
2. Extract purpose, owners, environments, dependencies, data classification,
   lifecycle, SLOs, alerts, recovery procedures, and escalation paths.
3. Mark every inferred or missing value explicitly as unknown.
4. Generate a draft with source references and review prompts.
5. Validate links, commands, and paths in the draft.
6. Write only with explicit permission; otherwise return the draft in memory.

## Safety

- Never invent an owner, contact, SLO, credential, or recovery guarantee.
- Do not include secrets or private telemetry.
- Preserve existing service documentation unless replacement is authorized.

## Output

Return the draft path or content, evidence sources, unknown fields, stale
references, and required owner review.
