---
title: "Command-Line Interfaces"
description: "Load this file when designing or modifying the command-line arguments, help, or output of any application."
---

# Command-Line Interfaces

Load this file when designing or modifying the command-line arguments, help,
or output of any application, including CLIs, TUIs, services, and batch jobs.

Language and framework files override this file where they conflict. TUI
layout and keyboard behavior live in `tui.md`.

## Standard Arguments

Every application must accept the arguments below unless a section states
otherwise. Use the same spelling in every application so users and agents
can rely on them.

| Argument | Requirement | Purpose |
| --- | --- | --- |
| `-h`, `-H`, `--help`, `--HELP`, `-?` | Required | Show help and exit. |
| `-V`, `--version` | Required | Print the version and exit. |
| `--about` | Required | Print identity, build, and license metadata as YAML. |
| `-v`, `--verbose` | Required | Increase diagnostic output. |
| `-c`, `--config <path>` | Required when the application reads a config file | Use an explicit configuration file. |
| `-q`, `--quiet` | Recommended | Suppress non-error output. |
| `--log-level <level>` | Recommended | Set log verbosity explicitly. |
| `--no-color` | Recommended | Disable ANSI color. |
| `--format=<type>` | Required with `--about`; recommended for other output | Select output format: `text`, `yaml`, or `json`. |
| `--dry-run` | Required for state-changing commands | Report actions without performing them. |
| `-y`, `--yes` | Recommended for confirmation prompts | Answer prompts affirmatively. |
| `--completion <shell>` | Recommended | Print a shell completion script. |

General rules:

- Accept `--` to end option parsing so values that begin with `-` are safe.
- Accept both `--name value` and `--name=value` for every long option that
  takes a value, and both `-n value` and `-nvalue` for short options. Never
  require one form.
- Write long options with an equals sign (`--config=<path>`) and short
  options with a space (`-c <path>`) in help, manpages, documentation, and
  examples. The equals form is unambiguous when a value is optional or
  begins with `-`.
- Options with an optional value accept only the equals form
  (`--color=auto`); a bare `--color` uses the documented default.
- Give each short option exactly one meaning across all applications.
- Reject unknown arguments with a clear error, a usage hint, and a non-zero
  exit code. Never ignore them silently.
- Parse arguments with the language's standard or established library rather
  than hand-written parsing.
- Do not accept secrets as argument values; command lines are visible in
  process listings and shell history. Read secrets from the environment, a
  file, or standard input as described in `configuration.md`.

## Help

Every application must provide:

- `-h`, `-H`, `--help`, `--HELP`, and `-?` help options.
- A human-readable help display that is also structured enough for an AI agent
  to parse.
- A Linux manpage.
- An HTML help document.

Help display requirements:

- Print help to standard output and exit with code `0` when it is requested.
- Print usage to standard error and exit non-zero when arguments are invalid.
- Use consistent sections: name and summary, usage, arguments and options,
  environment variables, configuration files, exit codes, and examples.
- List every option with its long form, short form, value placeholder,
  default, and the environment variable that sets it, if any.
- Include at least one runnable example for each subcommand.
- Keep the help display, manpage, and HTML help generated from one source, or
  verify in a test that they agree.

## Version

- `-V` and `--version` print one line to standard output, then exit with
  code `0`.
- Format the line as `<name> <semantic-version>`, for example
  `mytool 1.4.2`. Add no banner, color, or extra lines.
- Take the version from a single source of truth, such as the package
  manifest or a build-time injected value. Never duplicate it by hand.
- Do not perform network calls, read configuration, or start services when
  printing the version.
- Subcommands do not need their own `--version`; the top-level option reports
  the application version.

## About

`--about` prints standardized application metadata and build diagnostics.

### Output Formats

- The default output is valid **YAML**, readable in a terminal and parseable
  by tools.
- `--about --format=json` emits valid **JSON** with identical keys, and
  `--about --format=yaml` is the same as the default. Both formats must
  deserialize into the exact same data model.
- Emit keys in the order shown below so output is stable and diff-friendly.

### Schema

| Key | Requirement | Content |
| --- | --- | --- |
| `name` | Required | Application or binary name. |
| `version` | Required | Semantic version, identical to `--version`. |
| `purpose` | Required | One sentence describing what the tool does. |
| `repository` | Required | Full URL of the source repository. |
| `license` | Required | SPDX identifier, or `LicenseRef-<name>` for custom commercial terms. |
| `owner` | Recommended | Maintaining team or person. |
| `support` | Recommended | Issue tracker URL or support contact. |
| `build` | Required | Compile-time diagnostics object, below. |
| `build.target` | Required | Architecture and OS triple, such as `aarch64-apple-darwin`. |
| `build.commit` | Required | Short Git commit hash. |
| `build.dirty` | Recommended | `true` when built from uncommitted changes; omit when clean. |
| `build.timestamp` | Required | ISO 8601 UTC build timestamp. |
| `build.compiler` | Required | Compiler or toolchain and its version. |
| `build.profile` | Recommended | Build profile, such as `release` or `debug`. |
| `notices` | Recommended | Location of third-party license notices and the SBOM, when shipped. |

Example:

```yaml
name: mytool
version: 1.4.2
purpose: Validates and publishes release manifests.
repository: https://github.com/example/mytool
license: Apache-2.0
build:
  target: aarch64-apple-darwin
  commit: 3f9c2ab
  timestamp: 2026-11-23T14:05:00Z
  compiler: rustc 1.91.0
  profile: release
```

### Behavior

- Write directly to standard output and exit immediately with code `0`.
- Handle `--about` before configuration loading and validation, subcommand
  dispatch, and validation of other arguments, so it works even when
  configuration is missing or broken.
- Have no side effects: do not create directories, initialize logs, read
  configuration, start services, or use the network.
- Omit null and empty optional fields in both formats unless a field is
  explicitly required.
- Inject build metadata at build time from a single source. Honor
  `SOURCE_DATE_EPOCH` for `build.timestamp` when the build must be
  reproducible.
- Never include secrets, user data, hostnames, or absolute paths from the
  build machine.

## Verbose

- `-v` and `--verbose` increase diagnostic output. Repeating the flag (`-vv`)
  may raise the level further; the second level is for debugging.
- Write diagnostics to standard error so standard output stays clean for
  results and pipelines.
- Verbose output explains what the application is doing: resolved
  configuration sources, selected files, endpoints contacted, and timing.
- Redact secrets and tokens at every verbosity level. Verbose never means
  unredacted.
- `--verbose` and `--quiet` together are an argument error.
- When `--log-level` is also supported, `--verbose` is shorthand for a more
  detailed level, and an explicit `--log-level` takes precedence.
- Follow `observability.md` for log structure and fields.

## Config

- `-c <path>` and `--config <path>` load the named configuration file.
- An explicitly named file replaces the global, environment-specific, and
  local config file lookup described in `configuration.md`. Environment
  variables and other command-line arguments still override its values.
- Fail with a clear error and a non-zero exit code when the named file does
  not exist, is unreadable, or fails validation. Never fall back to defaults
  silently when a file was requested explicitly.
- Validate the file before starting any work, and report every error with the
  file, key, and reason.
- Support `--config` more than once only when the application documents merge
  order; later files win.
- Consider a `--print-config` option that shows the effective configuration
  with sources and redacted secrets. It is the fastest way to debug
  precedence.
- Document the config file format, default locations, and every key in the
  help, manpage, and HTML help.

## Output and Exit Codes

- Write results to standard output and diagnostics, warnings, and errors to
  standard error.
- Disable color and cursor control automatically when output is not a
  terminal, when `NO_COLOR` is set, or when `--no-color` is given.
- Exit `0` on success, `1` on general failure, and `2` on invalid arguments
  or usage. Document any additional codes in the help and manpage.
- `--format=<type>` takes one of `text`, `yaml`, or `json`, with `text` and
  `yaml` equivalent for `--about`. Reject any other value as an argument
  error that lists the valid types.
- The default format is `text`, except for `--about`, which defaults to
  `yaml`. Applications list the formats each command supports in help;
  requesting an unsupported format for a command is an argument error.
- `yaml` and `json` output must be a stable, documented schema on standard
  output with nothing else mixed in, and both must deserialize to the same
  data model.
- Handle `SIGINT` and `SIGTERM` by cleaning up and exiting promptly, using
  the conventional `128 + signal` exit code.

## Verification

- Test `--help`, `--version`, and `--about` output and exit codes.
- Test that `--about` and `--about --format=json` deserialize to the same data model
  and that `--about` works with a missing or invalid config file.
- Test that an unknown argument and a missing `--config` file exit non-zero
  with an error on standard error.
- Test that `--verbose` output never contains configured secret values.
