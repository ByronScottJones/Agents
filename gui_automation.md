# GUI Automation (UICTL)

For tasks involving a running GUI application on macOS, Windows, or Linux —
finding a window, taking screenshots, reading the accessibility tree, clicking,
typing, or waiting for an element — use the UICTL MCP implementation for the
current operating system:

| Operating system | UICTL implementation | Additional guidance |
| --- | --- | --- |
| macOS | `uictl-mac-mcp` | Read `~/dev/uictl-mac-mcp/AGENTS.md` on demand. |
| Windows | `uictl-win-mcp` | Read the corresponding `uictl-win-mcp/AGENTS.md` on demand. |
| Linux | `uictl-linux-mcp` | Read the corresponding `uictl-linux-mcp/AGENTS.md` on demand. |

Use the implementation's supported UICTL MCP tools, such as
`uictl_screenshot`, `uictl_elements`, `uictl_click`, `uictl_type`, and
`uictl_wait_for`, where available. Prefer UICTL over raw OS-specific automation
commands or screenshot-and-guess workflows. Do not use one operating system's
implementation for another operating system.

## Safety and Evidence

- Identify the target application and window before clicking or typing.
- Confirm destructive actions before performing them unless the user explicitly
  authorized that exact action.
- Never enter passwords, API keys, tokens, or other secrets through automation.
- Use bounded waits and retries; do not loop indefinitely.
- Capture accessibility or screenshot evidence after important actions when the
  result cannot otherwise be verified.
- If the UICTL server for the current operating system is unavailable, stop and
  report the limitation rather than switching to an unapproved automation
  method.
