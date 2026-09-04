# CUA (Common User Access) TUI Specification for Agentic Code Generation

This specification defines the structural and behavioral requirements for generating CUA-compliant Text User Interfaces (TUIs). Instruct the coding agent to adhere strictly to these rules to ensure standardized, predictable user experiences.

## 1. Global Screen Layout
*   **Menu Bar (Top):** Always visible or easily summoned at row 0. Contains application-level commands.
*   **Status / Key Bar (Bottom):** Fixed at the bottom row. Displays context-sensitive help or global shortcuts (e.g., `F1 Help | F3 Quit | F10 Menu`).
*   **Workspace (Center):** The primary view, which hosts text editors, lists, or floating modal dialogs.

## 2. Keyboard Navigation (Strict Requirements)
Every UI element MUST be fully operable without a mouse.
*   **`Tab` / `Shift+Tab`:** Move focus forward and backward between discrete UI controls (text boxes, buttons, lists, checkboxes).
*   **`Arrow Keys` (Up/Down/Left/Right):** Navigate *within* a focused control (e.g., moving the cursor in a text field, scrolling a listbox, moving between radio buttons in a group).
*   **`Enter`:** Execute the default action of a dialog or click the currently focused button.
*   **`Esc`:** Cancel the current operation, close a menu, or dismiss a dialog box without saving.
*   **`Spacebar`:** Toggle the state of checkboxes (`[ ]` to `[X]`) or select radio buttons (`( )` to `(*)`).

## 3. Standard Global Shortcuts
The agent must bind these keys to their designated actions globally:
*   **`F1`:** Open context-sensitive Help.
*   **`F3` (or `Alt+X` / `Alt+F4`):** Exit the application.
*   **`F5`:** Refresh or redraw the screen.
*   **`F10`:** Move focus to the top Menu Bar.
*   **`Alt+<Letter>`:** Trigger menu items or focus controls with a designated underlined mnemonic letter.

## 4. Menu System Conventions
*   **Standard Ordering:** Menus must follow the standard left-to-right order: `File`, `Edit`, `View`, `[Custom]`, `Help`. `Help` must always be the rightmost item.
*   **Standard Dropdowns:**
    *   **File:** New, Open, Save, Save As..., Exit.
    *   **Edit:** Undo (`Ctrl+Z`), Cut (`Ctrl+X`), Copy (`Ctrl+C`), Paste (`Ctrl+V`). *(Note: Support legacy `Shift+Del`, `Ctrl+Ins`, `Shift+Ins` if possible)*.
*   **Ellipsis Rule:** Any menu item that opens a dialog requiring further user input must end with an ellipsis (e.g., `Save As...`).

## 5. Dialog Boxes and Forms
*   **Visuals:** Floating windows must have borders (usually single or double line-drawing characters) and ideally a drop shadow to indicate elevation over the background.
*   **Controls:** Must include an `[ OK ]` button (commits changes) and a `[ Cancel ]` button (discards changes).
*   **Modality:** Dialogs must trap keyboard focus until dismissed via `Enter` (OK) or `Esc` (Cancel).

## 6. Mouse Integration (Progressive Enhancement)
If the target framework supports mouse events, the agent must implement:
*   **Left Click:** Instantly moves focus to the clicked control and triggers buttons/menus.
*   **Scroll Wheel:** Scrolls the currently hovered (or focused) listbox, text area, or menu.
*   **Outside Click:** Clicking outside a dropdown menu or non-critical popup should dismiss it (similar to `Esc`).

## 7. Framework-Specific Agent Instructions
When prompting the agent for specific languages, require the use of CUA-friendly TUI libraries that provide built-in focus management and layered rendering:
*   **C#:** Mandate `Terminal.Gui` (Gui.cs), as it is explicitly built on CUA paradigms out-of-the-box. (Avoid `Spectre.Console` for heavy, overlapping dialogs, as it is strictly a CLI rendering tool, not a CUA event loop).
*   **Go:** Mandate `tview` or `rivo/tview` forks. (While `Bubbletea` is popular, it requires building complex focus-management from scratch; `tview` provides CUA-like primitives like forms, dropdowns, and frames).
*   **Rust:** Mandate `cursive` for declarative CUA-style layouts, or `ratatui` (requires explicitly instructing the agent to build a custom focus-state manager and key-event router to satisfy CUA).
