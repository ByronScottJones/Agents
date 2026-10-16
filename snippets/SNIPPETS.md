# Code Snippets

- This directory contains code fragments to assist in development.
- Snippets are intended to be incorporated into larger projects, not run as
  standalone programs.
- Review each snippet's required imports, package context, error-handling
  conventions, and integration assumptions before using it.
- Formatting checks are useful, but standalone compilation is not required.
- Snippets are examples, not production-ready libraries. Verify dependencies,
  platform support, permissions, and error handling before reuse.
- Snippets are grouped in one subdirectory per language or topic. Paths below
  are relative to this directory.
- Keep this catalog synchronized with the files in this directory tree.

## Bash / Zsh (`shell/`)

- shell/checkOptionasMetaKey.sh
  - checks if the current terminal session supports using the Option key as a Meta/Alt key.
- shell/toggle-useOptionAsMetaKey.sh
  - toggles the value of the Terminal Configuration Flag useOptionAsKMetaKey for the current profile.

## Go (`go/`)

The Go snippets require Go modules and any imports shown in their file. Format
Go snippets with `gofmt` before reuse. Terminal protocol snippets require a
real terminal input/output device and should be tested with a pseudoterminal.

- go/checkOptionasMetaKey.go
  - checks if the current terminal session supports using the Option key as a Meta/Alt key.
- go/isKittyKeyboardAvailable.go
  - checks if the Kitty Terminal Keyboard Protocol is available.
- go/isAltKeyActive.go
  - checks if the current Kitty keyboard flags enable Alt key tracking.
- go/enableAltKeys.go
  - Enable the Alt Key in the Terminal.
- go/disableAltKeys.go
  - Disable the Alt Key in the Terminal.

## Rust (`rust/`)

The Rust files are project configuration templates. Copy each one into the
target repository at the location noted, and rename it where indicated.

- rust/rustfmt.toml
  - `rustfmt` settings (2024 edition, 100-column width, 4-space indent). Copy to the repository root as `rustfmt.toml`.
- rust/cargo_deny.toml
  - `cargo-deny` settings for advisories, licenses, and sources. Copy to the repository root as `deny.toml`; run with `cargo deny check`.
- rust/githook_precommits_rust
  - Git pre-commit hook that runs `cargo fmt`, Clippy, `cargo deny` (if installed), and tests when Rust or Cargo files are staged. Copy to `.git/hooks/pre-commit` and make it executable.
- rust/github_workflows_ci.yml
  - GitHub Actions CI workflow that checks formatting, runs Clippy, and runs unit, integration, and doc tests. Copy to `.github/workflows/ci.yml`.

## C# / Terminal.Gui (`terminal_gui_themes/`)

A Terminal.Gui theme-switching example. The three files work together and
require the `Terminal.Gui` NuGet package and .NET `System.Text.Json`.

- terminal_gui_themes/program.cs
  - example application that loads the themes and lets the user switch the active color scheme from a `ComboBox`.
- terminal_gui_themes/themeloader.cs
  - `ThemeLoader` that reads `themes.json`, builds Terminal.Gui color schemes, and falls back to the nearest 16-color ANSI value when TrueColor is unavailable.
- terminal_gui_themes/themes.json
  - theme definitions (for example One Dark Pro, Dracula, Tokyo Night, Catppuccin Mocha) with per-component normal, focus, and hot colors.
