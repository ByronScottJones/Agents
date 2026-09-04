# Coding Rules

These rules apply whenever a session involves source files, scripts, or config
— open this file once that's the case. Language-specific rules live in
separate files under `agents/` — see [Language-Specific
Instructions](#language-specific-instructions) below. Open the relevant file
only when you are actually writing or modifying code in that language.

## Git Hygiene

Whenever working on a code project (any session involving source files, scripts, or config):

- Check for git on first file change. Before making any edits, check whether the project directory is a git repo (`git rev-parse --git-dir`).
- Offer to initialise if missing. If there is no git repo, ask the user whether to create one.
  If they agree, run `git init`, create a sensible `.gitignore` for the language/stack, and make an initial commit of existing files.
- Create standard Git Repo files. If files such as Readme.md, Engineering.md, Agents.md do not exist, create them.
  Update as needed prior to each commit to ensure the information stays current.
- Create a git branch for each feature. Complete all coding tasks for that feature and commit, then create a pull request.
  Wait for the user to complete the review of that Pull Request. Once the Pull Request is merged, checkout main, pull changes,
  then continue the cycle with the next feature/task. 
- Commit after every code update. After each round of file changes (new file, edit, delete), stage and commit with a short, descriptive message.
  Use conventional-commit style where it fits (`feat:`, `fix:`, `refactor:`, `docs:`, `chore:`).
- Never leave changes uncommitted at the end of a task. Before wrapping up, confirm all edits are committed.
- Exception: skip all of the above when the task was explicitly framed as read-only, research-only, or "do not commit"/"do not implement" — a narrower per-task instruction always overrides this default. Do not stage or commit anything in that case, even if file changes happened.

---

## Formatting & Linting

Always assume these formatters are authoritative. Never suggest changes that would conflict with them.

| Language | Formatter | Linter |
|---|---|---|
| Go | `gofmt` / `goimports` | `golangci-lint` |
| Rust | `rustfmt` | `clippy` |
| C# | `dotnet format` | Roslyn analyzers |
| JS/TS | `prettier` | `eslint` |
| HTML | `prettier` | — |
| Bash/Zsh | `shfmt` | `shellcheck` |
| Terraform | `terraform fmt` | `tflint` |

- Never suggest style changes that a formatter would handle automatically.

Language-specific formatter and linter notes (e.g. `terraform fmt -recursive`,
`shellcheck` policy) live in the corresponding language file.

---

## Configuration

- Applications should store config files, caches, etc in user/application directories. "$HOME/.<app_name>/" should be the preferred directory, or equivalent depending on operating system.
- Config files should be human readable wherever possible.
- Applications should use composition to determine configuration. Read defaults from config files, but allow Environment Variables and Command Line Arguments to override where it makes sense.

---

## CLI and TUI Applications

- Review tui.md for TUI Design Requirements
- Always include help options.
  - Generate a linux manpage file.
  - Generate an HTML help file.
  - -h, -H, --help --HELP, -? should display help at the command line. It should be human readable, but also in a format useful to AI.
- TUIs must always take the active terminal window size into account.
  - Handle the framework's resize event (e.g. `tea.WindowSizeMsg` in Bubble Tea) and track current width/height in the model.
  - Any list, table, or picker whose contents can exceed the available height must scroll (windowed rendering that keeps the cursor in view, plus a "N more above/below" indicator) rather than letting content overflow or get silently cut off.
  - Size the scrollable window off the tracked height, not a fixed guess, and re-derive it on every resize.

## Input Validation

- When a task asks you to add a user-facing input field (CLI flag, TUI text input, form field, etc.) and does not specify what validation it needs, ask the user what validation to apply before implementing it — don't silently default to freeform/no validation, and don't silently invent a validation rule that wasn't asked for either.

---

## Topic Specific Instructions

These topics are common across languages.

| Topic | Open this file |
|-------|----------------|
| Configuration | [`configuration.md`](configuration.md) |
| GUI Automation, UI Control | [`gui_automation`](gui_automation.md) |
| TUI Text User Interface Design | [`tui`](tui.md) |
| CMAKE, MAKE Files | [`cmake_make.md`](makefile.md) |
| Github Actions | [`githubactions.md`](githubactions.md) |
| Go, Golang | [`go.md`](go.md) |
| Twelve Factor Architecture | [`twelvefactors`](twelvefactors.md) |

---

## Language-Specific Instructions

When you write or modify code in one of the languages below, **open the matching file
first** and follow its rules for error handling, testing, conventions, and code
generation. Do not load these files for languages you are not currently working in.

| Language | Open this file |
|---|---|
| Bash, Zsh, Shell| [`shell.md`](shell.md) |
| C#, Dotnet | [`dotnet.md`](dotnet.md) |
| Go, Golang | [`go.md`](go.md) |
| HTML | [`html.md`](html.md) |
| JavaScript, TypeScript | [`typescript.md`](typescript.md) |
| Rust | [`rust.md`](rust.md) |
| Terraform | [`terraform.md`](terraform.md) |
| Powershell | [`powershell.md`](powershell.md) |
| Rust/Tauri Stack | [`tauri.md'](tauri.md) |

---

## Version Selection

Language SDKs, compilers, and libraries should be the current supported versions.
Where libraries are defined in code or configuration, they should be pinned to those versions, rather than @latest.

---

## Code Snippets

- snippets/snippet.md lists a variety of code snippets by language,
  use those snippets to add functionality without having to reinvent it.

---

## Code Generation Defaults

When generating new files, follow these unless the project already has a convention.
Per-language details are in each language file; the shared rule is:

- Match the existing project's conventions first.
- Include the standard header/preamble the language file specifies (package declaration, shebang, namespace, `versions.tf`, etc.).

---

## What to Avoid

- Don't wrap every function in a try/catch — handle errors where they can be acted on.
- Don't suggest adding dependencies for trivial things the standard library covers.
- Don't leave dead code — remove it or comment why it's kept.
- Don't use deprecated APIs unless migration is explicitly out of scope.
- Never hardcode credentials, account IDs, or environment-specific values outside var files or secrets managers.
