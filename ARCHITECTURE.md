# OutSystems 11 MCP Skill Architecture

> **Repository:** outsystems11-mcp
> **Runtime Environment:** Static skill content — loaded as context into a coding agent (Claude Code, GitHub Copilot, Antigravity, Kiro, Codex CLI, Cursor, or any MCP-compatible agent). Not an executable service.
> **Last Updated:** 2026-09-14

## Overview

This repository distributes a single skill (`servicestudio-mcp-oml/`) that teaches a
coding agent how to read and evolve an OutSystems 11 (O11) application model open in Service Studio.
The skill body is harness-neutral; thin per-harness adapters (`AGENTS.md`, `.github/`, and the
committed MCP client configs) carry the same contract into Claude Code, GitHub Copilot,
Antigravity, Kiro, Codex CLI and Cursor — see T6.
The skill is prompt and reference material only — the runtime it drives (Service Studio's embedded
MCP server and the bundled Model API sidecar) lives in a **different codebase and binary that is out
of scope for this repo**.

## Architecture Diagram

The "code" here is not a running process — it is skill content the agent loads. The diagram shows
where that content is consumed and the system boundary it governs. Everything to the right of the
agent is external and not shipped by this repo.

```mermaid
graph TB
    %% This repository
    Skill["outsystems11-mcp<br/>(servicestudio-mcp-oml)<br/>Runs on: loaded as context into coding agent"]

    %% External actors / systems
    Agent["Coding Agent (Claude Code, GitHub Copilot,<br/>Antigravity, Kiro, Codex, Cursor, …)<br/>EXTERNAL"]
    MCP["Service Studio in-process MCP Server<br/>http://127.0.0.1:41820/mcp<br/>EXTERNAL"]
    Sidecar["Bundled Model API sidecar<br/>(spawned per call, inside Service Studio)<br/>EXTERNAL — out of repo scope"]
    OML[("Open .oml module + %TEMP% snapshots<br/>EXTERNAL")]

    %% Flows
    Skill -->|Loaded as context / read as reference<br/>Synchronous| Agent
    Agent -->|MCP tool calls: applyModelApiCode, get*,<br/>omlMerge, omlReset over HTTP<br/>Synchronous| MCP
    MCP -.->|Wraps lambda + spawns per call, stdio JSON-RPC<br/>Synchronous| Sidecar
    Sidecar -->|Snapshot in / save out .oml<br/>Synchronous| OML
    MCP -->|Compare-and-Merge window (omlMerge)<br/>Synchronous| OML

    %% Styling
    classDef thisRepo fill:#80cbc4,stroke:#00796b,stroke-width:3px,color:#000
    classDef external fill:#ef9a9a,stroke:#d32f2f,stroke-width:2px,stroke-dasharray: 5 5,color:#000
    classDef database fill:#ce93d8,stroke:#7b1fa2,stroke-width:2px,color:#000

    class Skill thisRepo
    class Agent,MCP,Sidecar external
    class OML database
```

## External Integrations

The skill itself makes no network calls. The table below lists the touchpoints the skill's contract
directs the **agent** to interact with, and the systems the MCP server engages downstream.

| External Service | Communication Type | Purpose |
| --- | --- | --- |
| Coding Agent (Claude Code, GitHub Copilot, Antigravity, Kiro, Codex CLI, Cursor) | Sync (context load) | Consumes SKILL.md + references, plus the harness adapter it recognizes (if any); emits Model API lambdas and MCP tool calls |
| Service Studio in-process MCP Server (`127.0.0.1:41820/mcp`) | Sync (HTTP, MCP tool calls) | Exposes `applyModelApiCode`, the `get*` reads, `runQuery`, `listApps`, `getValidationMessages`, and host-only `omlReset` / `omlMerge` |
| Bundled Model API sidecar | Sync (stdio JSON-RPC, out of scope) | Runs the C# `eSpace => {...}` lambda against a snapshot using the OutSystems Model API; implementation not in this repo |
| Open `.oml` module + `%TEMP%\ServiceStudio.MCPServer\` snapshots | Sync (file I/O, host-managed) | Input snapshot (`<runId>-in.oml`) and mutated output (`<runId>-out.oml`); surfaced to the user via the Compare-and-Merge window |

> **Scope note.** The MCP server and sidecar internals (e.g. `Host/McpToolNames.cs`, `Tools/*.cs`
> referenced in `reference/verb-reference.md`) live in Service Studio's codebase, not here. This
> document describes them only as far as the skill's own docs describe the contract/interface.

## Architectural Tenets

### T1. Skill content must describe the contract, never implement the runtime

This repository is prompt and reference material for an agent — it contains no server, no build, and
no executable entry point. The MCP server, its host wrap, and the Model API sidecar all live in a
separate Service Studio binary. The skill's job is to teach the agent the *observable contract*
(tool names, argument shapes, response fields, failure modes), not to reimplement or assume
undocumented internals. This keeps the skill portable across MCP-compatible agents and resilient to
host changes it cannot see.

**Evidence:**
- `servicestudio-mcp-oml/SKILL.md` (§ "Do NOT use for" / § 8 "Known gaps") — explicitly scopes the skill to the MCP tool contract and defers server behaviour to the host.
- `servicestudio-mcp-oml/reference/verb-reference.md` — points at `Host/McpToolNames.cs` and `Tools/*.cs` as the host's source of truth, which are *not* present in this repo.
- Repository contains only `.md`, `.cs` examples, `.json`/`.txt` doc artifacts, and `effectiveness.yml` — no project/build files.

### T2. The `code` argument must be a complete `eSpace => { ... }` lambda with no `eSpace.Save(...)`

The host rewrites the submitted lambda: it strips the trailing `}`, appends
`eSpace.Save("<host-allocated-path>")`, and re-closes. Consequently the agent must pass a full
`Action<IESpace>` lambda ending exactly in `}`, use the literal parameter name `eSpace`, and never
call `Save` itself (the host owns the output path). Every worked example ships in exactly this shape
so it can be lifted verbatim into the `code` field. This is the single way the contract diverges
from ODC's published full-lambda-with-Save form.

**Evidence:**
- `servicestudio-mcp-oml/SKILL.md` (§ 1 "The full-lambda contract") — documents the strip-and-append wrap and the four derived rules.
- `servicestudio-mcp-oml/reference/lambda-contract.md` — full wrap mechanics and the `CS0103` rename trap.
- `servicestudio-mcp-oml/examples/` (e.g. `AddServerEntity.cs`) — each file is a full `eSpace => { ... }` lambda with no `eSpace.Save`.

### T3. Always read before writing, and verify after writing — a clean response is not proof of mutation

The agent must call `getDataModel` (or the matching `get*`) before mutating, because the read output
is the same Model API dialect the agent must emit. After mutating, it must read back and scan
`validationMessages` for a `type: "error"` entry: the response shape cannot by itself distinguish a real
change from a **silent no-op** (a lambda that compiles and saves but never touches `eSpace`) or a
**saved-but-invalid** model. An empty `exceptionMessage` only means the injected `Save` ran (code
that does not compile never runs at all and comes back as an MCP tool error, not as a result).

**Evidence:**
- `servicestudio-mcp-oml/SKILL.md` (§ 2 "Reading before writing" and the § 3 outcome table) — read-before-write reflex and the silent-no-op / saved-but-invalid rows.
- `servicestudio-mcp-oml/reference/mcp-session-pointer.md` (Advance rules table) — the two deceptive outcomes where the pointer advances to an undesired save.
- `servicestudio-mcp-oml/effectiveness.yml` (`scenario_2`, `scenario_5`, `scenario_7`) — scores read-before-write, silent-no-op defence, and read-back confirmation.

### T4. OML mutation state is host-owned; the agent passes only `code` + `imports` and never threads file paths

The host keeps a per-module **session pointer** at the last successful save and auto-chains
successive edits through it — the agent never names input or output paths. The pointer advances only
when the out file was written and `exceptionMessage` is empty; failed calls leave it untouched, so
retries resume from the last good state (no `omlReset` needed for error recovery). The closing step
of any mutating task is `omlMerge` with the returned `mutatedOmlPath`, which surfaces the diff into
Service Studio's Compare-and-Merge window and clears that module's pointer, so the next call
re-snapshots the live module. The pointer is cleared when the window opens, not when the user
accepts, so a chain does not continue past a merge.

**Evidence:**
- `servicestudio-mcp-oml/reference/mcp-session-pointer.md` — pointer advance rules, `omlReset`, `omlMerge`, and cross-module eviction.
- `servicestudio-mcp-oml/SKILL.md` (§ 3 and § 3.1) — the agent-facing invocation shape (`code` + optional `imports`) and the auto-merge closing step.
- `servicestudio-mcp-oml/effectiveness.yml` (`scenario_4`) — asserts `omlReset` is *not* the recovery path for a failed apply.

### T5. Reference content is layered by responsibility, with machine facts kept reproducible

The corpus is organized so each layer has one job: `SKILL.md` is the single entry-point contract;
`reference/` holds prose deep-dives (lambda mechanics, session pointer, per-element and Traditional
patterns, verb catalogue); `examples/` holds directly-liftable full lambdas; `integration-examples/`
holds end-to-end transcripts; and `docs/` holds machine-derived Model API signatures. The `docs/`
files are not hand-maintained — they are reflected from the live `OutSystems.Model.V1` assembly, and
`generation/` records the reproducible method. This separation prevents duplication and keeps the
authoritative facts (API signatures) regenerable rather than drifting.

**Evidence:**
- `servicestudio-mcp-oml/SKILL.md` (§ 6 "Where to dig deeper") — maps each topic to exactly one file/folder.
- `servicestudio-mcp-oml/generation/README.md` — documents the reflection-based generation of the `docs/*.Generated.cs` Web files and their verification.
- `servicestudio-mcp-oml/docs/` and `servicestudio-mcp-oml/examples/` — signature reference vs. liftable lambdas, kept distinct.

### T6. Harness adapters restate the contract; `SKILL.md` remains its only source of truth

Claude Code, Antigravity, Kiro, Codex CLI and Cursor all read the `SKILL.md` format natively —
reaching them costs this repo nothing beyond the skill folder itself and the cross-tool `AGENTS.md`,
which Codex and Cursor also read natively with no adapter of their own. Reaching the one harness
that doesn't share either mechanism, GitHub Copilot, means it gets a thin adapter in the shape it
recognizes — `.github/copilot-instructions.md`, a VS Code chat mode, a `/oml-edit` prompt — and
every one of those necessarily **restates a condensed subset of the contract** so it is active
without the agent choosing to open `SKILL.md`. That duplication is deliberate and is the one
sanctioned exception to T5, but it comes with two hard rules.

First, **adapters are derived, never authoritative.** Where an adapter and `SKILL.md` disagree,
`SKILL.md` wins, and the adapter is the bug. Each adapter says so in its own text and points at
`servicestudio-mcp-oml/SKILL.md` for anything it does not cover.

Second, **a contract change is not done until every adapter is updated in the same change.** The
adapters are load-bearing for non-Claude harnesses, so a stale one silently teaches an obsolete
contract to a whole tool. This is why the contract-change checklist in `CONTRIBUTING.md` and
`CLAUDE.md` enumerates them alongside `effectiveness.yml`.

A third constraint follows from the adapters sharing a directory with the skill they describe: a
repo-root instruction file is read both by an agent *using* the skill and by an agent *editing this
repository*, and those need opposite instructions. `AGENTS.md` and `CLAUDE.md` therefore open by
routing the reader to the right one, rather than assuming the audience.

**Evidence:**
- `AGENTS.md` — cross-tool adapter; opens with the use-vs-edit routing block and names `SKILL.md` as the source of truth on conflict.
- `.github/copilot-instructions.md`, `.github/chatmodes/*.chatmode.md`, `.github/prompts/*.prompt.md` — the Copilot-shaped adapters, each deferring to `servicestudio-mcp-oml/SKILL.md` and `reference/*.md`.
- `CLAUDE.md` ("Editing guidance") and `CONTRIBUTING.md` ("Pull Request Process") — both list the adapters in the set of files a contract change must update together.
- `README.md` ("What's in this repository") — the layer table naming which layers have a cross-tool standard and which do not.
- `setup-docs/codex.md` and `setup-docs/cursor.md` — document the skill-folder copy plus native MCP config for the two harnesses that need no restated-contract adapter at all.
