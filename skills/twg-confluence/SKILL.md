---
name: twg-confluence
description: >
  Use with root `twg` for Confluence content, spaces, hierarchy, authoring,
  editing, comments, versions, permissions, exports, and CQL. Applies
  Confluence semantics and safe write rules.
---

# twg-confluence

Use with root `twg` for Confluence-focused work, not a supporting link in a
broader workflow. Live help owns command grammar.

## CLI launcher fallback

Run `twg <command>`. On shell `command not found`, use `$HOME/.local/bin/twg`
(macOS/Linux) / `$env:LOCALAPPDATA\Programs\twg\bin\twg.exe` (PowerShell), then
tell user to add that directory to PATH. Do not treat auth or command errors as
PATH failures.

## First Route

| Intent                           | Route                                                                                      |
| -------------------------------- | ------------------------------------------------------------------------------------------ |
| Known content ID/URL             | `confluence content get`; [route embeds](references/content.md#embed-and-smart-link-reads) |
| Several known pages              | Read each with `confluence content get`                                                    |
| Exact Confluence filtering       | Confluence search with CQL                                                                 |
| Date-filtered page/blogpost list | `confluence search query --cql` with `lastmodified`; `content list` has no date filter     |
| Fuzzy page/topic discovery       | Cross-product search, then native get                                                      |
| Create or update content         | Unified `confluence content` surface                                                       |
| Space metadata/lifecycle         | `confluence space`                                                                         |
| Hierarchy                        | `confluence tree`                                                                          |
| Export                           | Word returns a download directly; PDF requires export-status polling                       |

Use `twg help describe "<exact path>"` before an unfamiliar or consequential
mutation.

## Confluence Semantics

[Pagination](references/pagination.md).

- Search snippets are discovery candidates. Read the selected content before
  summarizing or editing it.
- `confluence content get` and `content versions list --id` are single-page.
  Use CQL for list metadata. For reading or summarizing known pages, fetch each
  once with `confluence content get <id-or-url> --detail full --format md -o json`.
  Read `data.body` for text and conversion warnings; use `output_files.stdout`
  when compact output omits the text. Reuse it for summaries and saved files.
- Markdown can drop status markers, macros, and table details. If
  `data.body.lossyConversion` is true, read that page once in `--format html`
  before drawing conclusions from missing content. Use HTML first for editing or known rich
  content. Never infer a missing status from surrounding prose.
- Do not use `docs get` to read page bodies. It only looks within a user's
  document activity window and may miss pages the user can access.
- Page titles are supplied separately from bodies. Do not repeat the title as
  the first body heading.
- Remix create returns an asset, not a page embed. Embed via the loaded HTML
  guide and read back the page; an ID/URL is not proof.
- Maui create uses `--content-id` only for ownership/embedding, not page
  reading. Read source first; include values, labels, narrative, and
  visualization instructions in the prompt. "This page" is not enough.

## Safe Authoring And Editing

- Treat user authorization and CLI confirmation as separate checks for archive,
  trash, and purge. Passing `--yes` is an execution preflight, not evidence the
  user authorized the mutation. Use `--yes` only after the user named the
  mutation and the exact target or a previously displayed bounded target set.
- Explicit instructions such as "archive page XYZ" or "archive those 15 pages"
  after a concrete list authorize execution without another confirmation. Vague
  language such as "clean up", "organize", or "take care of" does not authorize
  archive, trash, or purge.
- If the mutation or targets were inferred, resolve the proposed targets,
  present the exact action and bounded target list, and wait for the user to
  affirm it. Do not generalize approval from a similar prior action; `trash`
  does not authorize a permanent purge.
- Before authoring, apply the target space's instructions once per space per
  session; see `references/spaces.md`. Empty instructions mean defaults.
- Prefer `live_doc` for collaborative internal content where supported. Bare
  "page" or "doc" creation also defaults to live docs.
- Use `page` for explicit classic/non-live intent, knowledge bases, customer
  help, established classic-page spaces, or page-only operations. Preserve an
  existing target's type; a classic parent does not set a child's type.
- For non-trivial edits, read current content, save the body locally, edit the
  file, then update with the snapshot token.
- Use the lossless HTML round trip when macros or exact storage matter.
- Use `--dry-run` only for explicit preview requests or unusually risky edits
  where direct execution was not requested.
- Read back the content or space after mutation and report its URL.

## Handoffs

- Load `twg-context-discovery` for related Jira work, projects, goals, or
  dependencies; `twg-responsibility-routing` for people, ownership, authority,
  or escalation.
- Load `twg-status-rollups` when pages contribute to a broader status report.
- Load `twg-operational-health` for runbooks, incidents, or PIRs.
- Load `twg-space-creation` to create or clone a whole space.

## References

- `references/content.md` - content types, reads, writes, and exports
- `references/editing.md` - concurrency-safe body editing
- `references/spaces.md` - space lifecycle and hierarchy
- `references/querying.md` - CQL and fuzzy discovery
- `references/body-formats.md` - HTML, markdown, mentions, and special formats
