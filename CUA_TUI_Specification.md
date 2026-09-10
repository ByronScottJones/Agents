# CUA (Common User Access) TUI Specification for Agentic Code Generation

This specification defines the structural and behavioral requirements for generating CUA-compliant Text User Interfaces (TUIs). Instruct the coding agent to adhere strictly to these rules to ensure standardized, predictable user experiences.

Apply this specification only when the task explicitly requires CUA compliance.
When it applies, these rules override conflicting general TUI guidance.

## 1. Global Screen Layout

The implementation must define a minimum usable terminal size and show a clear
warning when the current terminal is smaller than that size.

* **Title Bar:** Row 0 - Displays Application Name, Current Document, or other relevant context.
* **Menu Bar (Top):** Row 1 - Contains application-level commands. See Menu System Conventions.
* **Workspace (Center):** The primary view, which hosts text editors, lists, or floating modal dialogs.
* **Status / Key Bar (Bottom):** Fixed at the bottom. Displays context-sensitive help or global shortcuts (e.g., `F1 Help | F3 Quit | F10 Menu`). If the window width would cause truncation, use additional rows. When the main workspace has a multiline text editor, the Status Bar should include the cursor `Row` and `Column`.

## 2. Keyboard Navigation (Strict Requirements)

Every UI element MUST be fully operable without a mouse.

* **`Tab` / `Shift+Tab`:** Move focus forward and backward between discrete UI controls (text boxes, buttons, lists, checkboxes). The item with current focus should be highlighted, unless it has a cursor.
* **`Arrow Keys` (Up/Down/Left/Right):** Navigate *within* a focused control (e.g., moving the cursor in a text field, scrolling a listbox, moving between radio buttons in a group).
* **`Enter`:** Execute the default action of a dialog or click the currently focused button.
* **`Esc`:** Cancel the current operation, close a menu, or dismiss a dialog box without saving.
* **`Spacebar`:** Toggle the state of checkboxes (`☐` to `☑`) or select radio buttons (`◯` to `◉`).

## 3. Standard Global Shortcuts

* **MacOS Limitation:** MacOs captures the Option Key, and does not always translate it to Alt.
  - The default terminal has a per-profile configuration flag `useOptionAsMetaKey`, and terminals such as Kitty support Option/Alt translation.
  - Test for Option/Alt translation at startup, and offer alternative CTRL based key bindings if Option/Alt translation cannot be confirmed. Check snippets/SNIPPETS.md for example code.
 
The agent must bind these keys to their designated actions globally:

* **`F1`:** Open context-sensitive Help.
* **`F3` (or `Alt+X` / `Alt+F4`):** Exit the application.
* **`F5`:** Refresh or redraw the screen.
* **`F10`:** Move focus to the top Menu Bar.
* **`Alt+<Letter>`:** Trigger menu items or focus controls with a designated underlined mnemonic letter.

## 4. Menu System Conventions

- Follow the Menu conventions, but do not include menu items that make no sense for the application.
* **Standard Ordering:** Menus should follow the standard left-to-right order: `[AppName]`, `File`, `Edit`, `View`, `[Custom]`, `Help`. `Help` must always be the rightmost item.
* **Standard Dropdowns:**
  * **File:** New, Open, Save, Save As..., Exit.
  * **Edit:** Undo (`Ctrl+Z`), Cut (`Ctrl+X`), Copy (`Ctrl+C`), Paste (`Ctrl+V`). *(Note: Support legacy `Shift+Del`, `Ctrl+Ins`, `Shift+Ins` if possible)*.
* **Ellipsis Rule:** Any menu item that opens a dialog requiring further user input must end with an ellipsis (e.g., `Save As...`).

## 5. Dialog Boxes and Forms

* **Visuals:** Floating windows must have borders (usually single or double line-drawing characters) and ideally a drop shadow to indicate elevation over the background.
* **Controls:** Must include an `[ OK ]` button (commits changes) and a `[ Cancel ]` button (discards changes).
* **Modality:** Dialogs must trap keyboard focus until dismissed via `Enter` (OK) or `Esc` (Cancel).
* Restore focus to the control that opened the dialog after the dialog closes.

## 6. Mouse Integration (Progressive Enhancement)

If the target framework supports mouse events, the agent must implement:

* **Left Click:** Instantly moves focus to the clicked control and triggers buttons/menus.
* **Scroll Wheel:** Scrolls the currently hovered (or focused) listbox, text area, or menu.
* **Outside Click:** Clicking outside a dropdown menu or non-critical popup should dismiss it (similar to `Esc`).

## 7. Framework-Specific Agent Instructions

When prompting the agent for specific languages, require the use of CUA-friendly TUI libraries that provide built-in focus management and layered rendering:

* **C#:** Prefer `Terminal.Gui` (Gui.cs). Use another library when project constraints require it, but preserve the CUA behavior defined here.
* **Go:** Prefer `tview` or `rivo/tview` forks. If using Bubble Tea, implement explicit focus management and key routing.
* **Rust:** Prefer `cursive` or `ratatui`; with `ratatui`, implement an explicit focus-state manager and key-event router.
