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

- Remote state always (S3 + DynamoDB lock or equivalent) — never local state in shared repos.
- Use separate var files per environment rather than inline `terraform.workspace` conditions.
- Never store secrets in `.tfvars` committed to source control — use a secrets manager reference.
- `prevent_destroy = true` lifecycle on stateful resources (databases, buckets with data).
- Lock provider versions: `version = "~> 5.0"` — never `version = ">= 0"`.
- Run `tflint` and `terraform validate` in CI on every PR.
- Run terraform plan before committing code to Github. If the repository includes or mentions
  iterraform.sh use that pattern to run the plan. When creating a Git Commit/PR add the relevant
  section(s) of the terraform plan output as a code block. If the plan output includes
  replacing or destroying any resource, that section MUST be included, and there needs to be a
  block in the comments warning that the terraform plan includes replacing or destroying resources.

## Variables

- Every variable needs `description` and `type`.
- Use `validation` blocks for constrained inputs.
- Mark sensitive variables `sensitive = true` — never output sensitive values.

## Code Generation Defaults

- Always include `versions.tf` when creating a new root module.
