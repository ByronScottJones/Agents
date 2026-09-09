# Kubernetes

Load this file only when designing or operating Kubernetes workloads.

## Workloads

- Choose the smallest suitable workload type: Deployment, Job, CronJob, or StatefulSet.
- Define resource requests, limits, probes, graceful termination, and disruption behavior.
- Use namespaces, labels, and ownership metadata consistently.
- Use least-privilege service accounts and restrictive security contexts.

## Networking and Secrets

- Restrict traffic with NetworkPolicies where the cluster supports them.
- Store secrets in an approved external secret system when possible.
- Define ingress, certificates, DNS, and exposure boundaries explicitly.
- Do not expose management endpoints publicly.

## Operations

- Define autoscaling signals, upgrade strategy, rollback, and capacity limits.
- Test manifests with schema validation and policy checks before deployment.
- Keep Helm, Kustomize, or raw manifests reproducible and environment-specific only at the configuration layer.
