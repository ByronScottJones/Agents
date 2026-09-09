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
