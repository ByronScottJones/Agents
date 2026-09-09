---
name: validate-and-deploy-iac
user-invocable: false
description: "Use when validating, planning, detecting drift in, or applying infrastructure as code. Supports Terraform and extensible IaC workflows with plan-first safety gates, destructive-change review, and explicit approval before mutation."
---

# Validate and Deploy Infrastructure as Code

Validate infrastructure changes and, when explicitly authorized, promote them
through a plan-first workflow.

## Parameters

- `projectPath`: IaC root; default workspace root.
- `framework`: `terraform`, `bicep`, `pulumi`, `cloudformation`, or auto-detect.
- `environment`: target environment; default `staging`.
- `mode`: `validate`, `plan`, `detect-drift`, `apply`, `destroy`, or `cost`; default `validate`.
- `targetResources`: optional resource scope.
- `reviewedPlanArtifact`: required for `apply`; identifies the reviewed plan by path, ID, or checksum.
- `format`: `summary`, `json`, or native plan artifact; default `summary`.

## Workflow

1. Detect the framework, backend, variables, provider versions, and environment.
2. Run formatting, syntax, schema, lint, and policy validation.
3. For `plan`, `cost`, or `detect-drift`, produce a reviewable artifact.
4. Classify creates, updates, replacements, and destroys; identify sensitive values.
5. Require explicit approval for apply, destroy, production, or destructive plans.
6. For `apply`, require and verify `reviewedPlanArtifact` before mutation; reject
  a regenerated or mismatched plan.
7. Apply the exact verified plan, never a newly generated unreviewed plan.
8. Verify post-apply state, outputs, health, and drift.

## Safety

- Default to read-only validation or plan mode.
- Approval is mandatory for every apply or destroy operation and cannot be disabled by a parameter.
- Redact secrets from plans, logs, and artifacts; redaction cannot be disabled.
- Reject apply when the reviewed plan artifact is missing or does not match.
- Do not bypass policy violations or state locking.

## Output

Return framework, environment, validation results, plan summary, drift status,
destructive changes, approvals, applied resources, and next steps.
