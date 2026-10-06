# Service Studio OML skill → Cursor

Setup for **Cursor**. Cursor reads the same open **Agent Skills** standard as
Claude Code and Codex — `SKILL.md` discovered from `.cursor/skills/<name>/`
or the shared `.agents/skills/<name>/` path — so the skill ports verbatim,
with no adapter markdown needed. As always, it's the **same** Service Studio
MCP server, so tool names and the lambda contract are identical.

## Two ways to give Cursor the skill

| Mechanism | Scope | Source in this package | Best for |
| --- | --- | --- | --- |
| **Skill** (recommended) | Project (`.cursor/skills/` or `.agents/skills/`) | `servicestudio-mcp-oml/` (has `SKILL.md`) | The truest 1:1 with the Claude skill — auto-discovered, progressive reference reads |
| **AGENTS.md** | Per project (repo root) | `../AGENTS.md` | Teams who prefer project-scoped, always-on instructions |

You can use either or both. If your project also targets Codex, prefer
`.agents/skills/` — both tools discover the same folder.

## Prerequisites

1. **Cursor** installed and signed in.
2. **Service Studio (OutSystems 11)** running, with the target module
   **open**, and its in-process MCP server **started** — it does not start
   automatically: in Service Studio, go to **Edit > MCP Configuration...**
   and click **Start MCP Server** (default port `41820`). Approve the
   connection the first time.

## Step 1 — install the skill

Cursor discovers skills from either of these project-scoped paths (run from
the repo root):

- **`.cursor/skills/` (Cursor-specific):**
```
  mkdir -p .cursor/skills
  cp -R servicestudio-mcp-oml .cursor/skills/servicestudio-mcp-oml
```
- **`.agents/skills/` (shared with Codex):**
```
  mkdir -p .agents/skills
  cp -R servicestudio-mcp-oml .agents/skills/servicestudio-mcp-oml
```

**Windows (PowerShell)**, either path:
```
New-Item -ItemType Directory -Force ".cursor\skills" | Out-Null
Copy-Item -Recurse servicestudio-mcp-oml ".cursor\skills\servicestudio-mcp-oml"
```

The folder name (`servicestudio-mcp-oml`) becomes the skill's identifier and
must match the `name` in the `SKILL.md` frontmatter.

> **This repo does not commit a copy under `.cursor/skills/` or
> `.agents/skills/`.** The canonical content lives in
> `servicestudio-mcp-oml/`; copy it as above rather than maintaining two
> locations.

**Skills are discovered when a session starts**, so **start a new chat**
after copying — adding the folder mid-session will not pick it up.

**Verify:** ask the agent what skills it has available and confirm
`servicestudio-mcp-oml` is listed.

### Alternative: AGENTS.md (project-scoped)

If you'd rather keep it per-project, drop the package's `AGENTS.md` in your
project root along with the `servicestudio-mcp-oml/` folder. Cursor reads
`AGENTS.md` from the workspace root automatically. (This is the same file
the Copilot CLI, Antigravity, Kiro and Codex read, so a single `AGENTS.md`
serves all of them.) Cursor's own richer `.cursor/rules/*.mdc` mechanism is
unaffected — this repo doesn't use it, so there's nothing to reconcile.

## Step 2 — add the MCP server

**This repo already ships the project file** (`.cursor/mcp.json`) — if you
open the repo root in Cursor there is nothing to install. For any other
project, add or merge the same entry into `.cursor/mcp.json` (project) or
`~/.cursor/mcp.json` (global):

```json
{
  "mcpServers": {
    "servicestudio": {
      "url": "http://127.0.0.1:41820/mcp"
    }
  }
}
```

Notes on the format:
- HTTP servers use **`url`** — matches Kiro's shape exactly, unlike
  Antigravity's `serverUrl` or the Copilot CLI's extra `tools: ["*"]` field.
- **No `type` field** needed.

**Verify:** open Cursor's MCP settings and confirm `servicestudio` shows
connected, exposing `applyModelApiCode`, `getDataModel`, `omlMerge`, and the
other verbs.

## Using it

Describe the change in natural language; the skill's contract steers the
agent:

```
> Add a DueDate Date attribute to the Task entity in the open module.
```

Expected flow: read the model (`getDataModel`) → `applyModelApiCode` lambda →
verify the response → `omlMerge` so the diff opens in Service Studio.

## Verify end-to-end

1. Open a module in Service Studio.
2. Open the repo root in Cursor; start a new chat and confirm the skill is
   loaded and the MCP server is connected.
3. Ask: *"List the entities in the open module."* → expect a `getDataModel`
   call.
4. Ask for a trivial change → confirm a non-empty `mutatedOmlPath` and a
   follow-up `omlMerge` (Compare-and-Merge window opens in Service Studio).

## Troubleshooting

| Symptom | Cause / fix |
| --- | --- |
| Skill not picked up | Wrong nesting (must be `<scope-root>/skills/<name>/SKILL.md`, not a loose `.md`); the folder name doesn't match the `name` in the frontmatter; or you didn't start a **new** chat after copying it. |
| MCP server won't connect | Service Studio not running / no module open, or a field-name error — it must be `url`, not `serverUrl`. Approve the connection in Service Studio. |
| MCP server not found at all | Cursor was opened at a parent or child directory instead of the repo root, so `.cursor/mcp.json` wasn't picked up. Open the repo root directly, or add the entry to `~/.cursor/mcp.json`. |
| Connection refused on `41820` specifically | Service Studio binds `41820` by default, but if that port was already taken it may be listening on a nearby one. Confirm what it actually bound — `netstat -ano \| findstr LISTENING` (Windows) or `lsof -nP -iTCP -sTCP:LISTEN` (macOS / Linux) — and put that port in the config. MCP clients need a literal URL, so there is no auto-discovery. |

