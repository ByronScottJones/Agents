# HTML

Open this file only when writing or modifying HTML. General rules stay in the root `AGENTS.md`.

## Formatting & Linting

- Formatter: `prettier` (authoritative — never suggest conflicting style).
- Linter: none.

## Conventions

- Semantic HTML first (`<nav>`, `<main>`, `<article>`, etc.).
- No inline styles unless generated dynamically. Ignore if the user explicitly states they want a single HTML file.
- Always include `<!doctype html>`, a `lang` attribute, UTF-8 charset, viewport metadata, and a meaningful `<title>` in documents.
- Maintain a logical heading hierarchy and use labels associated with form controls.
- Always include `alt` on images; use `aria-*` attributes only when native HTML semantics are insufficient.
- Preserve keyboard access, visible focus, and an accessible association between controls and validation errors.
- Add `rel="noopener noreferrer"` to external links that use `target="_blank"`.
- Use responsive image dimensions and meaningful alternative text.

## Validation

- Validate HTML with the project's configured validator or formatter.
- Test keyboard navigation and important states in a browser when the document is interactive.
