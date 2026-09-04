#Guidelines for development of Text based User Interfaces

## Window Layout
- Layouts should be full window, and should be aware of the window bounds.
- Window Resizing Events should be caught, and the UI adjusted accordingly.
  If the window size is too small to display needed information,
  WARN the user in the available space.
- The top of the window should be the application title, and relevant information.
- Below that, context information informing the user about where they are in the application
  and if they are editing information, such as a file, or record, those details.
- The bottom of the window should have a panel of command key options relevant to the current screen.
  See **Keyboard** for those items.
- Pressing a commandkey should cause the displayed command option to flash on screen before proceeding.
- If displaying entry fields, the cursor should start focused in the first entry field.

## Theme
- Color should be used to convey special meaning
- Color should be used to highlight areas in panels
- Be aware of color combinations that can be confusing for the color-blind
- Color themes should not be hard coded. If not including an active theming ability,
  use a color palette section defined in the application config file.


## Keyboard

 - Keyboard usage should be consistent.
 - Arrow keys should be active for navigation through the window.
 - Tab and Shift-Tab should shift focus between input fields, panes, and panels.
 - Enter should confirm an action, select a highlighted item, or execute a command.
   This does NOT apply in a multiline text field. In such a field, Enter should
   result in a New Line. In this case, Ctrl-Enter should confirm an action, etc.
 - Esc should close a popup, cancel an action, or back out of a submenu or screen.
 - Ctrl-C should ask the user if they want to close the application.
   Pressing it a second time should close the application.
 - Ctrl-Q is reserved for XON/XOFF flow control, do not use.
 - F1 should display Help
 - F3 should escape the current panel or subwindow
 - F5 should refresh the screen if relevant, If information is displayed from an API, refetch if relevant.
 - F7 / Page Up / Shift Up Arrow - Scroll up a page
 - F8 / Page Down / Shift Down Arrow - Scroll down a page
 - ? should display a search dialog, if relevant
 - Delete should delete the last character typed, Shift-Delete the last word. Ctrl-D the last sentence.
 - When a checkbox is the active field, the spacebar should toggle the value.
 - All Custom Commands should use Alt-A...Alt-Z
   On Macos, use plain letters but watch for conflicts.
   If developing in Go, check snippets/SNIPPETS.MD for the functions:
    - go_isKittyKeyboardAvailable
    - go_EnableAltKeys
    - go_DisableAltKeys
 - Clipboard Operations: Ctrl-Insert Copy, Shift-Insert Paste, Shift-Delete Cut
 
