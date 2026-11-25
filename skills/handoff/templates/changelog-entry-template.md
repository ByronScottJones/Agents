# Changelog entry template

Copy the entry below to the top of `CHANGELOG.md`, just under the first `---`
rule, and replace every `<placeholder>`. Remove `Outcome` for work that merges
and `Notes` when there are none. Keep the fields in this order; `tests/docs.rs`
checks that this template has them all.

Take the timestamp for `Entry written` from `scripts/docs-stamp.sh`. Do not
write commit ids, pull request numbers, review state, or merge state; they are
not known yet and `make changelog/status` reads them from git later.

```markdown
## <branch-name>: <one-line summary>

- Branch: `<branch-name>`, base `<base>`
- Entry written: <ISO 8601 UTC timestamp>
- Outcome: <abandoned or superseded, and why>
- Worked on by: <person> (<role>); <AI agent and model>, via <tool> (<role>)
- What it did:
  - <behavior or design change>
- Spec and docs: <ENGINEERING.md sections and requirement IDs changed; guides;
  `HANDOFF.md` items removed or added>
- Verification: <each command run and its result; tests that failed before the
  fix and pass after>
- Not exercised: <what was not run or not possible here, stated plainly>
- Notes: <surprises, reverts, incidents, and items moved to `HANDOFF.md`>

---
```
