# Architecture

Load this file when designing systems, changing boundaries, or recording
significant technical decisions.

## Design

- Prefer the simplest architecture that satisfies current requirements.
- Define ownership, dependency direction, trust boundaries, and failure modes.
- Distinguish synchronous interactions from asynchronous workflows.
- Consider operability, security, cost, performance, and migration impact together.
- Avoid introducing distributed-system complexity without a measured benefit.

## Decisions

- Record significant choices, rejected alternatives, assumptions, and consequences in ADRs.
- Keep public contracts explicit and versioned when compatibility matters.
- Revisit decisions when requirements, scale, or operational constraints change.
