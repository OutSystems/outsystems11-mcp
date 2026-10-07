# Contributing to the OutSystems 11 MCP

This repository distributes a single skill, [`servicestudio-mcp-oml/`](servicestudio-mcp-oml/),
for the OutSystems 11 MCP — packaged for Claude Code (the
optimized path) plus GitHub Copilot via a thin adapter, and Antigravity, Kiro, Codex CLI
and Cursor via their native `SKILL.md`/`AGENTS.md` support. The skill is **content, not
code** — Markdown, C# example snippets, and generated reference files that teach
a coding agent how to drive Service Studio's in-process MCP Server. There is no
compiler, no package manager, and no CI pipeline in this repo (the MCP server
itself ships inside Service Studio 11.55.91 and later, and is built elsewhere).

For the MCP contract, the `applyModelApiCode` lambda shape, and the Model API
surface, see `ARCHITECTURE.md` and `servicestudio-mcp-oml/SKILL.md`.
This document covers the human workflow of editing the skill.

## Development Setup

You need the same environment an end user has (see [README.md](README.md) "Getting
started"): Service Studio 11.55.91 or later installed and running with a module open,
the `servicestudio` MCP server registered, and a Claude Code session. That live
setup is the only way to exercise the skill against a real target. If you are
changing a non-Claude adapter, exercise it in that harness too — the per-tool
setup is in [`setup-docs/`](setup-docs/).

There is nothing to install to *edit* the skill — clone the repo and open it in
your editor. The skill runs from wherever Claude Code loads it (`~/.claude/skills/`
or `%USERPROFILE%\.claude\skills\`), so editing the repo copy alone does not change
what a running agent sees.

## Development Workflow

"Testing a change" here means one of two things — there is no unit-test runner.

1. **Manual end-to-end (primary).** Run the installer script from
   [`scripts/`](scripts/) against your local checkout — `bash scripts/install.sh
   --source . `(or` powershell -ExecutionPolicy Bypass -File scripts\install.ps1
   -Source . `on Windows) — to copy your edited` servicestudio-mcp-oml/` into
   your Claude Code skills directory. Start a **new** Claude Code session
   (skills load at session start) and drive a real task against a module open
   in Service Studio. Because the skill only loads at session
   start, restart the session after every reinstall.

2. **Effectiveness eval (when available).** `servicestudio-mcp-oml/effectiveness.yml`
   is a koda skill-effectiveness eval. It measures whether SKILL.md actually
   teaches the contract (full lambda, no `eSpace.Save`, the response shape,
   read-before-write) versus a baseline model with no skill loaded. It runs
   through the external `skill-forge` plugin, not from this repo:

```
   /skill-forge:run-eval skills/servicestudio-mcp-oml/SKILL.md
```

   The header of `effectiveness.yml` documents the direct `python` invocation
   alternative. This eval requires the `skill-forge` plugin to be installed; it is
   not vendored here and there is no runner script in this repository.

**Keep `effectiveness.yml` in sync with the contract.** Its `scenario_*` blocks
assert the specific behaviour SKILL.md teaches (tool names, `includeJson: "Never"`,
the `mutatedOmlPath` signal, the silent-no-op defence). If you change the contract
in SKILL.md or the reference docs, update the matching scenario points.

## Content Standards

**SKILL.md** is the entry point and the only file guaranteed to be read in full.

- The YAML frontmatter `name` and `description` drive skill activation — the
  `description` is what makes Claude load the skill, so it must enumerate the real
  trigger phrases and tool names. Edit it deliberately.
- **Mind Kiro's 1024-character cap on `description`.** Claude Code has no equivalent
  limit, so this is the binding constraint on the trigger list, and the current
  description is 996 characters, only 28 under it. If you add trigger phrases, measure
  the folded length — Kiro will reject the skill before any other harness complains.
  `name` must also stay lowercase-with-hyphens and match the folder name (Kiro
  enforces this; Claude Code does not).
- **Tool names are exact.** Use the host's camelCase names verbatim
  (`applyModelApiCode`, `getDataModel`, `omlMerge`, …). The old `oml_`-prefixed
  snake_case names no longer exist — do not reintroduce them.
- Keep SKILL.md a **map, not an encyclopedia.** Deep detail lives in `reference/`;
  SKILL.md links to it (§ 6 "Where to dig deeper"). Do not duplicate a reference
  file's body into SKILL.md.
- Maintain the `## Contents` table of contents when you add or rename a section.

**`reference/`** — one Markdown file per deep-dive topic (lambda contract, session
pointer, per-element patterns, Traditional Web, verb reference, tips). When you add
one, link it from the SKILL.md § 6 table. Cross-reference sibling files with
relative links rather than restating them.

**`examples/`** — each `*.cs` file is a complete `eSpace => { ... }` lambda, usable
verbatim as the `code` argument. Follow the existing convention exactly:
- A one-line `//` comment describing the example at the top.
- Leading `using` directives (these map to the `imports` array — see § 4 of SKILL.md).
- A full `eSpace => { ... }` body with **no `eSpace.Save(...)`** call.
- PascalCase, verb-first filenames grouped by intent (`Add*`, `Change*`).

**`integration-examples/`** — numbered (`NN-...md`) end-to-end transcripts of an
agent driving the MCP tools. Keep the wire shapes (request/response JSON) accurate
to the current contract; these are the most contract-sensitive files after
`effectiveness.yml`. Link new ones from SKILL.md § 7.

**`docs/*.Generated.cs`** — do **not** hand-edit. These are reflected from the live
`OutSystems.Model.V1` assembly. To regenerate, follow
`generation/README.md`: paste
`generation/generate-web-docs.cs` into `applyModelApiCode` (wrapped as a full
lambda, no save) with a module open. `BuiltinFunctions.Generated.json` is the
OutSystems expression-function catalogue, likewise generated.

**`AGENTS.md` and `.github/` (adapters)** — `AGENTS.md` is the cross-tool
instruction file; `.github/copilot-instructions.md`, `.github/chatmodes/` and
`.github/prompts/` are the Copilot-shaped equivalents. Each restates a condensed
subset of the contract so it is active without the agent opening `SKILL.md`. Treat
them as **derived**: never let one become the place a rule is defined, always have
them defer to `servicestudio-mcp-oml/SKILL.md`, and update all of them whenever the
contract moves (ARCHITECTURE.md T6). Keep them short — the temptation to grow an
adapter into a second SKILL.md is the failure mode to avoid.

Remember that a repo-root instruction file is read both by an agent *using* the
skill and by an agent *editing this repository*. `AGENTS.md` and `CLAUDE.md` each
open by routing the reader to the right file; preserve that routing when editing
either.

**`setup-docs/`** — one Markdown file per harness, written for a human doing setup
(prerequisites, where that client reads its instructions, how to install the skill,
how to register the MCP server, how to verify). They carry the full manual steps:
`scripts/install.sh` / `install.ps1` install for **Claude Code only**, so the other
harnesses are a manual copy and these guides are the primary path, not a footnote.
Named `setup-docs/` rather than `docs/` so it does not collide with
`servicestudio-mcp-oml/docs/` (generated Model API signatures).

**`mcp/`** — MCP client config samples for the harnesses that read config from the
user's **home directory** (Copilot CLI, Antigravity, Codex CLI) and therefore cannot be
configured by a committed file. Claude Code (`.mcp.json`), Copilot in VS Code
(`.vscode/mcp.json`), Kiro (`.kiro/settings/mcp.json`) and Cursor (`.cursor/mcp.json`)
are configured in-repo instead — Kiro and Cursor need no `mcp/` sample for that reason,
though their guides document the global `~/.kiro/settings/mcp.json` / `~/.cursor/mcp.json`
paths for other workspaces. These seven files describe the same server in as many
shapes — mind the `servers` vs `mcpServers` key, Antigravity's `serverUrl` instead of
`url`, Kiro's and Cursor's `url` with **no** `type` field at all, the Copilot CLI's extra
`tools: ["*"]`, and Codex's TOML `[mcp_servers.<name>]` table (the only non-JSON one). If
you change the endpoint, change all seven.

**The port is part of the endpoint.** All seven configs hardcode `41820`, and so do the
setup guides, `AGENTS.md`, `ARCHITECTURE.md`, `README.md` and
`servicestudio-mcp-oml/reference/verb-reference.md`. That is not laziness: no MCP client supports discovering a server's port,
so each one needs a literal URL. It does mean a port change is a repo-wide edit, not a
seven-file one. If Service Studio gains a configurable port or a fallback that walks past
an occupied `41820`, treat the default as *documentation* rather than a guarantee: sweep
`grep -rn 41820`, and keep the per-guide "connection refused on `41820`" troubleshooting
rows pointing at whatever the discovery story turns out to be.

## Pull Request Process

The repository is early — no branch-naming or commit-message convention is
established yet, and `.github/` holds Copilot configuration rather than PR
templates or workflows (there is no CI). Use the standard flow: fork off `main`,
open a PR against `main`, and get it reviewed before merge; CODEOWNERS requests the
`rd-o11-frontend-runtime` team automatically. Keep changes scoped to the skill content and avoid committing
anything environment-specific (the `.gitignore` covers Visual Studio noise, not
skill files).

When your change alters the MCP/Model API contract, update the affected artifacts
together in the same PR:

- `SKILL.md` and the relevant `reference/` file(s);
- any `integration-examples/` transcript that shows the changed wire shape;
- the matching `effectiveness.yml` scenario points;
- **every harness adapter that restates the contract** — `AGENTS.md`,
  `.github/copilot-instructions.md`,
  `.github/chatmodes/*.chatmode.md`, `.github/prompts/*.prompt.md`. These are
  load-bearing for the non-Claude harnesses (they are what is loaded when nothing
  opens `SKILL.md`), so a stale adapter silently teaches an obsolete contract to a
  whole tool. See ARCHITECTURE.md T6.

## Useful Commands

| Command | Description |
| --- | --- |
| `claude mcp add --transport http servicestudio` http://127.0.0.1:41820/mcp` | Register the Service Studio MCP server (one-time, per README) |
| `/mcp reconnect servicestudio` | Reconnect the agent to Service Studio if the connection drops |
| `/skill-forge:run-eval skills/servicestudio-mcp-oml/SKILL.md` | Run the effectiveness eval (requires the external `skill-forge` plugin) |
