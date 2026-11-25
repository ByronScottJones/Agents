#!/usr/bin/env bash
# Print the current time as ISO 8601 UTC, and with --apply write it into the
# three timestamp lines of the living documents:
#
#   HANDOFF.md      "- Last updated: ..."
#   HANDOFF.md      "- Results last verified ...:"
#   ENGINEERING.md  "- Last reviewed against the code: ..."
#
# Run it after the verification commands, so "Results last verified" is true.
# Nothing is written unless all three lines are found.
#
# Usage: scripts/docs-stamp.sh [--apply]
# Environment (for tests): DOCS_ROOT is the project root (default: this
# repository); DOCS_STAMP_NOW replaces the clock.
set -euo pipefail

apply=0
case "${1:-}" in
  "") ;;
  --apply) apply=1 ;;
  *)
    echo "usage: $0 [--apply]" >&2
    exit 2
    ;;
esac
if [[ $# -gt 1 ]]; then
  echo "usage: $0 [--apply]" >&2
  exit 2
fi

root="${DOCS_ROOT:-$(cd "$(dirname "$0")/.." && pwd)}"
now="${DOCS_STAMP_NOW:-$(date -u +%Y-%m-%dT%H:%M:%SZ)}"
printf '%s\n' "$now"
[[ $apply -eq 1 ]] || exit 0

stamp='[0-9]{4}-[0-9]{2}-[0-9]{2}T[0-9]{2}:[0-9]{2}:[0-9]{2}Z'
handoff="$root/HANDOFF.md"
spec="$root/ENGINEERING.md"

# Rewrite one line of one file into a temporary file, or fail when it is absent.
# awk is used instead of sed -i, which differs between macOS and GNU.
rewrite() { # file, ERE for the whole line, replacement prefix, suffix
  local file="$1" pattern="$2" prefix="$3" suffix="$4" output
  grep -Eq "$pattern" "$file" || {
    echo "ERROR: $file has no line matching: $pattern" >&2
    return 1
  }
  output="$(mktemp)"
  awk -v pattern="$pattern" -v prefix="$prefix" -v suffix="$suffix" -v now="$now" '
    !done && $0 ~ pattern { print prefix now suffix; done = 1; next }
    { print }
  ' "$file" >"$output"
  printf '%s' "$output"
}

work="$(mktemp -d)"
trap 'rm -rf "$work"' EXIT

first="$(rewrite "$handoff" "^- Last updated: $stamp\$" '- Last updated: ' '')"
mv "$first" "$work/handoff.1"
second="$(rewrite "$work/handoff.1" "^- Results last verified $stamp:\$" '- Results last verified ' ':')"
mv "$second" "$work/handoff.2"
third="$(rewrite "$spec" "^- Last reviewed against the code: $stamp\$" '- Last reviewed against the code: ' '')"
mv "$third" "$work/spec"

# All three lines were found; now replace the documents.
cat "$work/handoff.2" >"$handoff"
cat "$work/spec" >"$spec"
