# APIs and Service Contracts

Load this file when designing or changing HTTP, RPC, event, or service contracts.

## Contracts

- Define ownership, consumers, compatibility expectations, and versioning policy.
- Use protocol semantics consistently for status, errors, retries, and idempotency.
- Document authentication, authorization, rate limits, timeouts, and payload limits.
- Publish an OpenAPI, schema, or equivalent contract when the protocol supports it.

## Reliability

- Make retries safe and bounded; use idempotency keys for repeatable writes when needed.
- Propagate cancellation and correlation identifiers.
- Design pagination, filtering, ordering, and partial failure behavior explicitly.
- Avoid leaking internal implementation details or sensitive data in responses.

## Verification

- Use contract tests for independently deployed consumers and providers.
- Test invalid input, authorization failures, timeouts, compatibility, and overload behavior.
