#!/usr/bin/env bash

# Retrieve GitHub pull request review and issue comments as normalized JSON.
# This is a read-only fallback for the get-pr-comments skill. It requires gh and jq.

set -euo pipefail

usage() {
    cat <<'EOF'
Usage: get-pr-comments.sh [options]

Retrieve pull request comments using the GitHub CLI REST API.

Options:
  --owner OWNER             Repository owner.
  --repo REPOSITORY         Repository name.
  --pr NUMBER               Pull request number.
  --file PATH               Include only comments for PATH.
  --author LOGIN            Include only comments by LOGIN.
  --include-system          Include comments from bot/system authors.
  --format FORMAT           Output summary or json (default: json).
  -h, --help                Show this help.

When --owner, --repo, or --pr is omitted, the script uses the current GitHub
repository and the open pull request for the current branch.

Limitations:
  The REST fallback does not expose GraphQL review-thread resolution state.
  It reports review comments and issue/timeline comments, and marks its output
  with a limitation explaining that resolution filtering is unavailable.
EOF
}

error_json() {
    jq -n --arg message "$1" '{error: $message}'
}

require_command() {
    local command_name="$1"
    if ! command -v "$command_name" >/dev/null 2>&1; then
        error_json "${command_name} is not installed or not in PATH." >&2
        exit 1
    fi
}

owner=""
repository=""
pull_request=""
file_filter=""
author_filter=""
include_system=false
format="json"

while [[ $# -gt 0 ]]; do
    case "$1" in
        --owner)
            [[ $# -ge 2 ]] || { error_json "--owner requires a value." >&2; exit 2; }
            owner="$2"
            shift 2
            ;;
        --repo)
            [[ $# -ge 2 ]] || { error_json "--repo requires a value." >&2; exit 2; }
            repository="$2"
            shift 2
            ;;
        --pr)
            [[ $# -ge 2 ]] || { error_json "--pr requires a value." >&2; exit 2; }
            pull_request="$2"
            shift 2
            ;;
        --file)
            [[ $# -ge 2 ]] || { error_json "--file requires a value." >&2; exit 2; }
            file_filter="$2"
            shift 2
            ;;
        --author)
            [[ $# -ge 2 ]] || { error_json "--author requires a value." >&2; exit 2; }
            author_filter="$2"
            shift 2
            ;;
        --include-system)
            include_system=true
            shift
            ;;
        --format)
            [[ $# -ge 2 ]] || { error_json "--format requires a value." >&2; exit 2; }
            format="$2"
            shift 2
            ;;
        -h|--help)
            usage
            exit 0
            ;;
        *)
            error_json "Unknown option: $1" >&2
            usage >&2
            exit 2
            ;;
    esac
done

case "$format" in
    json|summary) ;;
    *)
        error_json "--format must be json or summary." >&2
        exit 2
        ;;
esac

require_command gh
require_command jq
require_command git

git rev-parse --is-inside-work-tree >/dev/null 2>&1 || {
    error_json "Not a git repository."
    exit 1
}

if [[ -z "$owner" || -z "$repository" ]]; then
    repo_metadata="$(gh repo view --json owner,name 2>/dev/null || true)"
    if [[ -z "$owner" ]]; then
        owner="$(jq -r '.owner.login // empty' <<<"$repo_metadata")"
    fi
    if [[ -z "$repository" ]]; then
        repository="$(jq -r '.name // empty' <<<"$repo_metadata")"
    fi
fi

if [[ -z "$pull_request" ]]; then
    pull_request="$(gh pr view --json number --jq '.number' 2>/dev/null || true)"
fi

if [[ -z "$owner" || -z "$repository" ]]; then
    error_json "Repository owner and name could not be determined. Use --owner and --repo."
    exit 1
fi

if [[ -z "$pull_request" ]]; then
    error_json "No open pull request found for the current branch. Use --pr NUMBER."
    exit 1
fi

if ! [[ "$pull_request" =~ ^[0-9]+$ ]]; then
    error_json "Pull request number must be an integer."
    exit 2
fi

work_dir="$(mktemp -d "${TMPDIR:-/tmp}/get-pr-comments.XXXXXX")"
trap 'rm -rf "$work_dir"' EXIT

review_file="$work_dir/review-comments.jsonl"
issue_file="$work_dir/issue-comments.jsonl"
: >"$review_file"
: >"$issue_file"

review_endpoint="repos/${owner}/${repository}/pulls/${pull_request}/comments"
issue_endpoint="repos/${owner}/${repository}/issues/${pull_request}/comments"

if ! gh api --paginate "$review_endpoint" --jq '.[] | {
    id: (.id | tostring),
    author: (.user.login // null),
    body: (.body // ""),
    file: (.path // null),
    line: (.line // null),
    position: (.position // null),
    createdAt: (.created_at // null),
    commentType: "review_comment",
    isResolved: null,
    canResolve: null
}' >"$review_file"; then
    error_json "Unable to retrieve pull request review comments. Check gh authentication and permissions."
    exit 1
fi

if ! gh api --paginate "$issue_endpoint" --jq '.[] | {
    id: (.id | tostring),
    author: (.user.login // null),
    body: (.body // ""),
    file: null,
    line: null,
    position: null,
    createdAt: (.created_at // null),
    commentType: "issue_comment",
    isResolved: null,
    canResolve: null
}' >"$issue_file"; then
    error_json "Unable to retrieve pull request timeline comments. Check gh authentication and permissions."
    exit 1
fi

comments_json="$(jq -s --arg file "$file_filter" --arg author "$author_filter" --argjson includeSystem "$include_system" '
    map(select(
        ($file == "" or .file == $file)
        and ($author == "" or .author == $author)
        and ($includeSystem or (.author != null and (.author | test("(^|[-_])bot$|\[bot\]$"; "i") | not)))
    ))
' "$review_file" "$issue_file")"

jq -n \
    --arg repository "${owner}/${repository}" \
    --argjson pullRequest "$pull_request" \
    --arg retrievedAt "$(date -u '+%Y-%m-%dT%H:%M:%SZ')" \
    --argjson comments "$comments_json" \
    '{
        repository: $repository,
        pullRequest: $pullRequest,
        title: null,
        state: null,
        url: null,
        retrievedAt: $retrievedAt,
        refreshed: true,
        reviewThreads: [],
        timelineComments: ($comments | map(select(.commentType == "issue_comment"))),
        reviewComments: ($comments | map(select(.commentType == "review_comment"))),
        limitations: [
            "REST fallback does not expose GraphQL review-thread resolution state.",
            "PR metadata such as title, state, and URL is unavailable unless retrieved separately."
        ]
    }' | if [[ "$format" == "summary" ]]; then
        jq '{repository, pullRequest, reviewComments: (.reviewComments | length), timelineComments: (.timelineComments | length), limitations}'
    else
        cat
    fi
