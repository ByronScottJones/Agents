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
- `requireApproval`: default `true` for `apply` and `destroy`.
- `redactPlan`: default `true`.
- `format`: `summary`, `json`, or native plan artifact; default `summary`.

## Workflow

1. Detect the framework, backend, variables, provider versions, and environment.
2. Run formatting, syntax, schema, lint, and policy validation.
3. For `plan`, `cost`, or `detect-drift`, produce a reviewable artifact.
4. Classify creates, updates, replacements, and destroys; identify sensitive values.
5. Require explicit approval for apply, destroy, production, or destructive plans.
6. Apply the exact reviewed plan, never a newly generated unreviewed plan.
7. Verify post-apply state, outputs, health, and drift.

## Safety

- Default to read-only validation or plan mode.
- Never apply or destroy without explicit confirmation.
- Redact secrets from plans, logs, and artifacts.
- Do not bypass policy violations or state locking.

## Output

Return framework, environment, validation results, plan summary, drift status,
destructive changes, approvals, applied resources, and next steps.
