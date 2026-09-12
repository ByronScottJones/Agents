---
title: "Containers"
description: "Load this file when creating, reviewing, or operating container images."
applies_to: ["Dockerfile", "*.dockerfile", "docker-compose*.yml", "compose.yml"]
---

# Containers

Load this file when creating, reviewing, or operating container images.

## Images

- Use minimal, trusted base images and pin them by version or digest where practical.
- Use multi-stage builds and keep compilers and build tools out of runtime images.
- Add a `.dockerignore` that excludes credentials, source-control metadata, and build output.
- Generate an SBOM and scan images before release.

## Runtime

- Run as a non-root user with a read-only filesystem when the application permits it.
- Do not put secrets in Dockerfiles, image layers, build arguments, or image labels.
- Handle `SIGTERM` and exit cleanly; define health checks appropriate to the service.
- Declare resource requests and limits in the deployment environment.
- Log to standard output and standard error rather than container-local log files.

## Reproducibility

- Make builds deterministic and use lockfiles or equivalent dependency snapshots.
- Tag images with immutable release identifiers in addition to human-readable tags.
- Verify the image actually used in deployment matches the built artifact.
