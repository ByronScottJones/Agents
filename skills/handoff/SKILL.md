---
name: handoff
description: Finish a branch's documentation. Reads git state, runs the verification commands, removes finished items from HANDOFF.md, writes the CHANGELOG.md entry from the template, updates ENGINEERING.md where behavior changed, stamps the timestamps, checks the documents, and prints a one-line summary. Use at the end of a branch, or when the owner asks for the handoff, the changelog entry, or a session summary.
---

# Handoff

Use this skill in a project that keeps `HANDOFF.md`, `CHANGELOG.md`, and
`ENGINEERING.md` in its root and has `scripts/docs-stamp.sh`,
`docs/changelog-entry-template.md`, and the `make docs/check` target. If any of
these are missing, the project has not adopted the tooling: say so, and offer
to copy the bundled `scripts/docs-stamp.sh` and
`templates/changelog-entry-template.md` from this skill's folder instead of
improvising.

The skill replaces the large hand-written edit across `ENGINEERING.md`,
`HANDOFF.md`, `CHANGELOG.md`, and usually `README.md` and a guide. The agent
supplies only the facts; the scripts and the template supply the rest.

## Rules

- Write only inside the project. Do not commit or push unless the owner asks.
- Record only facts known now. Never write commit ids, pull request numbers,
  review state, or merge state into `HANDOFF.md` or `CHANGELOG.md`.
  `make changelog/status` reads them from git later. `tests/docs.rs` rejects, in
  those two files: any `#<digits>`, `PR <digits>`, or `pull request <digits>`; a
  word that looks like a commit id (7 to 40 hex characters holding a digit and a
  letter); and the phrases "awaiting review", "pending merge", "in flight: pr",
  and "merge commit". It does not catch every wording of review or merge state,
  so keep such state out by hand, even inside a warning.
- Report verification honestly. A command that was not run, or could not run,
  goes under "Not exercised"; it is never recorded as passing.
- Remove a finished item from `HANDOFF.md` in the same branch that finishes it,
  and say so in the changelog entry (`HANDOFF.md` H-N removed).
- Timestamps are ISO 8601 UTC from `scripts/docs-stamp.sh`, never typed by hand.
- One changelog entry per branch. When an entry for the branch already exists,
  correct it; do not add a second one.

## Steps

1. **Read the state.** Start from a clean tree: `git status -sb` should show no
   changes, apart from the documents when an earlier handoff pass is being
   corrected. Run `git fetch origin`, `git branch --show-current`, and
   `git log --oneline origin/main..HEAD`. The base recorded in `- Branch:` is
   `main`, compared against `origin/main`, not a stale local `main`. Read
   `HANDOFF.md`, the newest entry of `CHANGELOG.md`, and the real diff: run
   `git diff --stat origin/main`, then read the diff of every new script, test,
   and document, because the stat does not show what they do. Stop and ask when
   the branch is `main`, when it has no commits ahead of `origin/main` (nothing
   to hand off, or it has already merged), or when the tree holds changes
   unrelated to the work.
2. **Verify.** Run the commands in the "Run first" block of `HANDOFF.md`
   section 2 (for this project: `cargo fmt --all -- --check`,
   `cargo clippy --workspace --all-targets --all-features -- -D warnings`,
   `cargo test --workspace --all-targets --all-features`,
   `cargo test --workspace --doc --all-features`, and `cargo deny check`), and
   record the result of each. Notes:
   - Read the exit status of each command and its `test result` lines; do not
     trust a pipe's status. In the agent sandbox, Rust builds print `xcrun`
     cache errors that are replayed build-script warnings, not failures.
   - `cargo test` can take longer than a default 120 second tool timeout; raise
     the timeout rather than treating a timeout as a failure or a pass.
   - `cargo deny check` exits 0 with the accepted advisories (`ENGINEERING.md`
     DEC-6); record it as "passes, with two accepted advisories".
   - A failure stops the handoff until it is fixed or recorded as a known issue
     with the owner's agreement.
3. **Update `ENGINEERING.md`** where behavior, requirements, interfaces, the
   file table, or the verification plan changed. Where an item named in the spec
   is finished, state the behavior as met and drop the item ID. Find stale IDs
   with `grep -on 'H-[0-9]*' ENGINEERING.md` and compare them with the table in
   `HANDOFF.md`; `tests/docs.rs` fails on the same mismatch.
4. **Update `HANDOFF.md`.**
   - Remove finished items from the outstanding table, along with their scope
     subsection and any decision that is now settled.
   - Add new items, risks, and unverified areas found during the work.
   - Set "In flight" to the branch and what is complete, without review or merge
     state.
   - Update the bullets under "Results last verified" with the results from
     step 2, keeping the standing bullets (the Python and Windows note, the
     `cargo deny` advisories, the `make docs/check` line, the CI note). Do this
     before stamping: step 7 rewrites only the heading line, not the bullets.
5. **Write or correct the changelog entry.** For a new entry, copy
   `docs/changelog-entry-template.md` to the top of `CHANGELOG.md`, under the
   first `---`, and fill every field. When the branch already has an entry (an
   earlier pass, or hand-written updates), compare it with the real diff from
   step 1 and fix only what is stale or missing, including the `Entry written`
   stamp: take it again from `scripts/docs-stamp.sh` whenever the entry changes.
   Name the `HANDOFF.md` items removed as `H-N removed`. Under "Not exercised"
   state plainly what could not be checked, and consider the usual candidates:
   the feature against real hardware, CI on GitHub, a second operating system,
   Windows, and any installer or script run only against a temporary directory.
6. **Update the user-facing text** (`README.md`, `docs/end-user-guide.md`,
   `docs/developer-guide.md`) only for behavior the user or developer sees.
7. **Stamp and check.** After steps 2 and 4, run `make docs/stamp ARGS=--apply`
   to write the three timestamps, then `make docs/check` (Markdown lint,
   `tests/docs.rs`, `tests/spec.rs`). Confirm the linter's `Linting: N files`
   line shows N above 0; "0 issues in 0 files" means nothing was checked, and
   the lint configuration (`.markdownlint-cli2.jsonc` `globs`) is wrong. Fix
   what the check reports.
8. **Print the summary.** One line the owner can read: the branch, what changed,
   the verification result, and anything not exercised. Then list the decisions
   the owner has to make, and any place where the steps above were unclear,
   wrong, or skipped, so the skill can be corrected.

## What the tests catch

`tests/docs.rs` fails when a document holds a commit id or pull request number,
when a stamp is missing, when `HANDOFF.md` still lists an item the changelog
says was removed, when `ENGINEERING.md` names an item the handoff no longer
lists, or when the changelog template loses a field. `tests/spec.rs` fails when
the specification disagrees with the code. Neither notices a lint run that
checked no files; step 7 does.
