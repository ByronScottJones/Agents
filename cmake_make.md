---
title: "Build System Selection"
description: "Load this file when choosing between CMake and GNU Make for a project. For writing a Makefile, load makefile.md."
applies_to: ["CMakeLists.txt"]
---

# Build System Selection: CMake vs. GNU Make

**Context & Objective:** Before generating a build file, evaluate whether CMake
or GNU Make is the correct tool. When GNU Make is selected, follow
`makefile.md`.

## Build System Selection

Before drafting a build configuration, assess the project scope to determine the appropriate tool.

### When to use CMake INSTEAD OF Make

* **Complex Compilation:** Use CMake for heavy C/C++ projects requiring cross-platform compilation (Windows, macOS, Linux).
* **Embedded & Hardware Targets:** Use CMake when managing complex library linking, automated dependency discovery, or configuring toolchains for embedded microcontrollers (e.g., compiling firmware for the ESP32-S3).
* **IDE Generation:** Use CMake when the development team requires native project files generated for modern IDEs (e.g., Visual Studio, CLion).

### When to use Make INSTEAD OF CMake

* **Task Orchestration (The "Glue"):** Use Make as a general-purpose task runner for infrastructure operations, such as orchestrating Terraform deployments or managing cross-account AWS utilities.
* **Script Wrapping:** Use Make to standardize execution commands for Bash scripts, Go utilities, or local application packaging where complex cross-platform C-compilation is not required.
* **Last-Mile Automation:** Use Make when the goal is simply to provide a unified developer interface (e.g., `make deps`, `make build`) over a diverse set of existing CLI tools.

### When to use CMake AND Make together

* **Generator to Runner:** Recognize that CMake is a *build system generator*. In many Linux/Unix environments, CMake is executed first to dynamically *generate* a `Makefile`, and Make is subsequently used to actually execute the compilation instructions.

When CMake is selected, prefer Ninja or the project's established generator
when it provides faster or more portable builds than GNU Make.
