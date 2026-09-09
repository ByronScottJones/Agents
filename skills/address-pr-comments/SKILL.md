---
name: address-pr-comments
user-invocable: false
description: "Use when implementing, responding to, or resolving GitHub pull request review feedback. Retrieves comments with get-pr-comments, applies requested changes, validates them, and optionally resolves addressed threads. Mutating actions require explicit confirmation."
---

# Address Pull Request Comments

Turn pull request review feedback into validated changes. This skill owns the
write-side workflow; `get-pr-comments` owns read-only retrieval.

## Parameters

- `owner`, `repository`, `pullRequest`: target PR; default to the active PR.
- `threadIds`: specific threads; default to all unresolved threads.
- `mode`: `implement`, `respond`, `resolve`, or `merge-ready-check`; default `merge-ready-check`.
- `autoApply`: allow applying inline suggestions; default `false`.
- `requireChecks`: require passing checks; default `true`.
- `format`: `summary` or `json`; default `summary`.

## Workflow

1. Retrieve unresolved feedback with `get-pr-comments`.
2. Group comments by file, thread, and requested behavior.
3. Read the affected code before editing; preserve unrelated changes.
4. For `implement`, make the smallest change that addresses the feedback.
5. Run the narrowest relevant validation after each substantive edit.
6. Re-read the resulting diff and map every requested thread to a change,
   response, or explicit decision not to change it.
7. Resolve threads or post replies only when the caller explicitly requests it.
8. For `merge-ready-check`, verify checks, approvals, conflicts, unresolved
   threads, and branch status without merging automatically.

## Safety

- Never auto-merge.
- Never resolve a thread that was not addressed or intentionally declined.
- Do not apply reviewer suggestions blindly; inspect their context first.
- Preserve secrets and do not paste sensitive build output into PR comments.

## Output

Return the PR identity, addressed threads, skipped threads with reasons, files
changed, validation commands and results, remaining blockers, and next steps.
