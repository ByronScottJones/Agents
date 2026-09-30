---
name: twg-context-discovery
description: >
  Use with root `twg` for deep context, dependency maps, related entities,
  project-to-repo discovery, OOO catch-ups, and
  "catch me up" requests around a concrete anchor.
---

# twg-context-discovery

Use the root `twg` skill. Get command grammar from live `twg help`,
`twg help <terms>`, or `twg help describe <path>`.

## CLI launcher fallback

Run `twg <command>`. On shell `command not found`, use `$HOME/.local/bin/twg`
(macOS/Linux) / `$env:LOCALAPPDATA\Programs\twg\bin\twg.exe` (PowerShell), then
tell user to add that directory to PATH. Do not treat auth or command errors as
PATH failures.

## First Move

Resolve the anchor before widening:

- Stable key, URL, or ARI: use directly when the family is clear.
- Named project/service: retain qualifiers; resolve/hydrate and verify
  identity before synthesis. Use an alias/successor only with source evidence;
  report missing named evidence as a scoped gap. Broad or ambiguous topics may
  explore plausible scopes.
- Fuzzy topic/name: classify scope; compare plausible anchors before ranking,
  hydrating only those whose evidence can resolve the scope. Keep the set small,
  but do not use a fixed anchor count when a material ambiguity remains.
- Multiple same-kind anchors: batch them in one context call when supported.
- Unknown command shape: inspect focused help before calling data.

For fuzzy topics, group candidates by charter, roadmap, project, product, or
service scope; keep same-named clusters separate. Compare scope, centrality,
breadth, and recency before selecting. If unclear, show alternatives or ask
before inferring ownership. Parent/program, component/subproject, sibling, and
successor candidates require charters or relationships; reopen comparison when
evidence contradicts a provisional anchor. Exact keys and URLs keep the fast path.

If context is not advertised for an anchor type, use product-native hydration
and search evidence instead of inventing paths.

For ownership, expertise, approval authority, leadership reach-outs, or
escalation, load `../twg-responsibility-routing/SKILL.md`. Return here only
when that workflow needs relationship or dependency expansion.

## Route Selection

- Known Jira work items usually need native workitem details plus relationship
  context.
- Projects and goals need native details plus Jira, docs, search, PR, and
  meeting evidence.
- For topic onboarding, start with one knowledge search and one native-work
  search; compare formal epic, project, goal, and page anchors by source-defined
  hierarchy. Prefer the anchor linking current delivery/code; use context or
  responsibility only for dependencies or people. Reuse returned output before
  another projection and add a focused read whenever it can resolve a material
  scope, relationship, owner, freshness, or delivery gap. Do not retry synonyms
  or continue once those claims are supported or the remaining evidence is
  unavailable.
- For restart, handoff, or OOO catch-up, load
  `../twg-status-rollups/references/personal-work-summary.md` and follow its
  restart guidance. Infer priority across connected evidence and hydrate only
  anchors that change the user's next action.
- Dependency map and page/topic prompts need hydrated anchors before broad search is
  evidence. Map broad subdomains before assigning owners/experts.
- Raw graph-query/debugging surfaces are not the default dependency-map route.
  Use them only when the user explicitly asks for that query language or typed
  commands cannot express the required edge.

## Evidence Policy

For central candidates, fetch fields, owner, status, body, comments, URLs, and
context edges/links/people/teams/projects/goals/docs/PRs/commits/branches.
Use summary first; escalate to full for the central anchor and the small set of
high-signal related anchors whose URLs, comments, body content, or provenance
can change the requested conclusion. Do not stop at a numeric count while a
material gap remains. Treat third-party URLs as nodes and retain relationship
direction. Use inline or compact output first; targeted native follow-ups may
close material missing relationships or coverage gaps within the established
scope while each follow-up can change the conclusion.

## Expansion Rules

- Expand by relationship role, not raw count.
- Hydrate parent, epic, inbound peer, blocker, consumer, central page, external
  design, PR, commit, branch, assignee, reporter, contributor, and reviewer
  signals when they change direction, risk, ownership, or next action.
- Fetch known older links directly by URL, key, ID, or ARI instead of widening
  the whole graph blindly.
- Use strong query variants rather than many synonyms.
- After the first source fetch plus context/search pass, pause and compare the
  evidence against the requested output. If owner, status, relation, recency,
  evidence URL/key, and the requested source roles/content are present,
  synthesize instead of widening. Otherwise continue only for a candidate that
  can close the material gap.
- If a context or graph-backed command returns the same backend/coverage error
  twice, do not keep probing adjacent graph paths. Record the coverage gap and
  continue with product-native hydrated evidence.
- Stop when the next candidate would not add new entities, links, contributors,
  teams, decisions, ownership, risk, or next action.

## Graph Visualization

For graph requests, pipe typed context output to `twg visualize`. Keep entities
that change direction, ownership, risk, or next action; collapse duplicates.

## Output Shape

- Anchor snapshot: what it is and why it matters.
- For OOO catch-ups, synthesize by priority workstream and next action; do not
  add a relationship table. For other context work, include entity,
  relationship, owner, importance, and evidence.
- Risks and dependencies, separating confirmed edges from inferred relationships.
- Suggested next actions.
- Confidence and gaps when evidence is incomplete, access-limited, stale, or
  sampled.

## Anti-Patterns

- Do not stop at search results without hydrating anchors.
- Do not treat `stdout_shape` as a complete entity or URL inventory.
- Do not skip peer expansion for graph/dependency prompts because peers look
  "Done".
- Do not dismiss a 1-hop candidate by title alone.
- Do not hand-roll graph HTML.
