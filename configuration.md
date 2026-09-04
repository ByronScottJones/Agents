# Configuration Best Practices

This document outlines the standard best practices for creating, managing, and using configuration files and directories across different operating systems. It is designed to be used as a subfile for agent architectures (`agent.md`).

## 1. Standard Configuration Locations

Applications should respect the conventions of the underlying operating system rather than polluting the user's home directory with dotfiles (e.g., avoid `~/.my_app_config`).

### Linux (XDG Base Directory Specification)
* **User-specific:** `~/.config/<app-name>/` (`$XDG_CONFIG_HOME`)
* **System-wide:** `/etc/<app-name>/` or `/etc/<app-name>.conf`
* **State/Data:** `~/.local/share/<app-name>/` (`$XDG_DATA_HOME`)

### macOS
* **User-specific:** `~/Library/Application Support/<app-name>/`
* **System-wide:** `/Library/Application Support/<app-name>/`
* *Note:* Many CLI/developer tools on macOS also adopt the Linux XDG standard (`~/.config/`). Both are acceptable, but `Application Support` is more native to Apple's HIG.

### Windows
* **User-specific (Roaming):** `%APPDATA%\<app-name>\` (e.g., `C:\Users\User\AppData\Roaming\<app-name>`)
* **User-specific (Local):** `%LOCALAPPDATA%\<app-name>\`
* **System-wide:** `%PROGRAMDATA%\<app-name>\`

## 2. Configuration Formats

Choose the format based on the user's need to read and edit the file:
* **TOML:** Strongly recommended for application configs. Excellent for human readability, strict typing, and native support in modern ecosystems (like Rust).
* **YAML:** Good for complex hierarchies and DevOps tools. Prone to indentation errors and typing ambiguities (e.g., the `NO` boolean issue).
* **JSON:** Avoid for human-edited configs (no comments, strict comma rules). Use for machine-generated state or IPC.
* **INI / .env:** Simple key-value pairs. Great for environment variables but lacks nesting.

## 3. Configuration Precedence

Follow a strict hierarchy for resolving configuration values. Later items override earlier ones:
1. **Command-line flags** (Highest priority)
2. **Environment variables** (e.g., `MYAPP_PORT=8080`)
3. **Local configuration file** (e.g., `.myapprc` in the current working directory)
4. **User-level configuration file** (e.g., `~/.config/myapp/config.toml`)
5. **System-level configuration file** (e.g., `/etc/myapp/config.toml`)
6. **Application defaults** (Lowest priority)

## 4. Environment Variables and Secrets

* **Prefixing:** Namespace environment variables to prevent collisions (e.g., `AGENT_API_KEY`, not `API_KEY`).
* **Secrets:** **Never** store secrets (API keys, tokens, passwords) in plaintext configuration files that are checked into version control.
* **Permissions:** Config files containing sensitive data locally should have strict permissions (e.g., `chmod 600` on Linux/macOS).
* **.env files:** Support loading `.env` files for local development, but ensure `.env` is in `.gitignore`.

## 5. Language-Specific Ecosystems

When implementing configuration logic, utilize ecosystem-standard libraries rather than reinventing OS-path detection.

### Bash
* **Pattern:** Source configuration files safely.
* **Example:** `[ -f "$HOME/.config/myapp/config.env" ] && source "$HOME/.config/myapp/config.env"`
* Use `XDG_CONFIG_HOME` fallback logic if not set.

### Rust
* **Paths:** Use the [`directories`](https://crates.io/crates/directories) crate to reliably determine OS-specific paths (`ProjectDirs::from("com", "Org", "MyApp")`).
* **Parsing:** Use [`serde`](https://crates.io/crates/serde) with `toml` or `serde_json`.
* **Management:** Use the [`figment`](https://crates.io/crates/figment) or [`config`](https://crates.io/crates/config) crate to handle merging environment variables, files, and defaults.

### GoLang
* **Paths:** Use the standard library `os.UserConfigDir()` which correctly maps to `~/.config`, `AppData/Roaming`, and `Library/Application Support`.
* **Management:** [`Viper`](https://github.com/spf13/viper) is the industry standard for Go apps. It handles multi-format, OS paths, and live-reloading natively.

### Node.js
* **Paths:** Use [`env-paths`](https://www.npmjs.com/package/env-paths) to determine the correct OS directories.
* **Parsing:** Utilize [`cosmiconfig`](https://www.npmjs.com/package/cosmiconfig) for finding and loading config files (supports `.rc`, `package.json` keys, YAML, JSON).
* **Env:** Standardize on [`dotenv`](https://www.npmjs.com/package/dotenv) for `.env` files.

### .NET (C#)
* **Paths:** Use `Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)` for Roaming/Application Support paths.
* **Management:** Rely on `Microsoft.Extensions.Configuration`. It natively supports JSON/XML/INI, Environment Variables, Command-line args, and merging configurations in a deterministic order. Use `appsettings.json` as the standard local convention.
* **Secrets:** Use `dotnet user-secrets` for local dev environment secrets.

## 6. Best Practices Summary
* **Fail Fast:** If a configuration file is malformed or contains invalid data, panic/exit immediately with a clear error message pointing to the exact line/key.
* **Schema Validation:** Validate configuration structures on load using schemas (JSON Schema, Zod for Node, Serde for Rust) to ensure type safety.
* **Watch for Changes:** For daemonized agents, consider file watchers to auto-reload configuration without restarting (SIGHUP on Linux).
* **Provide Templates:** Always provide a command to generate a default config file (e.g., `agent init` or `agent config --generate`). If no configuration file exists, notify the user, and offer to create it.


---

## .NET Configuration Best Practices

This document outlines standard best practices for configuring .NET (C#) applications, ranging from console agents to web services.

### 1. Standard Configuration Ecosystem

.NET has a powerful, unified configuration framework built into `Microsoft.Extensions.Configuration`. Avoid custom file parsers; rely on the built-in providers.

* **Primary Format:** JSON (`appsettings.json`) is the industry standard for .NET. 
* **Environment-Specific:** Use `appsettings.{Environment}.json` (e.g., `appsettings.Development.json`) to override base settings.
* **Binding:** Always bind configuration sections to strongly-typed C# classes using the **Options Pattern** (`IOptions<T>`, `IOptionsSnapshot<T>`, `IOptionsMonitor<T>`).

### 2. Configuration Precedence

The default `Host.CreateDefaultBuilder()` establishes a strict override hierarchy (later sources override earlier ones):
1. `appsettings.json`
2. `appsettings.{Environment}.json`
3. User Secrets (only in the `Development` environment)
4. Environment Variables
5. Command-line arguments

### 3. Environment Variables

* **Hierarchy Mapping:** .NET maps environment variable hierarchies using a double underscore `__` (cross-platform safe) or a colon `:` (Windows).
  * Example: `Logging__LogLevel__Default=Debug` maps to the `{"Logging": {"LogLevel": {"Default": "Debug"}}}` JSON structure.
* **Prefixing:** When using `AddEnvironmentVariables("PREFIX_")`, you can namespace your agent's variables to avoid system-wide collisions.

### 4. Secrets Management

* **Local Development:** Never put connection strings or API keys in `appsettings.json`. Use the **Secret Manager** tool (`dotnet user-secrets init` -> `dotnet user-secrets set "ApiKey" "12345"`).
* **Production:** Do not use User Secrets in production. Rely on Environment Variables, Azure Key Vault, AWS Secrets Manager, or HashiCorp Vault.

### 5. Cross-Platform File Paths

If your agent needs to store local SQLite databases or user-specific state files, avoid hardcoding `C:\` or `~/.config/`.
* **User Data:** `Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)` (Roaming) or `LocalApplicationData` (Local).
* **System Data:** `Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)`.
* **Path Construction:** Always use `Path.Combine()` to handle OS-specific directory separators safely.



## PowerShell Configuration Best Practices

This document outlines standard best practices for configuring PowerShell scripts, modules, and automation agents.

### 1. Preferred Configuration Formats

Avoid arbitrary text parsing or custom INI files. PowerShell natively handles structured data exceptionally well.

* **JSON (`.json`):** The recommended standard for cross-platform PowerShell (Core) configuration. Use `Get-Content config.json | ConvertFrom-Json`. Easily human-readable and integrates well with other languages.
* **PSD1 (PowerShell Data File):** Native hash table format. Excellent for module manifests or configurations that only PowerShell will read. Use `Import-LocalizedData` or safely parse with `Invoke-Expression` (only if the source is fully trusted).
* **CliXml:** Good for serializing rich .NET objects, but poor for human editing. Use primarily for state preservation, not user configuration.

### 2. Configuration Locations

* **Module-Specific:** Store default configurations alongside the module using `$PSScriptRoot`. Example: `$configPath = Join-Path $PSScriptRoot "config.json"`.
* **User-Specific:** For user overrides across sessions, use the OS-native application data paths via `$env:APPDATA\MyApp\` (Windows) or `$env:HOME/.config/MyApp/` (Linux/macOS).
* **Profile Config:** Avoid cluttering the `$PROFILE` file with heavy configuration. Instead, load lightweight bootstrapper settings from the profile.

### 3. Parameters vs. Configuration

* **Dynamic overrides:** Use `[CmdletBinding()]` and `param()` blocks for runtime overrides. Configuration files should provide the *defaults*, while parameters provide the *execution context*.
* **Validation:** Use parameter validation attributes (e.g., `[ValidateSet()]`, `[ValidateNotNullOrEmpty()]`) to enforce configuration correctness at runtime.

### 4. Secrets and Credentials

* **Never Hardcode:** Do not store plaintext passwords or API keys in `.ps1`, `.psm1`, or `.json` files.
* **SecretManagement Module:** Use the official `Microsoft.PowerShell.SecretManagement` module. It acts as an abstraction layer over local vaults (like `SecretStore`, KeePass, or Windows Credential Manager) and cloud vaults (Azure Key Vault).
* **Legacy Local Scripts:** For basic local automation, you can securely store credentials for the *current user/machine* using `Get-Credential | Export-Clixml -Path .\mycreds.xml`.

### 5. Environment Variables

* Access configuration dynamically via the `$env:` provider (e.g., `$env:AGENT_API_URL`).
* Set transient environment variables in the session or persistent ones using `[Environment]::SetEnvironmentVariable()`.



## Rust/Tauri Stack Configuration Best Practices

This document outlines best practices for configuring applications built on the Tauri framework, which bridges a Rust backend with a web frontend (e.g., Vite, React, Vue).

### 1. Core Framework Configuration (`tauri.conf.json`)

Tauri relies on `tauri.conf.json` (or `tauri.conf.json5`, `Tauri.toml`) at the root of the `src-tauri` directory.
* **Environment Agnostic:** This file configures the build process, window initialization, bundle settings, and security policies. 
* **Security Allowlist:** Be aggressively minimal. Only enable the specific Tauri APIs (e.g., `fs`, `shell`, `http`) your app actually uses to reduce the attack surface.
* **Dynamic Config:** You can manipulate this file dynamically during CI/CD pipelines, or use the `tauri.conf.json` `build.beforeBuildCommand` to inject environment-specific metadata.

### 2. Frontend Configuration

* **Environment Variables:** Use your frontend bundler's native `.env` system (e.g., Vite uses `.env`, `.env.development`, `.env.production`). 
* **Prefixing:** Only variables prefixed with your bundler's public prefix (e.g., `VITE_` for Vite) will be exposed to the client-side JavaScript. Keep secrets out of the frontend entirely.
* **IPC (Inter-Process Communication):** Pass configuration from the Rust backend to the frontend asynchronously at runtime using Tauri commands (`#[tauri::command]`) rather than hardcoding backend URLs in the frontend env files.

### 3. Backend (Rust) Configuration

* **Configuration Management:** Use standard Rust crates like [`config`](https://crates.io/crates/config) or [`figment`](https://crates.io/crates/figment) to merge TOML/JSON files, environment variables, and defaults for the Rust core.
* **Deserialization:** Always use `serde` to strongly type your configuration structures.
* **State Management:** Use Tauri's managed state (`tauri::State<T>`) to hold your application configuration in memory and make it accessible to all your commands.

### 4. User-Specific App Settings (State)

When users change settings in the UI (e.g., theme, custom hotkeys, local API targets), this must persist across sessions safely.
* **Tauri Plugin Store:** Use the official [`tauri-plugin-store`](https://github.com/tauri-apps/plugins-workspace/tree/v1/plugins/store) to persist key-value data. 
* **Locations:** The plugin automatically resolves to the correct OS-specific data directory (e.g., `AppData/Roaming` on Windows, `Library/Application Support` on macOS) using Tauri's `PathResolver`.
* **Avoid `fs` directly:** While you *can* use the Tauri `fs` API to write custom JSON files, `tauri-plugin-store` handles atomic writes, caching, and frontend reactivity gracefully.

### 5. Security & Scoped Access

* **File System Scopes:** If your configuration allows the user to specify custom working directories, you must dynamically update Tauri's `fs` scope (`app.fs_scope().allow_directory()`) at runtime. Tauri will block access to paths not explicitly allowed in `tauri.conf.json` or added dynamically to the scope.
