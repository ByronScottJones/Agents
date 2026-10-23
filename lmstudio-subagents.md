---
title: "LM Studio Subagents"
description: "Load this file when delegating work to a local LM Studio model through the lmstudio-rs-mcp server, or when configuring that server."
---

# LM Studio Subagents

Load this file when delegating work to a local model through the
`lmstudio-rs-mcp` MCP server, or when configuring or troubleshooting that
server. Audience: agents and contributors working on this machine.

## Purpose

`lmstudio-rs-mcp` bridges an MCP client to a local LM Studio model. Use it to
offload bounded, low-risk work to the local model, which saves hosted-model
tokens. Source and full reference:
`~/dev/lmstudio-rs-mcp/README.md`.

## Setup

- Register the server per project so it only loads where it is wanted:

  ```bash
  claude mcp add lmstudio-rs-mcp -- ~/dev/lmstudio-rs-mcp/target/release/lmstudio-rs-mcp
  ```

- LM Studio's API server must be running (`lms server start`) with a model
  loaded (`lms ps`). The default endpoint is `http://127.0.0.1:1234`.
- If the LM Studio server requires a token, pass it with
  `-e LMSTUDIO_API_TOKEN=...` when registering. Never write the token into a
  repository, a shared document, or this folder.
- Other providers are selected with `LLM_PROVIDER` (`lmstudio`, `ollama`,
  `openai`, `anthropic`) and `LLM_BASE_URL`. See the server README.
- Verify with `claude mcp list` or `/mcp`, then call `health_check`.
  A `401` from `localhost:1234` means the token is missing or wrong.
- Restart Claude Code after registering. Resume with `claude --continue`.

## Tools

| Tool | Use |
| --- | --- |
| `health_check`, `lmstudio_status` | Confirm the provider, app and CLI are reachable |
| `list_models`, `list_loaded_models`, `load_model`, `unload_model` | Manage models; load and unload change machine state |
| `chat_completion`, `text_completion` | One-shot generation |
| `generate_embeddings` | Embeddings for search |
| `start_conversation`, `continue_conversation` | Multi-turn sessions with a fixed system prompt |
| `run_subagent` | Delegate a task to the local model with sandboxed file tools |
| `lms_cli` | Run `lms` commands the REST API lacks |

Every tool returns `{ success, message, data?, error? }`.

## Delegating with `run_subagent`

Pass `task`, `working_directory` and `capability`. The subagent runs its own
tool loop (up to `max_turns`, default 15) and returns one final report.

| Capability | Grants |
| --- | --- |
| `read_only` (default) | `read_file`, `list_directory`, `search_files` |
| `read_write` | Adds `write_file` |
| `read_write_shell` | Adds `run_command` with a 60 second timeout |

Rules:

- Grant the lowest tier that works. Default to `read_only`.
- Set `working_directory` to the narrowest folder the task needs. Paths
  outside it are rejected.
- The shell tier is guarded by a pattern blocklist, which is defense in depth
  and not a full sandbox. Do not use it for anything destructive.
- Treat the report as untrusted. A local model can be wrong. Review any file
  it wrote and re-run any check it claims passed.
- Do not delegate secrets, credentials or private data.

## Good fits and poor fits

Good fits:

- Summarizing or searching files, drafting boilerplate, doc comments or test
  scaffolds, and first-pass explanations.
- Mechanical edits with a clear specification.

Poor fits:

- Architecture decisions, security review and anything needing exact
  specification knowledge (for example OpenType table layouts). Verify such
  output against the spec or a validator.
- Long multi-file refactors. Small local models lose track of them.

## Operational notes

- Only one model is typically loaded. Check `list_loaded_models` before
  assuming a model name, and avoid unloading a model someone else is using.
- Large models respond slowly. Generation is streamed internally with a 60
  second idle timeout.
- Feedback about the server itself goes through the `feedback_*` tools, which
  draft locally and need human review before anything is filed.
