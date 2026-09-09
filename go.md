# Go

Open this file only when writing or modifying Go code. General rules stay in the root `AGENTS.md`.

## Configuration

- Use `os.UserConfigDir()` for user-specific configuration paths.
- Prefer typed configuration and standard environment-variable handling before adding a dependency.

## Formatting & Linting

- Formatter: `gofmt` / `goimports` (authoritative — never suggest conflicting style).
- Linter: `golangci-lint`.
- Validate with `go test ./...`, `go vet ./...`, and the race detector when concurrency is involved.
- Use `govulncheck` when dependency or standard-library vulnerability analysis is required.

## Error Handling

- Always return errors explicitly — never ignore them with `_`.
- Wrap errors with context using `fmt.Errorf("...: %w", err)`.
- Use sentinel errors (`var ErrFoo = errors.New(...)`) for errors callers should match against.
- No `panic` in library code; panic only acceptable in `main` for unrecoverable init failures.
- Prefer early return over nested `if err != nil` blocks.

## Testing

- Table-driven tests are the default (`t.Run` subtests with a slice of test cases).
- Use `testify/assert` and `testify/require` — `require` for fatal assertions, `assert` for non-fatal.
- Test files live alongside source (`_test.go`). Integration tests use `//go:build integration`.
- Mock interfaces, not concrete types. Prefer `testify/mock` or hand-rolled fakes.

## Conventions

- Module path reflects real import path — no `example.com/...` placeholders.
- Exported functions/types must have godoc comments.
- Context is always the first parameter: `func Foo(ctx context.Context, ...)`.
- No `init()` functions unless there's no alternative.
- Prefer `slog` for structured logging (Go 1.21+).
- Apply timeouts to HTTP clients and servers.
- Propagate cancellation through goroutines and shut down long-running services gracefully.

## Code Generation Defaults

- Include `package` declaration and godoc on exported symbols.
