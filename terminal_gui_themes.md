# Agent Context: Terminal.Gui Theme Subsystem

This document provides architectural context, strict schema specifications, and implementation patterns for generating or modifying Terminal User Interface (TUI) theme layouts within this repository.

Refer to these guidelines when modifying `themes.json`, extending `ThemeLoader.cs`, or writing layout components.

---

## 🗺️ Architectural Overview

The application utilizes a decoupling layer that bridges modern TrueColor palettes with a component-scoped schema matching the state requirements of **Terminal.Gui (v2)**.

### Core Components

1. **`themes.json`**: The central design token registry containing 10 predefined color templates.
2. **`ThemeLoader.cs`**: Ingests JSON profiles at startup, registers them globally into `Colors.ColorSchemes`, and exposes fallback color translation layers for limited hardware emulators.
3. **`Program.cs`**: Initializes frames and demonstrates live interaction cascading via an atomic layout repaint workflow.

---

## 🎨 Theme Configuration Schema (`themes.json`)

All theme profiles are grouped within a root `"themes"` array using a configuration blueprint mapping tokens to individual UI widget types.

### Structural Requirements

Every theme block must declare a `name`, a debugging helper group (`base_ansi`), and a mandatory `components` object with exactly five framework targets:

| Component Target | Purpose | State Mappings Required |
| :--- | :--- | :--- |
| `window` | System application container background frameworks. | `normal`, `focus`, `hotNormal`, `hotFocus` |
| `dialog` | Ephemeral message alerts, popups, and confirmation boxes. | `normal`, `focus`, `hotNormal`, `hotFocus` |
| `menu` | Navigation sidebars or top-level action drop-downs. | `normal`, `focus`, `hotNormal`, `hotFocus` |
| `button` | Clickable interactive actions or control toggles. | `normal`, `focus`, `hotNormal`, `hotFocus` |
| `border` | Framing boundaries enclosing frames or viewport lines. | `normal`, `focus`, `hotNormal`, `hotFocus` |

### Value Formatting Constraint

State fields use an explicit slash-separated format string: `"Foreground/Background"`.

* **TrueColor (Hex):** Must use 24-bit `#RRGGBB` notations (e.g., `"#CDD6F4/#1E1E2E"`).
* **Legacy Fallback Names:** Must match standard named string primitives (e.g., `"BrightYellow/Blue"`, `"Black/BrightWhite"`).

---

## ⚙️ Execution Pipeline & The Fallback Engine

When generating code patches or creating automated framework tests, maintain the integrity of the multi-tiered rendering pipeline:
