---
title: "Terminal.Gui Theme Subsystem"
description: "Load this file when modifying themes.json, extending themeloader.cs, or writing Terminal.Gui theme layout components."
applies_to: ["themes.json", "themeloader.cs", "ThemeLoader.cs"]
---

# Agent Context: Terminal.Gui Theme Subsystem

This document provides architectural context, strict schema specifications, and
implementation patterns for generating or modifying Terminal User Interface
(TUI) theme layouts within this repository.

Files referenced here are located in `snippets/terminal_gui_themes`.

Refer to these guidelines when modifying `themes.json`, extending
`themeloader.cs`, or writing layout components.

---

## Architectural Overview

The application utilizes a decoupling layer that bridges modern TrueColor
palettes with a component-scoped schema matching the state requirements of
**Terminal.Gui v2.5** (`IApplication`, `Scheme`, `SchemeManager`). Terminal.Gui
v1 code uses a different API (`ColorScheme`, `Colors.ColorSchemes`); verify
against the pinned version before changing it.

### Core Components

1. **`themes.json`**: The central design token registry containing 10 predefined color templates.
2. **`themeloader.cs`**: Ingests JSON profiles at startup, registers five schemes per theme with `SchemeManager`, and exposes fallback color translation layers for limited hardware emulators.
3. **`program.cs`**: Initializes frames and demonstrates live interaction by switching the active scheme from a `DropDownList`.

---

## Theme Configuration Schema (`themes.json`)

All theme profiles are grouped within a root `"themes"` array using a
configuration blueprint mapping tokens to individual UI widget types.

### Structural Requirements

Every theme block must declare a `name`, a debugging helper group
(`base_ansi`, not read by the loader), and a mandatory `components` object with
exactly five framework targets:

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

## Execution Pipeline & The Fallback Engine

When generating code patches or creating automated framework tests, maintain
the integrity of the multi-tiered rendering pipeline:

1. **Load:** `ThemeLoader.LoadThemes(path)` reads `themes.json` and throws
   `FileNotFoundException` or `InvalidDataException` on a missing or invalid
   file. Call it after the application is initialized.
2. **Register:** each theme registers `{Key}Window`, `{Key}Dialog`, `{Key}Menu`,
   `{Key}Button`, and `{Key}Border` schemes, where the key is the theme name
   without spaces or apostrophes. Views select a theme with `view.SchemeName`.
3. **Parse:** each `"Foreground/Background"` pair becomes a `Scheme` attribute.
   A missing or malformed pair falls back to gray on black.
4. **Downgrade:** a `#RRGGBB` color is used as TrueColor only when `COLORTERM`
   is `truecolor` or `24bit`. Otherwise it snaps to the nearest of the 16 ANSI
   colors by Euclidean RGB distance. Named colors always map directly to ANSI.
5. **Unknown values:** an unrecognized color name or a malformed hex value
   falls back to white.
