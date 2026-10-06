# CLAUDE.md

This repository distributes a single skill, `servicestudio-mcp-oml/`, for the OutSystems 11 MCP, packaged for Claude Code (the optimized path) plus GitHub Copilot, Antigravity, Kiro, Codex CLI and Cursor. The skill teaches an agent to read and mutate an OutSystems 11 module open in Service Studio via the embedded MCP server's `applyModelApiCode` tool. **This repo is skill content (Markdown, C# example lambdas, generated reference files) — there is no build system, no runtime, and no executable entry point.**

Note the distinction: working **on** this repo means editing the skill content. Using the skill (driving a live OutSystems module) is what the content teaches an *end user's* agent to do — that is not this task.

> If you are in fact here to drive a live OutSystems 11 module rather than change this repo, stop and read [AGENTS.md](./AGENTS.md) and `servicestudio-mcp-oml/SKILL.md` instead. AGENTS.md is written for that job and is loaded automatically alongside this file, so take care not to act on its contract while editing repo content.

## Foundation documents

- [ARCHITECTURE.md](./ARCHITECTURE.md) — the system boundary (skill content vs. the out-of-repo MCP server + Model API sidecar) and the six tenets (T1–T6) governing what the skill may claim. Read T2 (the no-`Save` full-lambda contract), T5 (reference layering) and T6 (harness adapters are derived, never authoritative) before editing contract-bearing content.
- [CONTRIBUTING.md](./CONTRIBUTING.md) — the human dev workflow (no build; test via manual reinstall + restart, or the `effectiveness.yml` koda eval), per-folder content standards, and the command quick-reference.

## Editing guidance

`servicestudio-mcp-oml/SKILL.md` is the authoritative contract and the only file guaranteed to be read in full by a consuming agent. Everything else is supporting material layered by responsibility (see ARCHITECTURE.md T5): `reference/` (prose deep-dives), `examples/` (liftable full lambdas), `integration-examples/` (wire transcripts), `docs/` (generated API signatures), `generation/` (how `docs/` is produced).

- **Do not hand-edit `docs/*.Generated.cs` or `docs/BuiltinFunctions.Generated.json`** — they are reflected from the live `OutSystems.Model.V1` assembly. Regenerate via `servicestudio-mcp-oml/generation/README.md`.
- A contract change is never a one-file edit: keep SKILL.md, the relevant `reference/` file(s), any affected `integration-examples/` transcript, the matching `effectiveness.yml` `scenario_*` points, **and the harness adapters that restate the contract** (`AGENTS.md`, `.github/copilot-instructions.md`, `.github/chatmodes/*.chatmode.md`, `.github/prompts/*.prompt.md`) in sync in the same change (see CONTRIBUTING.md "Development Workflow" and "Pull Request Process").
- Tool names are exact host camelCase (`applyModelApiCode`, `getDataModel`, `omlMerge`, …). The old `oml_`-prefixed snake_case names no longer exist — never reintroduce them.
