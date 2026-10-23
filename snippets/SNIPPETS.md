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
  - toggles the value of the Terminal Configuration Flag useOptionAsMetaKey for the current profile.

The next two are standalone commands, not fragments. They need `jq` and `gh`,
read tokens from `~/.secrets/secrets.json`, and are installed by symlinking them
into a directory on `PATH` (for example `~/.local/bin`); see `git.md` for the
setup and the secret naming convention.

- shell/ghx
  - runs `gh` as the GitHub account that owns the target repository, giving the token only to that one process.
- shell/git-credential-ghx
  - git credential helper that supplies the token of the account that owns the repository being accessed.

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
  - Git pre-commit hook that runs `cargo fmt`, Clippy, `cargo deny` (required; the hook fails if it is not installed), tests, and doc tests (only when a library target exists) when Rust or Cargo files are staged. Copy to `.git/hooks/pre-commit` and make it executable.
- rust/github_workflows_ci.yml
  - GitHub Actions CI workflow with SHA-pinned actions that checks formatting, runs Clippy, runs unit, integration, and doc tests, and runs `cargo deny check`. Runs manually by default (`workflow_dispatch`); automatic triggers are defined but their jobs run only when the repository variable `AUTO_CI` is `true`. Copy to `.github/workflows/ci.yml`.

## C# / Terminal.Gui (`terminal_gui_themes/`)

A Terminal.Gui theme-switching example. The first three files work together and
require the `Terminal.Gui` NuGet package, .NET `System.Text.Json`, and a project
with `<Nullable>enable</Nullable>` and `<ImplicitUsings>enable</ImplicitUsings>`.
The code targets Terminal.Gui 2.5.0 on `net10.0` (`IApplication`, `Scheme`,
`SchemeManager`, `DropDownList`) and builds cleanly with warnings as errors.
Terminal.Gui v1 code uses a different API (`Application.Init()`, `ColorScheme`,
`ComboBox`); verify against the version you pin before upgrading.

- terminal_gui_themes/program.cs
  - example application that loads the themes and lets the user switch the active color scheme from a `DropDownList`.
- terminal_gui_themes/themeloader.cs
  - `ThemeLoader` that reads `themes.json`, builds Terminal.Gui color schemes, and falls back to the nearest 16-color ANSI value when TrueColor is unavailable.
- terminal_gui_themes/themes.json
  - theme definitions (for example One Dark Pro, Dracula, Tokyo Night, Catppuccin Mocha) with per-component normal, focus, and hot colors.
- terminal_gui_themes/popular_color_themes.json
  - reference token dictionary of popular palettes with hex, ANSI 16/256, and closest CSS color values; not read by `ThemeLoader`.
