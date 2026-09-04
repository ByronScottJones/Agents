# JavaScript / TypeScript

Open this file only when writing or modifying JavaScript or TypeScript code. General rules stay in the root `AGENTS.md`.

## Formatting & Linting

- Formatter: `prettier` (authoritative — never suggest conflicting style).
- Linter: `eslint`.

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

## Code Generation Defaults

- Named exports preferred over default exports.
