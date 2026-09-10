# Coding Standards

Load this file when a task involves source files, scripts, or configuration.
Load the language-specific file named in `AGENTS.md` before editing that
language.

## Formatting and Linting

Use the formatter and linter defined by the applicable language file. Treat
those tools as authoritative and do not recommend conflicting style changes.

## Code Comments

- Prefer clear names and small functions over explanatory comments.
- Add a brief comment within a function when a function’s purpose, intent, or behavior is not obvious from the code.
- Use comments to explain intent, constraints, tradeoffs, or non-obvious behavior.
- Avoid restating what the code already says.
- Keep comments accurate and update or remove them when the code changes.
- Use language-appropriate comment styles and follow the repository’s existing conventions.

## Input Validation

When a task adds a user-facing input field and does not define its validation
requirements, ask the user what validation to apply. Do not silently choose
free-form input or invent validation rules.

## Testing and Validation

- Test behavior, including unhappy paths, empty states, and invalid inputs.
- Match the project's existing test framework instead of introducing a second one.
- Run the narrowest relevant executable check after each substantive edit.
- Preserve public APIs unless a breaking change is explicitly requested.
- Keep unrelated refactors out of the change.

## Version Selection

- Use currently supported language SDKs, compilers, and libraries.
- Pin libraries and tools in project configuration instead of using `latest`.

## Code Generation

- Match the existing project conventions before applying defaults.
- Include the standard header or preamble required by the language file.
- Use existing snippets and helpers when they solve the same problem.

## Design and Maintenance

- Fix root causes instead of applying surface-level patches.
- Prefer the standard library for trivial functionality.
- Do not wrap every function in `try/catch`; handle errors where they can be acted on.
- Remove dead code or explain why it must remain.
- Do not use deprecated APIs unless migration is explicitly out of scope.
- Never hardcode credentials, account IDs, or environment-specific values.
- Code generation warnings should generally be treated as errors, and be resolved rather than ignored.
