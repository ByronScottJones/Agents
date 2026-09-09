# Git Workflow

Load this file for repository, branch, commit, or pull request work.

## Repository Setup

- Check `git rev-parse --git-dir` before the first edit.
- If no repository exists, ask whether to initialize one. If approved, add a suitable `.gitignore` and commit the existing files.
- Keep standard project documentation current, including `README.md` and `AGENTS.md` when they exist.

## Changes and Branches

- Create a branch for each feature or requested change.
- Never discard changes you did not make. Work with unrelated changes in place.
- Do not use destructive commands such as `git reset --hard` or `git checkout --` unless explicitly requested.
- If a task is explicitly read-only, research-only, or says not to commit, do not stage or commit changes.

## Commits and Pull Requests

- Commit each completed change set with a short, descriptive message; use conventional prefixes such as `refactor:` or `docs:` when appropriate.
- Do not leave requested edits uncommitted.
- Create a pull request only when requested or when the repository workflow explicitly requires it.
- When working on projects where there will be multiple pull requests, and those pull requests could cause merge conflicts, pause and allow the user to review, approve, and merge those pull requests before proceeding.
- For a GitHub repository task, update its description and relevant topics with `gh` when those fields are missing.
- Include `MCP` as a repository topic when creating an MCP service.
