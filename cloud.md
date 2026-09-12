---
title: "Cloud Architecture"
description: "Load this file when designing or operating cloud-hosted systems."
---

# Cloud Architecture

Load this file when designing or operating cloud-hosted systems. Provider files
may add implementation-specific rules.

## Design

- Choose regions and availability zones based on latency, resilience, data residency, and cost.
- Prefer managed services when their operational and security characteristics fit the workload.
- Define identity, network boundaries, quotas, and failure domains before deployment.
- Separate environments and apply least privilege to deployment and runtime identities.

## Operations

- Tag resources for ownership, environment, service, and cost allocation.
- Define backup, recovery, observability, and upgrade responsibilities.
- Review service limits, egress costs, and dependency concentration.
- Keep infrastructure reproducible through approved infrastructure-as-code workflows.
