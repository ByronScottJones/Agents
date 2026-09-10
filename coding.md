# Coding Standards

Load this file when a task involves source files, scripts, or configuration.
Load the language-specific file named in `AGENTS.md` before editing that
language.

## Formatting and Linting

Use the formatter and linter defined by the applicable language file. Treat
those tools as authoritative and do not recommend conflicting style changes.

## Code Comments

- Prefer clear names and small functions over explanatory comments.
- Add a brief comment or docstring when a function’s purpose or non-obvious behavior would not be clear to a reader.
- Use comments to explain intent, constraints, tradeoffs, or non-obvious behavior.
- Avoid restating what the code already says.
- Keep comments accurate and update or remove them when the code changes.
- Use language-appropriate comment styles and follow the repository’s existing conventions.

## Documentation

- Keep documentation aligned with code and configuration changes.
- Prefer concise, task-focused documentation with clear headings and examples.
- Update README files, usage notes, and references when behavior, setup, or workflows change.
- Avoid adding documentation that simply repeats obvious code behavior.

## Input Validation

When a task adds a user-facing input field and does not define its validation
requirements, ask the user what validation to apply. Do not silently choose
free-form input or invent validation rules.

## Error Handling

- Handle errors where they can be acted on and surfaced meaningfully.
- Prefer explicit, actionable error messages over generic failures.
- Avoid swallowing errors unless there is a documented reason to do so.
- Use retries, fallbacks, or graceful degradation only when they are appropriate for the specific failure mode.
- Keep failure behavior consistent with the surrounding code and project conventions.

## Testing and Validation

- Test behavior, including unhappy paths, empty states, and invalid inputs.
- Match the project's existing test framework instead of introducing a second one.
- Add or update tests when changing behavior, fixing bugs, or covering regressions.
- Run the narrowest relevant executable check after each substantive edit.
- Preserve public APIs unless a breaking change is explicitly requested.
- Keep unrelated refactors out of the change.
- If tests are missing, add at least one focused test that covers the changed behavior or regression.

## Version Selection

- Use currently supported language SDKs, compilers, and libraries.
- Pin libraries and tools in project configuration instead of using `latest`.
- Prefer minimal version bumps unless a broader upgrade is needed.

## Code Generation

- Match the existing project conventions before applying defaults.
- Include the standard header or preamble required by the language file.
- Use existing snippets and helpers when they solve the same problem.
- Distinguish project conventions and code structure from formatting rules; defer formatting to the project’s formatter and linter.

## Maintenance and Compatibility

- Fix root causes instead of applying surface-level patches.
- Prefer the standard library for trivial functionality.
- Do not wrap every function in `try/catch`; handle errors where they can be acted on.
- Remove dead code or explain why it must remain.
- Do not use deprecated APIs unless migration is explicitly out of scope.
- Never hardcode credentials, account IDs, or environment-specific values.
- Treat compiler and linter warnings as errors unless there is a documented reason not to.

## Public API and Compatibility

- Document public APIs in markdown when other agents or contributors need to use them.
- Preserve backward compatibility unless a breaking change is requested.
- Document or call out breaking changes clearly when they are unavoidable.
- Keep input and output contracts stable unless the task explicitly changes them.
- Update examples and references when public behavior changes.

## Security Considerations

- Validate and sanitize untrusted input before use.
- Avoid shell injection, path traversal, and unsafe deserialization patterns.
- Do not log secrets or sensitive identifiers.
- Use the least-privilege approach when handling credentials, tokens, and permissions.
- Treat security-sensitive changes conservatively and follow existing project patterns.

## Concurrency and Timing

- Be explicit about timeouts, retries, and cancellation behavior when relevant.
- Avoid race conditions by using the project’s established synchronization patterns.
- Keep operations idempotent when they may be retried or run concurrently.
- Document ordering assumptions when they matter to correctness.
