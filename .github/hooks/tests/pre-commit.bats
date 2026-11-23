#!/usr/bin/env bats
# Tests for .github/hooks/pre-commit. Each test runs the hook against a staged
# change in a throwaway repository. Fake credentials are assembled at runtime
# so this file never contains a value the hook would flag.

setup() {
  HOOK="${BATS_TEST_DIRNAME}/../pre-commit"
  # bats removes BATS_TEST_TMPDIR after each test; route the hook's mktemp there.
  export TMPDIR="${BATS_TEST_TMPDIR}"
  REPO="${BATS_TEST_TMPDIR}/repo"
  mkdir -p "${REPO}"
  cd "${REPO}" || return 1
  git init -q -b main .
  git config user.email test@example.com
  git config user.name test
  FAKE_GITHUB_TOKEN="ghp_$(printf 'a%.0s' {1..36})"
  FAKE_AWS_KEY="AKIA$(printf 'A%.0s' {1..16})"
}

# stage CONTENT: write CONTENT to f.txt and stage it.
stage() {
  printf '%s\n' "$1" >f.txt
  git add f.txt
}

@test "clean change passes" {
  stage "plain text"
  run "${HOOK}"
  [ "${status}" -eq 0 ]
}

@test "github token is blocked" {
  stage "token ${FAKE_GITHUB_TOKEN}"
  run "${HOOK}"
  [ "${status}" -eq 1 ]
  [[ "${output}" == *"f.txt:1: github-token"* ]]
}

@test "matched value is never printed" {
  stage "token ${FAKE_GITHUB_TOKEN}"
  run "${HOOK}"
  [[ "${output}" != *"${FAKE_GITHUB_TOKEN}"* ]]
}

@test "aws access key is blocked" {
  stage "key ${FAKE_AWS_KEY}"
  run "${HOOK}"
  [ "${status}" -eq 1 ]
  [[ "${output}" == *"f.txt:1: aws-access-key"* ]]
}

@test "private key header is blocked" {
  stage "-----BEGIN RSA PRIVATE KEY-----" # secret-scan: ignore
  run "${HOOK}"
  [ "${status}" -eq 1 ]
  [[ "${output}" == *"f.txt:1: private-key"* ]]
}

@test "literal secret assignment is blocked" {
  stage 'password = "hunter2hunter2"' # secret-scan: ignore
  run "${HOOK}"
  [ "${status}" -eq 1 ]
  [[ "${output}" == *"f.txt:1: secret-assignment"* ]]
}

@test "environment reference assignment is allowed" {
  # The literal text "$PASSWORD" is the test input, so it must not expand.
  # shellcheck disable=SC2016
  stage 'password = "$PASSWORD"'
  run "${HOOK}"
  [ "${status}" -eq 0 ]
}

@test "real email address is blocked" {
  stage "contact someone@corp.com" # secret-scan: ignore
  run "${HOOK}"
  [ "${status}" -eq 1 ]
  [[ "${output}" == *"f.txt:1: email"* ]]
}

@test "placeholder and noreply emails are allowed" {
  stage "a@example.com b@users.noreply.github.com git@github.com"
  run "${HOOK}"
  [ "${status}" -eq 0 ]
}

@test "absolute home path is blocked" {
  stage "see /Users/realname/notes" # secret-scan: ignore
  run "${HOOK}"
  [ "${status}" -eq 1 ]
  [[ "${output}" == *"f.txt:1: home-path"* ]]
}

@test "placeholder home path is allowed" {
  # The literal text "$HOME" is the test input, so it must not expand.
  # shellcheck disable=SC2016
  stage 'see /Users/you/notes and $HOME/notes'
  run "${HOOK}"
  [ "${status}" -eq 0 ]
}

@test "ignore marker exempts a line" {
  stage "token ${FAKE_GITHUB_TOKEN} secret-scan: ignore"
  run "${HOOK}"
  [ "${status}" -eq 0 ]
}

@test "node_modules is skipped" {
  mkdir -p node_modules/pkg
  echo "token ${FAKE_GITHUB_TOKEN}" >node_modules/pkg/a.js
  git add node_modules
  run "${HOOK}"
  [ "${status}" -eq 0 ]
}

@test "only added lines are scanned" {
  stage "old line"
  git commit -q --no-verify -m first
  printf 'token %s\n' "${FAKE_GITHUB_TOKEN}" >>f.txt
  git add f.txt
  run "${HOOK}"
  [ "${status}" -eq 1 ]
  [[ "${output}" == *"f.txt:2:"* ]]
}

@test "local denylist pattern blocks, comments are ignored" {
  printf '# comment\nacmecorp\n' >"$(git rev-parse --git-path sensitive-denylist)"
  stage "AcmeCorp internal notes"
  run "${HOOK}"
  [ "${status}" -eq 1 ]
  [[ "${output}" == *"local-denylist"* ]]
}

@test "malformed denylist pattern fails closed without printing it" {
  printf '[\n' >"$(git rev-parse --git-path sensitive-denylist)"
  stage "plain text"
  run "${HOOK}"
  [ "${status}" -eq 2 ]
  [[ "${output}" == *"local-denylist"* ]]
  [[ "${output}" != *"["* ]]
}

@test "runs the repository's own hook and propagates its failure" {
  mkdir -p .git/hooks
  printf '#!/bin/sh\ntouch "%s/ran"\nexit 7\n' "${BATS_TEST_TMPDIR}" >.git/hooks/pre-commit
  chmod +x .git/hooks/pre-commit
  stage "plain text"
  run "${HOOK}"
  [ "${status}" -eq 7 ]
  [ -e "${BATS_TEST_TMPDIR}/ran" ]
}

@test "does not run the repository's own hook when the scan fails" {
  mkdir -p .git/hooks
  printf '#!/bin/sh\ntouch "%s/ran"\n' "${BATS_TEST_TMPDIR}" >.git/hooks/pre-commit
  chmod +x .git/hooks/pre-commit
  stage "token ${FAKE_GITHUB_TOKEN}"
  run "${HOOK}"
  [ "${status}" -eq 1 ]
  [ ! -e "${BATS_TEST_TMPDIR}/ran" ]
}

@test "installed as .git/hooks/pre-commit it does not run itself twice" {
  mkdir -p .git/hooks
  cp "${HOOK}" .git/hooks/pre-commit
  stage "plain text"
  run .git/hooks/pre-commit
  [ "${status}" -eq 0 ]
}

@test "no denylist file means no local findings" {
  stage "acmecorp internal notes"
  run "${HOOK}"
  [ "${status}" -eq 0 ]
}

@test "--help exits 0" {
  run "${HOOK}" --help
  [ "${status}" -eq 0 ]
  [[ "${output}" == *"Usage:"* ]]
}

@test "unknown argument exits 2" {
  run "${HOOK}" --bogus
  [ "${status}" -eq 2 ]
}
