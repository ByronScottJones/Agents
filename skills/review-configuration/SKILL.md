---
name: review-configuration
user-invocable: false
description: "Use when auditing application configuration for secret exposure, precedence conflicts, missing validation, undocumented defaults, environment drift, or unsafe public variables. Produces a report and does not modify configuration by default."
---

# Review Configuration

Audit configuration against `configuration.md`, platform guidance, and project
conventions.

## Parameters

- `projectPath`: project root; default workspace root.
- `scope`: files, environment variables, deployment configuration, or all; default all.
- `environment`: optional target environment.
- `checkSecrets`, `checkPrecedence`, `checkSchemas`, `checkPublicVariables`: default `true`.
- `fix`: default `false`.
- `format`: `summary` or `json`; default `summary`.

## Workflow

1. Discover config files, environment examples, schemas, deployment manifests,
   and code paths that load settings.
2. Detect secrets, credentials, unsafe defaults, missing types, undocumented
   variables, precedence conflicts, and client-exposed sensitive values.
3. Compare development, staging, and production configuration expectations.
4. Check schema validation and startup failure behavior.
5. In fix mode, propose narrowly scoped documentation or configuration changes.

## Safety

- Never print secret values; report only names and locations.
- Do not load production secrets into the agent context.
- Do not change precedence or defaults without identifying compatibility impact.
- Treat browser-exposed variables as public.

## Output

Return files and variables reviewed, findings by severity, secret locations with
values redacted, precedence issues, missing documentation, and remediation steps.
