---
title: "GNU Makefile Guidance"
description: "Load this file when generating, editing, or evaluating a GNU Makefile."
applies_to: ["Makefile", "*.mk"]
---

# GNU Makefile Guidance

Load this file when generating, editing, or evaluating a GNU Makefile. These
rules follow the [Cloud Posse Makefile Best Practices](https://docs.cloudposse.com/best-practices/developer/makefile/)
and are the default for new Makefiles. For an existing Makefile, adapt them to
the project's existing conventions and constraints. Use `cmake_make.md` only to
choose between CMake and GNU Make.

## 1. Target Naming & Namespacing

* **Namespace All Targets:** Namespace targets in larger Makefiles (e.g., `docker/build`).
* **Use Forward Slash (`/`):** ALWAYS use `/` as the target namespace delimiter.
* **NEVER Use Colons (`:`):** DO NOT use `:` in target names (e.g., `docker:build`); this silently breaks target dependencies in Make.

## 2. File Modularity & Includes

* **Separate by Namespace:** Stick all targets within a specific namespace into a separate file (e.g., place all `docker/*` targets into `Makefile.docker`).
* **Root Includes:** Include these modular files at the top of the root `Makefile` using the `-include` directive (e.g., `-include tasks/Makefile.*`).
* **Suppress Errors:** The leading `-` MUST be used to tell `make` not to error if the included folder is empty.

## 3. Variables & Arguments

* **Sane Defaults:** Set default values for all variables using the `?=` operator (e.g., `DOCKER_TAG ?= latest`) to avoid requiring excessive command-line arguments.
* **Environment Variables:** Treat environment variables like function arguments. Expect them to be passed at runtime (e.g., `make docker/build DOCKER_TAG=dev`).
* **No Evals:** DO NOT use `$(eval ...)`. It causes confusing execution paths during Make's template preprocessing and execution phases.

## 4. Execution Logic & Dependencies

* **Write Small Targets:** DO NOT embed complex, multi-line bash scripts directly inside a Makefile target.
* **Delegate Complex Logic:** Extract complex logic into standalone shell scripts and execute those scripts from within the Makefile target.
* **Use Dependencies:** Define prerequisite target dependencies to ensure required steps (like `deps`) execute and succeed prior to the main target (e.g., `build: deps`).

## 5. Root Makefile Standards

* **Standard Targets:** Define `deps`, `build`, `install`, and `all` when they match the project workflow. Do not define a separate `default` target; the default goal is `help` (see below).
* **Display Artifact Paths** Always display the full path to generated artifacts. For Build steps, include instructions for running the application.
* **Strict Tabbing:** All leading whitespace for commands MUST be tabbed (`^T`), never spaces.
* **Default Target:** Make help the default target, for example with `.DEFAULT_GOAL := help`.

## 6. Reliability

* Mark command targets `.PHONY` unless they intentionally create a file.
* Validate required tools and variables before starting a destructive or remote operation.
* Avoid recursive `make` unless separate working directories or independent build graphs require it.
* Consider parallel execution when targets are independent; use `.NOTPARALLEL` only when ordering is required.
* Test the help target and the main dependency paths after changing the Makefile.

## 7. Self-Documenting Help System

* **Comment Syntax:** Document all user-facing targets with a double-hash comment (`##`) on the preceding line.
* **Help Implementation:** You MUST include the following `awk` script in the Makefile to auto-generate the help menu:

<!-- markdownlint-disable MD010 -->

```makefile
## This help screen
help:
	@printf "Available targets:\n\n"
	@awk '/^[a-zA-Z\-_0-9%:\\]+/ { \
		helpMessage = match(lastLine, /^## (.*)/); \
		if (helpMessage) { \
			helpCommand = $$1; \
			helpMessage = substr(lastLine, RSTART + 3, RLENGTH); \
			gsub("\\\\", "", helpCommand); \
			gsub(":+$$", "", helpCommand); \
			printf "  \x1b[32;01m%-35s\x1b[0m %s\n", helpCommand, helpMessage; \
		} \
	} \
	{ lastLine = $$0 }' $(MAKEFILE_LIST) | sort -u
	@printf "\n"
```

<!-- markdownlint-enable MD010 -->

## 8. Dependency Inventory & SBOM Targets

Every project Makefile that builds or ships code MUST provide these targets so
dependencies are auditable the same way in every repo.

* **`libs-version`:** Print every direct and transitive library the shipped
  artifact depends on, one `name  version` pair per line, aligned in columns,
  sorted by name. Scope it to what ships (exclude test and dev-only projects).
  Depend on `deps` so the lock/restore state is current.
* **`sbom`:** Write a CycloneDX JSON SBOM to `$(SBOM_DIR)/bom.json`
  (`SBOM_DIR ?= artifacts/sbom`). Depend on `deps`.
* **`sbom/install`:** Install the SBOM tool once, pinned in a committed
  manifest or lock file, so the tool version is reproducible. Once the
  manifest is committed, restore from it (e.g. `dotnet tool restore`) rather
  than installing the latest version, and invoke the tool by the exact,
  case-sensitive command name the manifest declares. `sbom` MUST fail
  with a clear "run `make sbom/install`" message when the tool is missing.
* **Publish/release targets:** Also write the SBOM next to each published
  artifact (`<publish dir>/bom.json`). A missing SBOM tool MUST only warn and
  skip (e.g. `sbom.sh ... --skip-if-missing`) so publishing is not blocked;
  genuine tool failures still fail the build. The standalone `sbom` target
  stays strict.
* **`clean`:** Remove `$(SBOM_DIR)`. Add `artifacts/` (or the build output
  directory) to `.gitignore`.
* **Scripts, not recipes:** Put the logic in `scripts/libs-version.sh` and
  `scripts/sbom.sh` (see section 4); the Makefile only passes variables.
  Mark all of these targets `.PHONY` and document them with `##` help text.

Pick the ecosystem's standard tooling rather than parsing files by hand:

| Ecosystem | `libs-version` | `sbom` |
| --- | --- | --- |
| .NET | `dotnet list <project> package --include-transitive` | `dotnet-CycloneDX` (local tool, `dotnet-tools.json`) |
| Node | `npm ls --all --omit=dev` | `@cyclonedx/cyclonedx-npm` |
| Python | `pip freeze` or `uv pip list` in the project venv | `cyclonedx-py` |
| Go | `go list -m all` | `cyclonedx-gomod` |
| Rust | `cargo tree --prefix none` | `cargo-cyclonedx` |

Verify each tool's flags against its `--help` before writing the script; do not
rely on remembered options.

* **Self-reporting binaries:**
  All executables should have the following command line arguments:
  - `--libs-version` returns the same format as make --libs-version.
  - `--sbom` returns the bom.json.
