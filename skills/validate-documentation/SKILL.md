---
name: validate-documentation
user-invocable: false
description: "Use when auditing repository documentation for Markdown lint, broken links, stale paths, missing topic routing, invalid code fences, and inaccurate commands. Produces a report by default and only edits documentation with explicit confirmation."
---

# Validate Documentation

Check documentation integrity across the repository and its agent guidance.

## Parameters

- `rootPath`: repository root; default workspace root.
- `scope`: Markdown, agent guidance, README, or all; default all.
- `checkLinks`: default `true`.
- `checkCommands`: default `true`.
- `checkRouting`: default `true`.
- `fix`: default `false`.
- `format`: `summary`, `json`, or `sarif`; default `summary`.

## Workflow

1. Discover Markdown and agent customization files.
2. Run configured Markdown lint and detect frontmatter errors.
3. Verify local links, referenced files, anchors, code fences, commands, and paths.
4. Check reusable topic files are routed from `AGENTS.md`.
5. Identify duplicated ownership, contradictions, stale filenames, and unclear scope.
6. In fix mode, propose a diff first and apply only confirmed mechanical fixes.

## Safety

- Do not rewrite documentation wholesale.
- Do not weaken lint configuration to hide content errors.
- Do not execute commands copied from documentation without confirmation.

## Output

Return files checked, lint results, broken references, routing gaps, stale or
contradictory guidance, fixes proposed/applied, and remaining issues.
