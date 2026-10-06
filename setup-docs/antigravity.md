# Service Studio OML skill → Google Antigravity

Setup for **Google Antigravity** (the agentic IDE and its CLI). Antigravity has the cleanest fit of the three targets: it has a **native Skills system that uses the same `SKILL.md` folder format** as Claude Code, so the skill ports almost verbatim. As always, it's the **same** Service Studio MCP server, so tool names and the lambda contract are identical.

## Two ways to give Antigravity the skill

| Mechanism | Scope | Source in this package | Best for |
| --- | --- | --- | --- |
| **Skill** (recommended) | Global, shared across Antigravity IDE + CLI | `servicestudio-mcp-oml/` (has `SKILL.md`) | The truest 1:1 with the Claude skill — auto-discovered, progressive reference reads |
| **AGENTS.md** | Per project (repo root) | `../AGENTS.md` | Teams who prefer project-scoped, always-on instructions |

You can use either or both.

## Prerequisites

1. **Google Antigravity** installed (IDE and/or CLI), signed in.
2. **Service Studio (OutSystems 11)** running, with the target module **open**, and its in-process MCP server **started** — it does not start automatically: in Service Studio, go to **Edit > MCP Configuration...** and click **Start MCP Server** (default port `41820`). Approve the connection the first time.

## Step 1 — install the skill

Antigravity discovers skills from a global folder, shared across the IDE and CLI. Copy the skill folder into it, naming the destination after the skill (run these from the repo root). The repo's `scripts/install.sh` / `install.ps1` only target
Claude Code, so Antigravity is a manual install:

- **macOS / Linux:**
```
  mkdir -p ~/.gemini/skills
  cp -R servicestudio-mcp-oml ~/.gemini/skills/servicestudio-mcp-oml
```
- **Windows (PowerShell):**
```
  New-Item -ItemType Directory -Force "$HOME\.gemini\skills" | Out-Null
  Copy-Item -Recurse servicestudio-mcp-oml "$HOME\.gemini\skills\servicestudio-mcp-oml"
```

The folder name (`servicestudio-mcp-oml`) becomes the skill's identifier; Antigravity reads the `name` / `description` from the `SKILL.md` frontmatter.

> **CLI-specific location:** if a skill needs to be scoped to the Antigravity CLI only, the CLI also reads `~/.gemini/antigravity-cli/skills/`. The shared `~/.gemini/skills/` above covers both the IDE and CLI, so prefer it.

**Verify:** run `/skills` in the Antigravity CLI (or ask the agent "what skills are available?") and confirm `servicestudio-mcp-oml` is listed.

### Alternative: AGENTS.md (project-scoped)

If you'd rather keep it per-project, drop the package's `AGENTS.md` in your project root along with the `servicestudio-mcp-oml/` folder. Antigravity reads `AGENTS.md` from the project root as enhanced system instructions. (This is the
same file the Copilot CLI uses, so a single `AGENTS.md` serves both.)

## Step 2 — add the MCP server

Antigravity keeps a **single, unified** MCP config shared across Antigravity 2.0, the IDE, and the CLI at:

```
~/.gemini/config/mcp_config.json
```

Merge the `servicestudio` entry (provided in `mcp/antigravity.mcp_config.json`) into it:

```json
{
  "mcpServers": {
    "servicestudio": {
      "serverUrl": "http://127.0.0.1:41820/mcp"
    }
  }
}
```

Notes on the format (these differ from other tools):
- HTTP servers use **`serverUrl`** — *not* `url` or the older `httpUrl`.
- No `type` field is needed; `serverUrl` implies streamable HTTP.
- Don't add a top-level `timeout` — it's no longer supported per server.

**Via the IDE UI instead:** open the **MCP Servers** dropdown at the top of the agent panel → **Manage MCP Servers** → **View raw config**, and paste the entry there.

**Verify:** the `servicestudio` server should show as connected, exposing `applyModelApiCode`, `getDataModel`, `omlMerge`, and the other verbs.

## Using it

Describe the change in natural language; the skill's contract steers the agent:

```
> Add a DueDate Date attribute to the Task entity in the open module.
```

Expected flow: read the model (`getDataModel`) → `applyModelApiCode` lambda → verify the response → `omlMerge` so the diff opens in Service Studio.

## Verify end-to-end

1. Open a module in Service Studio.
2. In Antigravity, confirm the skill is loaded (`/skills`) and the MCP server is connected.
3. Ask: *"List the entities in the open module."* → expect a `getDataModel` call.
4. Ask for a trivial change → confirm a non-empty `mutatedOmlPath` and a follow-up `omlMerge` (Compare-and-Merge window opens in Service Studio).

## Troubleshooting

| Symptom | Cause / fix |
| --- | --- |
| Skill not listed by `/skills` | Wrong folder. Must be `~/.gemini/skills/<name>/SKILL.md`. Confirm the `SKILL.md` frontmatter (`name`, `description`) is intact. |
| MCP server won't connect | Service Studio not running / no module open, or `serverUrl` typo (must be `serverUrl`, not `url`). Approve the connection in Service Studio. |
| Connection refused on `41820` specifically | Service Studio binds `41820` by default, but if that port was already taken it may be listening on a nearby one. Confirm what it actually bound — `netstat -ano \| findstr LISTENING` (Windows) or `lsof -nP -iTCP -sTCP:LISTEN` (macOS / Linux) — and put that port in the config. MCP clients need a literal URL, so there is no auto-discovery. |
