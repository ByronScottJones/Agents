---
title: "Terraform"
description: "Open this file only when writing or modifying Terraform code."
applies_to: ["*.tf", "*.tfvars"]
---

# Terraform

Open this file only when writing or modifying Terraform (`.tf`) code. General rules stay in the root `AGENTS.md`.

## Formatting & Linting

- Formatter: `terraform fmt` (authoritative — never suggest conflicting style).
- Linter: `tflint`.
- Always run `terraform fmt -recursive` after any `.tf` edits.

## Structure

- Root module layout: `main.tf`, `variables.tf`, `outputs.tf`, `versions.tf`, `locals.tf`.
- Use modules for any resource group used in more than one place.
- Pin module and provider versions — never use open-ended constraints.

## Naming & Tagging

- Resources: `{type}_{description}` snake_case (e.g., `aws_security_group_web_ingress`).
- Variables and outputs: snake_case, descriptive — no unexplained abbreviations.
- Tags: always include `Name`, `Environment`, `ManagedBy = "terraform"` at minimum.

## State & Safety

- Use a locking-capable remote backend appropriate to the cloud provider; never use local state in shared repositories.
- Use separate var files per environment rather than inline `terraform.workspace` conditions.
- Never store secrets in `.tfvars` committed to source control — use a secrets manager reference.
- Consider `prevent_destroy = true` for production stateful resources after confirming it fits the recovery workflow.
- Lock provider versions: `version = "~> 5.0"` — never `version = ">= 0"`.
- Run `tflint` and `terraform validate` in CI on every PR.
- Run `terraform fmt -check` and review plans for unintended replacement or destruction.
- Run `terraform plan` before applying infrastructure changes. Redact sensitive values from
  plan artifacts and review any replacement or destruction explicitly.

## Variables

- Every variable needs `description` and `type`.
- Use `validation` blocks for constrained inputs.
- Mark sensitive variables `sensitive = true` — never output sensitive values.

## Code Generation Defaults

- Always include `versions.tf` when creating a new root module.
