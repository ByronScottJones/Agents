---
name: validate-and-deploy-kubernetes
user-invocable: false
description: "Use when validating, diffing, applying, rolling back, or health-checking Kubernetes manifests. Defaults to read-only validation and dry-run behavior; cluster mutations require explicit confirmation."
---

# Validate and Deploy Kubernetes

Implement `kubernetes.md` as a manifest and rollout workflow.

## Parameters

- `manifestPath`: default `k8s/` or `manifests/`.
- `clusterContext`: current kubectl context unless explicitly supplied.
- `namespace`: default `default`.
- `mode`: `validate`, `diff`, `apply`, or `rollback`; default `validate`.
- `checkPolicy`, `checkSecurity`, `checkResources`: default `true`.
- `waitForHealth`: default `true` for apply.
- `rolloutTimeout`: default `10m`.
- `format`: `summary` or `json`; default `summary`.

## Workflow

1. Parse YAML and validate Kubernetes schemas and kinds.
2. Check labels, requests and limits, probes, security contexts, service
   accounts, secrets, network exposure, and policy rules.
3. Run client-side and server-side dry runs when available.
4. Produce a diff before apply.
5. For apply, require explicit confirmation, apply the reviewed manifests, wait
   for rollout, and run health checks.
6. For rollback, identify the target revision, confirm scope, roll back, and verify health.

## Safety

- Never apply to a cluster without naming the context and namespace.
- Never delete resources implicitly.
- Never print secret values.
- Stop on failed rollout or degraded health unless rollback was explicitly enabled.

## Output

Return cluster target, resources checked, policy findings, diff summary, rollout
status, health checks, rollback status, and remaining risks.
