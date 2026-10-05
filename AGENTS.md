# Agent Guidance

This file is always active. Apply its rules to every task, then load only the
topic files required by the task. Do not load every topic file by default.

## Topic Routing

Load the matching file before making decisions or edits in that topic:

| Topic | File to load |
| --- | --- |
| Git repositories, branches, commits, or pull requests | [`git.md`](git.md) |
| Source code, scripts, or general coding practice | [`coding.md`](coding.md) |
| Markdown files or documentation authoring | [`markdown.md`](markdown.md) |
| Security engineering or supply-chain security | [`security.md`](security.md) |
| Test strategy or test implementation | [`testing.md`](testing.md) |
| Container images or container runtime | [`containers.md`](containers.md) |
| Logs, metrics, traces, or service health | [`observability.md`](observability.md) |
| Releases, deployments, or change promotion | [`release.md`](release.md) |
| Production incidents or operational response | [`incident-response.md`](incident-response.md) |
| Software architecture or design decisions | [`architecture.md`](architecture.md) |
| Dependency lifecycle or software supply chain | [`dependencies.md`](dependencies.md) |
| API design or service contracts | [`api.md`](api.md) |
| Databases, schemas, or data migrations | [`database.md`](database.md) |
| Performance or capacity engineering | [`performance.md`](performance.md) |
| Kubernetes workloads | [`kubernetes.md`](kubernetes.md) |
| Cloud architecture or cloud operations | [`cloud.md`](cloud.md) |
| Disaster recovery or business continuity | [`disaster-recovery.md`](disaster-recovery.md) |
| Cloud cost management or FinOps | [`cost-management.md`](cost-management.md) |
| Team engineering workflow | [`engineering-workflow.md`](engineering-workflow.md) |
| Compliance, risk, or audit controls | [`risk-and-compliance.md`](risk-and-compliance.md) |
| Service ownership or operational documentation | [`service-ownership.md`](service-ownership.md) |
| Accessibility for user-facing software | [`accessibility.md`](accessibility.md) |
| Application configuration, settings, or secrets | [`configuration.md`](configuration.md) |
| CMake build-system selection | [`cmake_make.md`](cmake_make.md) |
| GNU Makefiles and task runners | [`makefile.md`](makefile.md) |
| GitHub Actions workflows | [`githubactions.md`](githubactions.md) |
| Go | [`go.md`](go.md) |
| Bash, Zsh, or shell scripts | [`shell.md`](shell.md) |
| PowerShell | [`powershell.md`](powershell.md) |
| C# or .NET | [`dotnet.md`](dotnet.md) |
| JavaScript or TypeScript | [`typescript.md`](typescript.md) |
| HTML | [`html.md`](html.md) |
| Rust | [`rust.md`](rust.md) |
| Terraform | [`terraform.md`](terraform.md) |
| Tauri applications | [`tauri.md`](tauri.md) |
| Delegating work to local LM Studio models, or the `lmstudio-rs-mcp` server | [`lmstudio-subagents.md`](lmstudio-subagents.md) |
| Terminal user interfaces | [`tui.md`](tui.md) |
| Common User Access TUI specification | [`CUA_TUI_Specification.md`](CUA_TUI_Specification.md) |
| Twelve-Factor application design | [`twelvefactors.md`](twelvefactors.md) |
| GUI automation for macOS, Windows, or Linux | [`gui_automation.md`](gui_automation.md) |
| Code snippets | [`snippets/SNIPPETS.md`](snippets/SNIPPETS.md) |

Load more than one file when a task spans topics. For example, a Tauri
application with a Rust backend and Terraform deployment requires
`tauri.md`, `rust.md`, and `terraform.md`.

Language files supplement this file; they do not replace the rules below.

## Guidance Precedence

Apply this file to every task. Load every matching topic file before making
decisions or edits. More specific topic guidance supplements general guidance;
when two files conflict, the more specific file wins unless this file states
otherwise.

Load `CUA_TUI_Specification.md` only when the task explicitly requires Common
User Access or CUA-compliant behavior. Otherwise, use `tui.md` for terminal UI
guidance.

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
