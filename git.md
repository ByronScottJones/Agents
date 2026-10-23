---
title: "Git Workflow"
description: "Load this file for repository, branch, commit, or pull request work."
---

# Git Workflow

Load this file for repository, branch, commit, or pull request work.

## Repository Setup

- Check `git rev-parse --git-dir` before the first edit.
- If no repository exists, ask whether to initialize one. Do not initialize a
  repository during a read-only task.
- Account check: run `gh auth status` before creating a repo, and ask the user
  to confirm the account is the one to create the project in.
- Repo details: ask the user whether the project is work or personal, and never
  assume. Work goes to `byronjones-elsevier` and personal goes to
  `byronscottjones`.
- New empty project: run `git init -b main`, add a `.gitignore`, and make an
  initial commit on `main` with only that file. Then work on a feature branch.
- Project with existing files: run `git init -b main`, add a `.gitignore` and a
  `README.md`, and commit the existing files as the initial commit on `main`.
  Then work on a feature branch.
- GitHub repository: create it private, always. Set the description and
  relevant topics with `gh` when those fields are missing. Set a license; MIT
  is the default unless the user says otherwise. Include `MCP` as a topic when
  creating an MCP service.
- Keep standard project documentation current, including `README.md` and
  `AGENTS.md` when they exist.

## Changes and Branches

- Create a branch for each feature or requested change.
- Never discard changes you did not make. Work with unrelated changes in place.
- Do not use destructive commands such as `git reset --hard` or `git checkout --`
  unless explicitly requested.
- If a task is explicitly read-only, research-only, or says not to commit, do not
  stage or commit changes.

## Commits and Pull Requests

- Every completed change that compiles, and passes lint and tests where they
  exist, gets its own commit. Do not leave requested edits uncommitted.
- Use a short, descriptive message with a conventional prefix such as
  `refactor:` or `docs:` when appropriate.
- End each commit message with the attribution line supplied by the tool or
  session instructions, when one is provided.
- Open one pull request per major feature, such as a milestone in the Plan, when
  that feature is complete. Small fixes and documentation updates are commits
  grouped under the larger pull request.
- When working on projects where there will be multiple pull requests, and those
  pull requests could cause merge conflicts, pause and allow the user to review,
  approve, and merge those pull requests before proceeding.
