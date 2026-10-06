# Service Studio OML skill → Codex CLI

Setup for OpenAI's **Codex CLI**. Codex has the cleanest fit of the targets in
this repo: it reads the open **Agent Skills** standard from
`.agents/skills/<name>/SKILL.md` — the exact `SKILL.md` folder format this
skill already ships in — so it ports verbatim, with no adapter markdown
needed. As always, it's the **same** Service Studio MCP server, so tool names
and the lambda contract are identical.

## Two ways to give Codex the skill

| Mechanism | Scope | Source in this package | Best for |
| --- | --- | --- | --- |
| **Skill** (recommended) | Project (`.agents/skills/`) or global (`~/.codex/skills/`) | `servicestudio-mcp-oml/` (has `SKILL.md`) | The truest 1:1 with the Claude skill — auto-discovered, progressive reference reads |
| **AGENTS.md** | Per project (repo root) | `../AGENTS.md` | Teams who prefer project-scoped, always-on instructions |

You can use either or both. Codex reads `AGENTS.md` automatically regardless
of whether a skill is installed, so keep it thin and let the skill carry the
depth.

## Prerequisites

1. **Codex CLI** installed and signed in — see OpenAI's current install docs
   for your platform.
2. **Service Studio (OutSystems 11)** running, with the target module
   **open**, and its in-process MCP server **started** — it does not start
   automatically: in Service Studio, go to **Edit > MCP Configuration...**
   and click **Start MCP Server** (default port `41820`). Approve the
   connection the first time.

## Step 1 — install the skill

Codex discovers skills from `.agents/skills/<name>/SKILL.md`, scanning the
current directory and its parents up to the repo root (project scope), or
from `~/.codex/skills/<name>/SKILL.md` (global scope, shared across
projects). Copy the skill folder into it, naming the destination after the
skill (run these from the repo root):

- **Project scope, macOS / Linux / Git Bash on Windows:**
```
  mkdir -p .agents/skills
  cp -R servicestudio-mcp-oml .agents/skills/servicestudio-mcp-oml
```
- **Global scope, macOS / Linux:**
```
  mkdir -p ~/.codex/skills
  cp -R servicestudio-mcp-oml ~/.codex/skills/servicestudio-mcp-oml
```
- **Global scope, Windows (PowerShell):**
```
  New-Item -ItemType Directory -Force "$HOME\.codex\skills" | Out-Null
  Copy-Item -Recurse servicestudio-mcp-oml "$HOME\.codex\skills\servicestudio-mcp-oml"
```

The folder name (`servicestudio-mcp-oml`) becomes the skill's identifier and
must match the `name` in the `SKILL.md` frontmatter.

> **This repo does not commit a copy under `.agents/skills/`.** The canonical
> content lives in `servicestudio-mcp-oml/`; copy it as above rather than
> maintaining two locations.

**Skills are discovered when a session starts**, so **start a new session**
after copying — adding the folder mid-session will not pick it up.

**Verify:** ask the agent what skills it has available and confirm
`servicestudio-mcp-oml` is listed.

### Alternative: AGENTS.md (project-scoped)

If you'd rather keep it per-project, drop the package's `AGENTS.md` in your
project root along with the `servicestudio-mcp-oml/` folder. Codex reads
`AGENTS.md` from the workspace root (and nested directories) automatically.
(This is the same file the Copilot CLI, Antigravity, Kiro and Cursor read, so
a single `AGENTS.md` serves all of them.)

## Step 2 — add the MCP server

Codex reads MCP configuration from **`~/.codex/config.toml`** — a home-directory
TOML file shared across the CLI, IDE extension and desktop app, so it cannot
be committed in this repo. Merge the entry from
`mcp/codex.config.toml` into it:

```toml
[mcp_servers.servicestudio]
url = "http://127.0.0.1:41820/mcp"
```

Notes on the format (these differ from other tools):
- **TOML, not JSON** — the only harness in this repo configured this way.
- The table key is **`mcp_servers`** (snake_case), not `mcpServers`.
- **`url`** alone is enough for a streamable-HTTP server — no `type` field
  and no experimental flag required.
- Stdio servers (not needed here) instead take `command` / `args` / `env`
  under the same table.

**Verify:** check Codex's MCP status (its config/session inspection surface)
and confirm `servicestudio` is connected, exposing `applyModelApiCode`,
`getDataModel`, `omlMerge`, and the other verbs.

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
2. Start a new Codex session from the repo (or with the global skill
   installed); confirm the skill is loaded and the MCP server is connected.
3. Ask: *"List the entities in the open module."* → expect a `getDataModel`
   call.
4. Ask for a trivial change → confirm a non-empty `mutatedOmlPath` and a
   follow-up `omlMerge` (Compare-and-Merge window opens in Service Studio).

## Troubleshooting

| Symptom | Cause / fix |
| --- | --- |
| Skill not picked up | Wrong nesting (must be `<scope-root>/skills/<name>/SKILL.md`, not a loose `.md`); the folder name doesn't match the `name` in the frontmatter; or you didn't start a **new** session after copying it. |
| MCP server won't connect | Service Studio not running / no module open, or a TOML syntax error in `~/.codex/config.toml` (missing quotes, wrong table name). Approve the connection in Service Studio. |
| Connection refused on `41820` specifically | Service Studio binds `41820` by default, but if that port was already taken it may be listening on a nearby one. Confirm what it actually bound — `netstat -ano \| findstr LISTENING` (Windows) or `lsof -nP -iTCP -sTCP:LISTEN` (macOS / Linux) — and put that port in the config. MCP clients need a literal URL, so there is no auto-discovery. |

