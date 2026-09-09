---
name: remediate-dependency-vulnerabilities
user-invocable: false
description: "Use when scanning dependencies for CVEs, auditing software supply chains, or upgrading vulnerable packages. Detects package managers, reports findings, optionally proposes upgrades, and validates the result without changing files unless explicitly requested."
---

# Remediate Dependency Vulnerabilities

Scan declared and lockfile dependencies, identify safe remediation paths, and
validate upgrades. This skill implements `security.md` and `dependencies.md`.

## Parameters

- `projectPath`: project root; default workspace root.
- `packageManagers`: explicit list or auto-detect.
- `severity`: `critical`, `high`, `medium`, `low`, or `all`; default `high`.
- `includeTransitive`: default `true`.
- `excludeDevDependencies`: default `false`.
- `mode`: `report`, `propose`, or `apply`; default `report`.
- `validateAfter`: `none`, `build`, `test`, or `build+test`; default `build+test` for `apply`.
- `format`: `summary`, `json`, or `sarif`; default `summary`.

## Workflow

1. Detect manifests, lockfiles, package managers, and project conventions.
2. Run the safest available scanner for each ecosystem.
3. Normalize findings by package, version, advisory, severity, and fix version.
4. Distinguish direct, transitive, runtime, and development dependencies.
5. In `propose` mode, identify compatible upgrade paths and breaking changes.
6. In `apply` mode, require confirmation, update only the selected dependencies,
   and preserve lockfile consistency.
7. Run the requested build and test validation.
8. Report unresolved findings, exceptions, and rollback guidance.

## Safety

- Never weaken a security scanner to obtain a green result.
- Do not upgrade across a major version without identifying compatibility risk.
- Never commit secrets, generated credentials, or private scan data.
- Treat scanner results as evidence; verify false positives and compensating controls.

## Output

Return package managers, scan timestamp, findings by severity, proposed or
applied versions, validation results, skipped findings, and remaining risk.
