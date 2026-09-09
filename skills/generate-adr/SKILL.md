---
name: generate-adr
user-invocable: false
description: "Use when recording a significant architecture or engineering decision. Generates a concise ADR from context, constraints, alternatives, decision, consequences, operational impact, security, and cost considerations."
---

# Generate Architecture Decision Record

Create a reviewable ADR that complements `architecture.md`.

## Parameters

- `title`: decision title.
- `context`: problem, constraints, and forces.
- `decision`: selected approach.
- `alternatives`: considered alternatives and rejection reasons.
- `status`: proposed, accepted, superseded, or deprecated; default proposed.
- `outputPath`: optional ADR path; default project convention.
- `write`: default `false`.

## Workflow

1. Gather the decision context and distinguish facts from assumptions.
2. Identify affected systems, ownership boundaries, dependencies, and risks.
3. Record alternatives, decision criteria, and consequences.
4. Add operational, security, accessibility, performance, and cost impact where relevant.
5. Check for related or superseded ADRs.
6. Produce the ADR draft and write it only when explicitly authorized.

## Safety

- Never invent stakeholder approval or accepted status.
- Mark unknown consequences as open questions.
- Do not include secrets or sensitive incident details.
- Prefer superseding an old ADR over silently contradicting it.

## Output

Return ADR metadata, draft content or path, related decisions, open questions,
reviewers or owners, and follow-up actions.
