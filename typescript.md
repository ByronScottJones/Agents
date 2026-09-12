---
title: "JavaScript / TypeScript"
description: "Open this file only when writing or modifying JavaScript or TypeScript code."
applies_to: ["*.ts", "*.tsx", "*.js", "*.jsx"]
---

# JavaScript / TypeScript

Open this file only when writing or modifying JavaScript or TypeScript code. General rules stay in the root `AGENTS.md`.

## Configuration

- Use `env-paths` or an equivalent maintained library for platform-specific application directories.
- Use `cosmiconfig` or the project's established loader for layered configuration files.
- Use `dotenv` only for local development and never expose secrets through client-side environment variables.

## Formatting & Linting

- Formatter: `prettier` (authoritative — never suggest conflicting style).
- Linter: `eslint`.
- Validate with the project's `tsc --noEmit`, lint, format-check, and test commands.

## Error Handling

- TypeScript: `strict` mode always on. No `any` without a `// eslint-disable` comment and justification.
- Async errors must be caught — no unhandled promise rejections.
- Prefer `unknown` over `any` for catch clauses: `catch (e: unknown)`.

## Testing

- Jest or Vitest — match project convention, don't mix.
- React: `@testing-library/react` — test behavior, not implementation.
- Always test the unhappy path (errors, empty states, null inputs).

## Conventions (TypeScript)

- `interface` for object shapes; `type` for unions/intersections/aliases.
- Explicit return types on public functions.
- Avoid enum — prefer `as const` objects or union string literals.
- Use `zod` (or equivalent) for runtime validation of external data.
- Validate env vars at startup with a schema — never `process.env.FOO` inline without a guard.
- Keep server-only secrets out of browser bundles and public environment-variable prefixes.
- Use `AbortController` and timeouts for network requests.
- Match the project's package manager and lockfile; do not mix npm, pnpm, and Yarn workflows.

## Code Generation Defaults

- Named exports preferred over default exports.
