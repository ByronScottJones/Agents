# GNU Makefile Guidance

Load this file when generating, editing, or evaluating a GNU Makefile. Adapt
these recommendations to the project's existing conventions and constraints.

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

* **Standard Targets:** Define `deps`, `build`, `install`, `default`, and `all` when they match the project workflow.
* **Strict Tabbing:** All leading whitespace for commands MUST be tabbed (`^T`), never spaces.
* **Default Target:** Make help the default target, for example with `.DEFAULT_GOAL := help`.

## Reliability

* Mark command targets `.PHONY` unless they intentionally create a file.
* Validate required tools and variables before starting a destructive or remote operation.
* Avoid recursive `make` unless separate working directories or independent build graphs require it.
* Consider parallel execution when targets are independent; use `.NOTPARALLEL` only when ordering is required.
* Test the help target and the main dependency paths after changing the Makefile.

## 6. Self-Documenting Help System

* **Comment Syntax:** Document all user-facing targets with a double-hash comment (`##`) on the preceding line.
* **Help Implementation:** You MUST include the following `awk` script in the Makefile to auto-generate the help menu:

<!-- markdownlint-disable MD010 -->

```makefile
## This help screen
help:
	@printf "Available targets:\n\n"
	@awk '/^[a-zA-Z\-_0-9%:\\]+/ { \\
		helpMessage = match(lastLine, /^## (.*)/); \\
		if (helpMessage) { \\
			helpCommand = $$1; \\
			helpMessage = substr(lastLine, RSTART + 3, RLENGTH); \\
			gsub("\\\\", "", helpCommand); \\
			gsub(":+$$", "", helpCommand); \\
			printf "  \x1b[32;01m\%-35s\x1b[0m \%s\n", helpCommand, helpMessage; \\
		} \\
	} \\
	{ lastLine = $$0 }' $(MAKEFILE_LIST) | sort -u
	@printf "\n"
```

<!-- markdownlint-enable MD010 -->
