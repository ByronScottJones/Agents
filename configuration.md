---
title: "Configuration"
description: "Load this file when designing or modifying application configuration."
---

# Configuration

Load this file when designing or modifying application configuration. Language
and framework files contain their own ecosystem-specific configuration rules.

## Locations

Use the operating system's application-data conventions instead of adding
unrelated dotfiles to the user's home directory.

| Platform | User-specific | System-wide |
| --- | --- | --- |
| Linux | `~/.config/<app-name>/` | `/etc/<app-name>/` |
| macOS | `~/Library/Application Support/<app-name>/` | `/Library/Application Support/<app-name>/` |
| Windows | `%APPDATA%\<app-name>\` or `%LOCALAPPDATA%\<app-name>\` | `%PROGRAMDATA%\<app-name>\` |

`XDG_CONFIG_HOME`, `XDG_DATA_HOME`, and equivalent platform variables may
override the default user directories. Some command-line tools on macOS
intentionally use XDG paths.

## Formats

- Use TOML for human-edited application configuration when the ecosystem
  supports it.
- Use YAML for hierarchical configuration where the consuming tool requires it.
- Use JSON for machine-generated state or interoperability, not for config that
  users must edit frequently.
- Use INI or `.env` files only for simple key-value data.

## Precedence

All applications and services, including those following `twelvefactors.md`,
use composable configuration. Load and apply sources in the order listed below,
from lowest priority to highest priority. When two sources set the same key,
the last source applied wins. A source that does not set a key leaves the value
from the previously applied source unchanged.

1. Application defaults (lowest priority)
2. Global config file ("<appname>_config.toml")
3. Environment-specific config file ("<appname>_config_dev.toml", "<appname>_config_prod.toml")
4. Local config file in the application directory (development override, not
   committed)
5. Environment variables
6. Runtime command-line arguments (highest priority)

Where the files live follows the Locations table above. Frameworks and
deployment platforms may define a different order; document that order in the
owning topic file, including whether it replaces or extends this order.

## Secrets

- Never commit API keys, tokens, passwords, or connection strings.
- Use a secret manager or an environment variable for runtime secrets.
- Treat variables exposed to browser or client bundles as public configuration,
  never as secrets.
- Namespace environment variables to avoid collisions, such as "<APPNAME>_API_KEY"
  instead of "API_KEY".
- Keep local `.env` files in `.gitignore`; commit a `.env.example` when useful.
- Restrict permissions on local files that contain sensitive values.

## Validation and Failure

- Validate configuration against a schema or strongly typed model at startup.
- Fail fast with an actionable error that identifies the invalid key or value.
- Never include secret values in validation errors or logs.
- Provide a documented way to generate a default configuration file.
- Add file watching only when live reload is a deliberate requirement.
- Treat schema changes as migrations: preserve compatibility or document the
  required upgrade path.
