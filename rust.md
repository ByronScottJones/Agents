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

## Rust Version

- Rust supports only the latest stable release and has no long-term-support line, so "supported" means "recent".
- Record the minimum supported version once, as `rust-version` in the workspace `Cargo.toml` (`[workspace.package]`), and make CI and any `rust-toolchain.toml` agree with it.
- **New projects:** use the latest stable toolchain, set `rust-version` to it, and use the 2024 edition. Use an older version only when a required dependency does not build on the latest stable or needs an older compiler, and put the reason in a comment beside `rust-version`.
- **Existing projects:** leave `rust-version` alone until one of these is true, then move to the latest stable (not just the next release):
  - a dependency, a security fix or a needed language or library feature requires a newer compiler;
  - `rust-version` is more than six months (about four releases) behind the latest stable.
- **Upgrading:**
  1. Read the release notes for each release crossed, especially "Compatibility Notes", and check the edition guide if the edition changes.
  2. Update `rust-version`, `rust-toolchain.toml` and CI together.
  3. Run the full "Required Verification Pipeline" and `cargo deny check`; fix new `clippy` warnings and deprecations rather than allowing them.
  4. Make the upgrade its own change, apart from feature work, and say in it what broke or was avoided.
- For a published library, raising `rust-version` affects its users: say so in the changelog and release notes.

## Formatting & Linting

- Formatter: `rustfmt` (authoritative — never suggest conflicting style).
- Linter: `clippy`.
- Validate with the commands in "Required Verification Pipeline" below.
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
- Prefer `assert_eq!` / `assert_ne!` over raw `assert!` for readable failures. Use `assert!(matches!(..))` for patterns; `std::assert_matches!` is unstable, so use the `assert_matches` crate only if the project already depends on it.

## Conventions

- Derive `Debug` on all public types.
- Use `tracing` for structured logging/instrumentation.
- Avoid `unsafe` — if required, isolate it and document the invariants.
- Test public documentation with `cargo test --doc` whenever the workspace has a library target. Skip it for binary-only projects, where it fails with "no library targets found".

## Code Generation Defaults

- Explicit `use` imports — no wildcard imports except in tests.

## Required Verification Pipeline

Always run these commands before finalizing changes. All warnings are treated as
errors (`-D warnings`). Run the last command only when the workspace has a
library target.

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

## Dependency Auditing (`deny.toml`)

- **Dependency Hygiene:**
  - Never use wildcard versions (`*`) in `Cargo.toml`.
  - Prefer pure-Rust dependencies (e.g., `rustls` over `openssl-sys`) to simplify cross-compilation and auditing.
  - All added crates must use a license on the `allow` list in the standard `deny.toml` below, which is
    the single source of truth for permitted licenses, and pass `cargo deny check` with zero advisories or yanked versions.
- A pre-commit hook and CI workflow that run this pipeline are in `snippets/rust/`; see `snippets/SNIPPETS.md`.

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
