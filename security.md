# Security Engineering

Load this file for security-sensitive design, implementation, review, or
deployment work.

## Secure Design

- Identify assets, trust boundaries, abuse cases, and security assumptions.
- Use authentication for identity and authorization for access decisions.
- Apply least privilege to users, services, credentials, and deployment roles.
- Validate untrusted input at boundaries and encode output for its destination.
- Prefer secure defaults and fail closed for authorization decisions.

## Secrets and Data

- Never commit or log credentials, tokens, private keys, or sensitive personal data.
- Use an approved secret manager and rotate credentials when exposure is suspected.
- Classify sensitive data and minimize collection, retention, and access.
- Redact secrets from errors, logs, traces, test fixtures, and support bundles.

## Supply Chain

- Pin dependencies and actions where reproducibility and integrity require it.
- Scan dependencies, container images, and generated artifacts for known issues.
- Review licenses, provenance, maintenance status, and transitive dependencies.
- Record accepted risks with an owner, rationale, and expiration date.

## Verification

- Add security tests for authorization, input validation, and abuse-prone paths.
- Review threat-model changes when architecture or trust boundaries change.
- Treat security guidance as supplementary to platform-specific controls.
