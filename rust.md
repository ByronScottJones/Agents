---
title: "Rust"
description: "Open this file only when writing or modifying Rust code."
applies_to: ["*.rs"]
---

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

## Formatting Config (`rustfmt.toml`) & Verification Commands

Always run verification commands before finalizing changes. All warnings are treated as errors (`-D warnings`).

## Required Verification Pipeline

```bash
cargo fmt --all -- --check
cargo clippy --workspace --all-targets --all-features -- -D warnings
cargo test --workspace --all-targets --all-features
cargo test --workspace --doc --all-features
```

## Standard `rustfmt.toml`

```toml
edition = "2024"
style_edition = "2024"
max_width = 100
hard_tabs = false
tab_spaces = 4
newline_style = "Unix"
use_small_heuristics = "Default"
reorder_imports = true
reorder_modules = true
remove_nested_parens = true
merge_derives = true
use_try_shorthand = true
use_field_init_shorthand = true
force_explicit_abi = true
```

## Dependency Auditing (`deny.toml`) & Git Hooks

- **Dependency Hygiene:**
  - Never use wildcard versions (`*`) in `Cargo.toml`.
  - Prefer pure-Rust dependencies (e.g., `rustls` over `openssl-sys`) to simplify cross-compilation and auditing.
  - All added crates must use permissive SPDX licenses (`MIT`, `Apache-2.0`, `BSD-3-Clause`, `ISC`, `Zlib`, `MPL-2.0`, `Unicode-3.0`) and pass `cargo deny check` with zero advisories or yanked versions.

## Standard `deny.toml`
```toml
[graph]
all-features = true

[advisories]
version = 2
unmaintained = "workspace"
unsound = "all"
yanked = "deny"

[licenses]
version = 2
allow = [
    "Apache-2.0",
    "Apache-2.0 WITH LLVM-exception",
    "BSD-2-Clause",
    "BSD-3-Clause",
    "BSL-1.0",
    "CC0-1.0",
    "ISC",
    "MIT",
    "MPL-2.0",
    "Unicode-3.0",
    "Unicode-DFS-2016",
    "Zlib",
]

[bans]
multiple-versions = "warn"
wildcards = "deny"

[sources]
unknown-registry = "deny"
unknown-git = "deny"
allow-registry = ["https://github.com/rust-lang/crates.io-index"]
```
