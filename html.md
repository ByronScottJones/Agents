# HTML

Open this file only when writing or modifying HTML. General rules stay in the root `AGENTS.md`.

## Formatting & Linting

- Formatter: `prettier` (authoritative — never suggest conflicting style).
- Linter: none.

## Conventions

- Semantic HTML first (`<nav>`, `<main>`, `<article>`, etc.).
- No inline styles unless generated dynamically. Ignore if the user explicitly states they want a single HTML file.
- Always include `alt` on images; `aria-*` attributes where needed for accessibility.
