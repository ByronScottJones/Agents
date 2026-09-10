# Agents

 This repository stores my `$HOME/.agents` folder for AI agent use.

 The agent files should be kept in the repository's `.agents/` directory.

## Clone into `$HOME/.agents`

```bash
# Optional: back up any existing ~/.agents directory
mv "$HOME/.agents" "$HOME/.agents.backup.$(date +%Y%m%d%H%M%S)"

# Clone this repository directly into ~/.agents
git clone -b main https://github.com/ByronScottJones/Agents.git "$HOME/.agents"
```

If you already cloned the repo elsewhere, you can link it instead:

```bash
ln -s /path/to/Agents "$HOME/.agents"
```

## Link into AI coding tools

Many tools work best when they can see the repo contents through a stable path such as `$HOME/.agents` or a symlink to it.

### Recommended pattern

Use a tool-specific parent directory for that tool's own files, then link the shared agent guidance into a common subdirectory such as `agents`.

Examples:

```bash
mkdir -p "$HOME/.claude"
ln -s /path/to/Agents "$HOME/.claude/agents"

mkdir -p "$HOME/.codex"
ln -s /path/to/Agents "$HOME/.codex/agents"

mkdir -p "$HOME/.gemini"
ln -s /path/to/Agents "$HOME/.gemini/agents"
```

This lets each tool keep its own settings, cache, and history alongside a shared `agents` directory.

### Claude Code

- Keep Claude Code's own configuration in `.claude/`.
- Link the shared guidance into `.claude/agents`.
- If Claude Code supports a configurable instructions path, point it at the repo root or the linked `agents` directory.

### Codex

- Keep Codex-specific files in its own parent directory.
- Link the shared guidance into `.../agents` rather than replacing the whole directory.
- Start Codex from the repository root when possible so `AGENTS.md` is discovered automatically.

### Gemini CLI

- Keep Gemini CLI state in its own directory.
- Link the shared guidance into `.../agents` as a subdirectory.
- Prefer launching Gemini CLI from the repo root or a workspace that can reach the linked path.

### Other agent tools

- Prefer a direct clone or symlink for the shared guidance, not a full copy.
- Use a tool-specific parent directory and link only the shared agent guidance into a common `agents` subdirectory.
- Make sure the tool can reach the repository root and any nested topic files.
- If the tool supports a dedicated instructions path, point it to the linked `agents/AGENTS.md` or equivalent entry file.

## Guidance Layout

- `AGENTS.md` is always active and routes work to topic files.
- Load only the topic files that match the task; load multiple files when a task
  spans languages or frameworks.
- `CUA_TUI_Specification.md` applies only when CUA compliance is explicitly
  requested.

## Validation

Run the Markdown lint check before committing documentation changes:

```bash
npx --yes markdownlint-cli2 '**/*.md' '#.git/**'
```

## Skills

Skills are on-demand workflows under `skills/<name>/SKILL.md`. Load a skill
when its trigger matches the task; topic files remain the source of policy and
the skill provides the execution workflow.

| Skill | Purpose |
| --- | --- |
| `get-pr-comments` | Retrieve pull request review feedback. |
| `address-pr-comments` | Implement and validate pull request feedback. |
| `remediate-dependency-vulnerabilities` | Scan and remediate vulnerable dependencies. |
| `validate-and-deploy-iac` | Validate, plan, and safely apply infrastructure as code. |
| `validate-github-actions` | Audit GitHub Actions syntax, permissions, and pinning. |
| `build-and-scan-container-image` | Build, scan, and generate SBOMs for images. |
| `validate-and-deploy-kubernetes` | Validate and safely deploy Kubernetes manifests. |
| `execute-database-migration` | Plan, test, and execute schema migrations. |
| `verify-service-health` | Verify deployment readiness and service health. |
| `generate-service-documentation` | Create service records and operational documentation. |
| `prepare-release` | Prepare release notes and promotion readiness. |
| `incident-triage` | Triage incidents using bounded operational evidence. |
| `validate-documentation` | Check Markdown, links, routing, and stale references. |
| `review-configuration` | Audit configuration safety and precedence. |
| `generate-adr` | Draft architecture decision records. |
