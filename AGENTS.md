# Global Rules

These rules apply to **every** session, regardless of task type.

## Conditional Guidance

Open these files only when they apply — don't load them into every session:

| When | Open this file |
|---|---|
| Session involves source files, scripts, or config (any coding task) | [`coding.md`](coding.md) |
| Task involves automating a running macOS GUI app | [`gui_automation.md`](gui_automation.md) |
| Tasks involving BASH and ZSH Shell Files | [`shell.md`](shell.md) |

---

## General Behavior

- Be concise and direct. Prefer shorter responses when the answer is clear.
- Don't add explanatory preamble before code ("Here is a function that..."). Just write the code.
- Don't apologize or over-explain errors. Diagnose and fix.
- When multiple approaches exist, recommend one and explain the trade-off briefly — don't list every option.
- Never add `TODO` comments unless explicitly asked.
- Never generate placeholder logic (e.g., `// implement later`) — implement it or say you can't.
- Prefer editing existing code over rewriting files wholesale.
- When unsure about intent, ask one targeted question rather than guessing.

---

## Tool Calling

- When calling command line tools, add a comment at the end of the command line explaining the purpose of the command.
  When calling multi-line scripts, including custom executables that you have created, each script should have detailed comments,
  including a comment block at the beginning explaining the purpose of the script, and each line or group of commands should have a comment.
