# Rust

Open this file only when writing or modifying Rust code. General rules stay in the root `AGENTS.md`.

## Configuration

- Use the `directories` crate for platform-specific application paths when the standard library is insufficient.
- Deserialize configuration with `serde` and use TOML or JSON as appropriate.
- Use `figment` or `config` when configuration must merge files, environment variables, and defaults.

## Formatting & Linting

- Formatter: `rustfmt` (authoritative — never suggest conflicting style).
- Linter: `clippy`.
- Validate with `cargo fmt --check`, `cargo clippy --all-targets --all-features -- -D warnings`, and `cargo test`.
- Use `cargo audit` or the project's established dependency scanner when appropriate.

## Error Handling

- Use `?` for propagation — never `.unwrap()` or `.expect()` in library code.
- `.unwrap()` / `.expect()` are acceptable in tests and `main` with a meaningful message.
- Prefer `thiserror` for library error types; `anyhow` for application/binary error handling.
- Never silence errors with `let _ = ...` unless the intent is explicitly documented.

## Testing

- Unit tests in the same file under `#[cfg(test)]`.
- Integration tests in `tests/` directory.
- Use `rstest` for parameterized tests.
- Prefer `assert_eq!` / `assert_matches!` over raw `assert!` for readable failures.

## Conventions

- Derive `Debug` on all public types.
- Use `tracing` for structured logging/instrumentation.
- Avoid `unsafe` — if required, isolate it and document the invariants.
- Test public documentation with `cargo test --doc` when the project publishes a library.

## Code Generation Defaults

- Explicit `use` imports — no wildcard imports except in tests.
