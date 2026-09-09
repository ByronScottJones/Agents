---
name: validate-github-actions
user-invocable: false
description: "Use when auditing or validating GitHub Actions workflows for syntax, action pinning, least-privilege permissions, fork safety, caching, artifacts, matrices, and CI security. Defaults to read-only analysis; mutations require explicit confirmation."
---

# Validate GitHub Actions

Audit workflow files against `githubactions.md`, `security.md`, and repository
conventions.

## Parameters

- `workflowPath`: workflow directory or file; default `.github/workflows`.
- `mode`: `syntax`, `security`, `best-practices`, or `full`; default `full`.
- `requireSHA`: default `true`.
- `auditPermissions`: default `true`.
- `checkForks`, `checkMatrices`, `checkCaching`, `checkArtifacts`: default `true`.
- `fixUnpinned`: default `false`.
- `format`: `summary`, `json`, or `sarif`; default `summary`.

## Workflow

1. Discover YAML workflow files and parse them without executing workflows.
2. Validate triggers, jobs, matrices, expressions, permissions, and dependencies.
3. Check action references for immutable SHAs and readable version comments.
4. Audit token permissions, fork behavior, secret exposure, concurrency,
   timeouts, caching, artifacts, environments, and deployment gates.
5. When `fixUnpinned` is true, propose pinning changes. Apply them only after
    explicit confirmation and record each resolved action SHA.
6. Re-run validation and return findings by severity.

## Safety

- Never execute a workflow as part of static validation.
- Do not expose secrets from workflow files or logs.
- Treat fork workflows as untrusted.
- Do not rewrite action references without recording the resolved version.

## Output

Return files checked, syntax status, security findings, best-practice findings,
changes applied, unresolved risks, and CI recommendations.
