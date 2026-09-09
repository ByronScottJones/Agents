---
name: build-and-scan-container-image
user-invocable: false
description: "Use when building, validating, scanning, or generating an SBOM for a container image. Checks Dockerfile safety, base images, secrets, non-root execution, vulnerabilities, reproducibility, and immutable tagging."
---

# Build and Scan Container Image

Implement `containers.md` as a build-and-release gate.

## Parameters

- `dockerfilePath`: default `Dockerfile`.
- `contextPath`: default directory containing the Dockerfile.
- `image`: optional image tag.
- `registry`: optional push target.
- `scan`: default `true`.
- `sbom`: default `true`.
- `maxSeverity`: default `high`.
- `push`: default `false`.
- `format`: `summary`, `json`, or `sarif`; default `summary`.

## Workflow

1. Parse the Dockerfile and inspect `.dockerignore` and build context.
2. Check base-image support, multi-stage builds, secret leakage, user identity,
   ports, health checks, and reproducible dependency inputs.
3. Build without pushing unless explicitly requested.
4. Scan the resulting image with the configured scanner and generate an SBOM.
5. Verify runtime user, exposed capabilities, and immutable digest.
6. Push only after explicit confirmation and a passing policy gate.

## Safety

- Never place secrets in Dockerfiles, layers, arguments, or labels.
- Never push `latest` as the only release identifier.
- Default to no registry mutation.
- Report scanner limitations and unavailable checks.

## Output

Return image identity, Dockerfile findings, vulnerability counts, SBOM path,
non-root and runtime checks, digest, push status, and blockers.
