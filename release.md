---
title: "Release Engineering"
description: "Load this file when designing builds, releases, deployments, or environment promotion."
---

# Release Engineering

Load this file when designing builds, releases, deployments, or environment
promotion.

## Artifacts

- Build once and promote the same immutable artifact through environments.
- Record version, source revision, dependency state, and build provenance.
- Generate release notes that identify behavior changes, migrations, risks, and rollback steps.
- Do not embed environment secrets in build outputs.

## Deployment

- Prefer backward-compatible changes and independently deployable steps.
- Coordinate database migrations so old and new application versions can coexist.
- Use feature flags for risky behavior changes when appropriate.
- Define health checks, rollout gates, rollback, and roll-forward procedures.

## Change Control

- Automate repeatable approvals and checks; reserve manual approval for meaningful risk.
- Separate normal, emergency, and routine maintenance changes.
- Verify the deployed artifact and record deployment evidence.
