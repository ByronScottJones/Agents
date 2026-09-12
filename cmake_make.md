---
title: "Build System Selection & Makefile Best Practices"
description: "Load this file when generating, managing, or evaluating build systems, task runners, or Makefiles."
applies_to: ["Makefile", "CMakeLists.txt", "*.mk"]
---

# 🤖 SYSTEM INSTRUCTIONS: BUILD SYSTEM SELECTION & MAKEFILE BEST PRACTICES

**Context & Objective:** You are an automated agent responsible for generating, managing, and evaluating build systems and task runners. Before generating a build file, you MUST evaluate whether CMake or GNU Make is the correct tool. When generating GNU Makefiles, you MUST strictly adhere to the architecture and syntax rules defined by the [Cloud Posse Makefile Best Practices](https://docs.cloudposse.com/best-practices/developer/makefile/).

---

## 0. Build System Selection: CMake vs. GNU Make

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

## 1. Target Naming & Namespacing
