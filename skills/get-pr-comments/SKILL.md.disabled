---
name: get-pr-comments
user-invocable: false
description: "Use when an agent needs to retrieve GitHub pull request review comments, review threads, or general review feedback. Supports an explicit repository and PR number or the active pull request, with optional filtering for unresolved, resolved, inline, and timeline comments. Read-only: never changes or resolves review threads."
---

# Get Pull Request Comments

Retrieve pull request review feedback in a predictable, read-only form. Use this
skill before addressing review comments, summarizing review feedback, or deciding
whether a PR needs changes.

## Parameters

Accept these parameters when the caller provides them:

| Parameter | Type | Default | Meaning |
| --- | --- | --- | --- |
| `owner` | string | repository owner from workspace | GitHub repository owner. |
| `repository` | string | repository name from workspace | GitHub repository name. |
| `pullRequest` | integer | active PR | Pull request number. |
| `active` | boolean | `true` when no PR number is given | Use the checked-out active PR. |
| `refresh` | boolean | automatic | Force a fresh GitHub read. |
| `includeResolved` | boolean | `false` | Include resolved inline review threads. |
| `includeTimeline` | boolean | `true` | Include general PR reviews and timeline comments. |
| `includeSystem` | boolean | `false` | Include bot/system events that are not review feedback. |
| `file` | string | all files | Limit inline comments to a repository-relative file path. |
| `author` | string | all authors | Limit comments to a GitHub login. |
| `format` | `summary` or `json` | `summary` | Select human-readable output or normalized structured output. |

If `pullRequest` is supplied, require `owner` and `repository` unless the GitHub
integration can infer them unambiguously. If neither `pullRequest` nor `active`
is supplied, use the active PR only when one is available; otherwise report the
missing repository and PR parameters.

## Retrieval Procedure

1. Determine the target using the parameters.
2. For the active PR, call the active pull request tool once without forcing a
   refresh.
3. Inspect `lastUpdatedAt`. If the cached result is less than three minutes old,
   call the same tool again with `refresh: true`. Otherwise use the cached result
   unless the caller explicitly set `refresh: true`.
4. For an explicit PR number, use the GitHub pull request or issue/PR details
   tool that returns review threads and timeline comments. Do not use repository
   text search as the primary retrieval method.
5. Read both sources when available:
   - `reviewThreads` for inline comments, thread state, and file locations.
   - `timelineComments` for general reviews, change requests, and non-inline feedback.
6. Filter the result:
   - Exclude resolved threads unless `includeResolved` is `true`.
   - Exclude system-only events unless `includeSystem` is `true`.
   - Apply `file` and `author` filters to both inline comments and timeline comments
     when the fields exist.
7. Preserve the original comment text, author, file, line or position, timestamp,
   thread ID, resolution state, and comment type when available.
8. Never resolve, reply to, edit, label, approve, or otherwise mutate the PR.
   Use the separate review-comment workflow when an action is requested.

## Normalized Output

For `format: summary`, return:

1. Repository and PR identity.
2. PR title, state, and URL when available.
3. Retrieval timestamp and whether the result was refreshed.
4. Unresolved inline threads first, grouped by file and ordered by severity or
   source order when severity is unavailable.
5. General review and timeline feedback next.
6. Resolved threads only when requested.
7. A short note for skipped filters, unavailable fields, or retrieval limitations.

For `format: json`, return an object with this shape:

```json
{
  "repository": "owner/name",
  "pullRequest": 0,
  "title": "",
  "state": "",
  "url": "",
  "retrievedAt": "",
  "refreshed": false,
  "reviewThreads": [
    {
      "id": "",
      "isResolved": false,
      "canResolve": false,
      "file": "",
      "comments": [
        {
          "author": "",
          "body": "",
          "commentType": "",
          "createdAt": "",
          "line": null,
          "position": null
        }
      ]
    }
  ],
  "timelineComments": [],
  "limitations": []
}
```

Use `null` for unavailable scalar values and an empty array for unavailable
collections. Do not invent line numbers, authors, timestamps, or resolution
states.

## Failure Handling

- If no active PR exists, request or report `owner`, `repository`, and `pullRequest`.
- If GitHub authentication fails, report that authentication is required and do
  not substitute local Git history for review comments.
- If the PR exists but comments are unavailable, return the PR metadata and a
  limitation explaining which comment source was unavailable.
- If the result is truncated, say so and preserve the available comments.
- If cached and refreshed results disagree, prefer the refreshed result and note
  that the PR changed during retrieval.

## CLI Fallback

Use the bundled Bash script only when the structured GitHub tools are unavailable
or the caller explicitly requests a local CLI fallback:

```bash
skills/get-pr-comments/scripts/get-pr-comments.sh \
  --owner OWNER \
  --repo REPOSITORY \
  --pr NUMBER \
  --format json
```

The script requires `bash`, `gh`, `jq`, and `git`. It supports `--file`,
`--author`, and `--include-system` filters, uses paginated REST responses, and
cleans up temporary files automatically. It never changes the pull request.

The REST fallback returns `reviewComments` and `timelineComments`, but its
`reviewThreads` array is empty because REST does not provide GraphQL thread
resolution state. Its `limitations` array must be preserved and surfaced to the
caller. Do not claim that unresolved or resolved filtering was applied when the
fallback was used.

Run the script from the repository root or use its absolute path. Do not copy
its output into a committed file unless the caller explicitly requests an
export.

## Review Handoff

When another workflow will address the feedback, pass along the normalized
thread IDs and whether each thread is resolved. Do not resolve threads in this
skill; the addressing workflow owns implementation, replies, and resolution.
