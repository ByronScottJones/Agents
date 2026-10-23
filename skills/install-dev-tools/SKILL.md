---
name: install-dev-tools
user-invocable: true
description: "Use when setting up a new machine or checking that the coding tools required by the agent guidance (Rust, Go, .NET, Node, Python, shell, PowerShell, Terraform, containers, Tauri, and workflow tools) are installed. Audits what is installed on macOS, Windows, or Ubuntu/Debian, reports gaps, and installs each tool group only after confirmation."
---

# Install Dev Tools

Guide the user through installing the tools required by the languages and
topics in `~/.agents`. The tool catalog is `tools.toml` in this skill's
directory; this file is the workflow. Topic files remain the source of policy:
`tools.toml` names the guidance file each group supports.

## Parameters

- `groups`: group ids from `tools.toml`, or `all`; default is to ask.
- `mode`: `audit` (report only) or `install`; default `install`, which always
  audits first.
- `includeOptional`: install tools whose `note` calls them optional; default `false`.

## Workflow

1. **Detect the platform.** Use `uname -s` and `/etc/os-release` (or
   `$env:OS` in PowerShell) to select `macos`, `windows`, or `ubuntu`. Treat any
   Debian-family distribution as `ubuntu`. Stop and report for any other OS.
2. **Check the package manager.** Require Homebrew (macOS), winget (Windows),
   or apt (Ubuntu). If it is missing, tell the user how to install it and stop;
   do not install a package manager yourself.
3. **Choose groups.** List the `[[group]]` entries with their titles and ask
   which to cover. Pre-select groups for languages the user names. Include a
   group's `requires` dependencies even if their group was not chosen, and say
   so.
4. **Audit.** For each tool in the chosen groups, run its `check` command (use
   `check_<os>` when present) and record the version or "missing". Run checks
   one at a time; a failing check is expected output, not an error. Compare
   against `min_version` when set and mark older versions "outdated". Never
   install or upgrade during this step.
5. **Report the gaps.** Show one table per group: tool, status (installed,
   outdated, missing, manual), version, and what needs doing. Tools with no
   install command for this OS are "manual": show their `note`. If `mode` is
   `audit`, stop here.
6. **Install one group at a time.** For each group that has gaps, in the order
   core, language groups, then the rest:
   1. Print the exact commands, with `{winget_flags}` expanded from `[meta]`.
   2. Flag anything that needs attention before the user answers: `sudo`,
      remote scripts (`remote_script`), shell-profile changes, GUI installers,
      large downloads, and any `note`.
   3. Ask for confirmation for that group. Do not batch confirmations across
      groups.
   4. Run the commands in `requires` order, one tool at a time. If a command
      fails, stop that group, show the error, and ask whether to skip, retry, or
      abort. Do not try alternative install methods on your own.
   5. Re-run each tool's `check` and report the new status. If a tool installed
      but its check still fails, look for a PATH problem (for example
      `~/go/bin` or `~/.local/bin`) and offer the profile change shown in the
      tool's `note`.
7. **Corporate TLS.** Only when a download or install failed with a certificate
   error, or the user asks, run the `[tls]` test. If it fails and
   `~/.ssh/zscaler/zscaler.pem` exists, follow `ssl-tls.md` and the `[tls]`
   section of `tools.toml`: build the bundle if needed, then offer the
   per-tool settings one at a time. Show the exact line to be added to the shell
   profile and get confirmation before editing it. Never set `SSL_CERT_FILE` to
   the Zscaler file alone; "replaces" settings need the combined bundle.
8. **Final report.** List installed, already present, outdated, skipped,
   failed, and manual items. Remind the user to open a new shell so PATH changes
   take effect, and to run `gh auth login` themselves if `gh` was installed.

## Safety

- Never run `sudo` without telling the user which command and why, and never
  run it for a group the user has not confirmed.
- Never pipe a download into a shell. For `remote_script` commands, download to
  a file first, show the URL and file size, tell the user they can read the
  script, and run it only after confirmation.
- Do not upgrade, downgrade, or uninstall a tool that is already present
  unless the user asks. For an outdated tool, offer the upgrade as its own
  confirmed step.
- Do not edit shell profiles, system PATH, or registry settings without showing
  the exact change first.
- Do not authenticate to any service, enter credentials, or write tokens or
  certificates into a repository. Authentication steps such as `gh auth login`
  are for the user to run.
- Do not add the user to groups (for example `docker`) or change system
  security settings.
- Stay within the catalog. If the user wants a tool that is not in
  `tools.toml`, add a `[[tool]]` entry only after confirming the commands, and
  say you did.
- Installation needs network access and writes outside the project. Expect
  permission prompts, and ask the user before running any command outside the
  sandbox. The default sandbox network allowlist covers GitHub and npm only;
  Homebrew, crates.io, and the Go module proxy are blocked, so those installs
  fail inside it. A 403 "CONNECT tunnel failed" or a read-only `/opt/homebrew`
  means the sandbox blocked the command, not that the system is broken: retry
  outside the sandbox with the user's confirmation, and never run the
  `sudo chown` that Homebrew suggests in that case.
- Check that a command really succeeded: piping through `tail` hides the exit
  status, so use `${PIPESTATUS[0]}` or re-run the tool's `check`.

## Sandbox network allowlist

Installs that run inside the Claude Code sandbox need their download hosts in
`sandbox.network.allowedDomains` in the user's `settings.json` (or via the
`/sandbox` command). This skill never edits that setting; suggest the hosts and
let the user decide. Homebrew is deliberately left out, because it also needs
write access to `/opt/homebrew`, which the sandbox does not grant.

| Needed for | Hosts |
| --- | --- |
| `cargo install` | `crates.io`, `index.crates.io`, `static.crates.io` |
| `go install` | `proxy.golang.org`, `sum.golang.org` |
| Go toolchain downloads (optional) | `go.dev`, `dl.google.com` |
| Release binaries from GitHub, such as tflint | `*.githubusercontent.com` (GitHub redirects downloads there; `*.github.com` does not cover it) |
| Python packages (optional) | `pypi.org`, `files.pythonhosted.org` |
| PowerShell modules (optional, not verified) | `www.powershellgallery.com` |

GitHub and npm hosts are normally already allowed. Allowing a package registry
lets builds run inside the sandbox fetch and execute third-party code such as
Rust build scripts; the sandbox's filesystem limits contain it, but tell the
user that trade-off.

## Maintaining the catalog

- Add or change tools in `tools.toml` only; keep the field documentation at the
  top of that file accurate.
- Keep each `guidance` value in step with the topic files in `AGENTS.md`
  routing. When a topic file adds a required tool, add it here in the same
  change.
- Keep commands pinned or locked where the ecosystem allows (for example
  `cargo install --locked`). Tools installed with `@latest` carry a note that
  projects must pin their own version.
- Verify the file still parses after editing:

  ```bash
  python3 -I -c "import tomllib; tomllib.load(open('tools.toml','rb'))"
  ```
