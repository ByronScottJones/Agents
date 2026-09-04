# GUI Automation (uictl)

For tasks involving a running macOS GUI app — finding a window, screenshotting
it, reading its accessibility tree, clicking, typing, waiting for an element —
use the `uictl` MCP tools (`uictl_screenshot`, `uictl_elements`, `uictl_click`,
`uictl_type`, `uictl_wait_for`, etc.), registered globally via `claude mcp add
uictl --scope user`. Prefer these over raw `osascript`/`cliclick`/
screenshot-and-guess workflows. Full usage patterns and gotchas: read
`~/dev/uictl-mac-mcp/AGENTS.md` on demand when doing this kind of work.
