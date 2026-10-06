# Service Studio OML skill → Kiro

The steps below are written for the **Kiro CLI** (`kiro-cli`). The IDE reads the same `.kiro/` configuration, so everything applies there too.

## Two ways to give Kiro the skill

| Mechanism | Scope | Source in this package | Best for |
| --- | --- | --- | --- |
| **Skill** (recommended) | Global (`~/.kiro/skills/`) or per workspace (`.kiro/skills/`) | `servicestudio-mcp-oml/` (has `SKILL.md`) | The truest 1:1 with the Claude skill — auto-discovered, progressive reference reads |
| **AGENTS.md** | Per project (repo root) | `../AGENTS.md` | Teams who prefer project-scoped, always-on instructions |

You can use either or both. `AGENTS.md` is always included and supports no inclusion modes, so keep it thin and let the skill carry the depth.

## Prerequisites

1. **Kiro CLI** installed and signed in:
```
   curl -fsSL https://cli.kiro.dev/install | bash        # macOS / Linux
   irm 'https://cli.kiro.dev/install.ps1' | iex          # Windows (PowerShell)
```
   Running `kiro-cli` the first time opens a browser sign-in (Google, GitHub, AWS Builder ID, or your organization identity). No Pro subscription is required for the CLI. Check state any time with `kiro-cli whoami`.
2. **Service Studio (OutSystems 11)** running, with the target module **open**, and its in-process MCP server **started** — it does not start automatically: in Service Studio, go to **Edit > MCP Configuration...** and click **Start MCP Server** (default port `41820`). Approve the connection the first time.

## Step 1 — install the skill

Kiro discovers skills from a global folder shared by the IDE and CLI. Copy the skill folder into it, naming the destination after the skill (run these from the repo root):

- **macOS / Linux:**
```
  mkdir -p ~/.kiro/skills
  cp -R servicestudio-mcp-oml ~/.kiro/skills/servicestudio-mcp-oml
```
- **Windows (PowerShell):**
```
  New-Item -ItemType Directory -Force "$HOME\.kiro\skills" | Out-Null
  Copy-Item -Recurse servicestudio-mcp-oml "$HOME\.kiro\skills\servicestudio-mcp-oml"
```

The folder name (`servicestudio-mcp-oml`) becomes the skill's identifier and **must match the `name` in the `SKILL.md` frontmatter**. The `<name>/SKILL.md` nesting is mandatory — a `.md` file placed directly under `~/.kiro/skills/` is ignored.

> **Workspace scope instead:** to scope the skill to one project, use `.kiro/skills/servicestudio-mcp-oml/` in that project's root. A workspace skill wins over a global one of the same name.

**Skills are discovered when a chat session starts**, so **start a new session** after copying — adding the folder mid-session will not pick it up.

**Verify:** run `/context show` in the CLI (or ask the agent "what skills are available?") and confirm `servicestudio-mcp-oml` is listed. You can also invoke it explicitly with `/servicestudio-mcp-oml`.

### Alternative: AGENTS.md (project-scoped)

If you'd rather keep it per-project, drop the package's `AGENTS.md` in your project root along with the `servicestudio-mcp-oml/` folder. Kiro reads `AGENTS.md` from the workspace root automatically, and it is always included. (This is the same file the Copilot CLI and Antigravity use, so a single `AGENTS.md` serves all three.)

## Step 2 — add the MCP server

Kiro reads MCP configuration from two places, and **merges them with the workspace file taking precedence**:

| Scope | Path |
| --- | --- |
| Workspace | `.kiro/settings/mcp.json` |
| Global | `~/.kiro/settings/mcp.json` |

**This repo already ships the workspace file** (`.kiro/settings/mcp.json`) — if you launch `kiro-cli` from the repo root there is nothing to install. For any other project, merge the same entry into the global file:

```json
{
  "mcpServers": {
    "servicestudio": {
      "url": "http://127.0.0.1:41820/mcp"
    }
  }
}
```

Notes on the format (these differ from other tools):
- HTTP servers use **`url`** — *not* `serverUrl` (Antigravity) or `httpUrl`.
- **No `type` field.** Kiro infers the transport from which key is present: `command` means a local stdio server, `url` means a remote one. Adding `"type": "http"` is undocumented and unnecessary.
- Plain `http://` is fine here — Kiro requires HTTPS only for non-localhost endpoints.
- **No restart needed.** MCP configuration changes apply when you save the file.
- `"autoApprove": ["*"]` would skip Kiro's per-tool confirmations. Deliberately omitted — see **Notes**.

**Launch `kiro-cli` from the repo root** so the workspace file is found. Whether the CLI walks parent directories to locate it is not documented, so don't rely on it.

**Verify:** `kiro-cli mcp status --name servicestudio` (or `/mcp` inside a session) should show the server connected, exposing `applyModelApiCode`, `getDataModel`, `omlMerge`, and the other verbs. `kiro-cli doctor` diagnoses a broken setup.

## Using it

Describe the change in natural language; the skill's contract steers the agent:

```
> Add a DueDate Date attribute to the Task entity in the open module.
```

Expected flow: read the model (`getDataModel`) → `applyModelApiCode` lambda → verify the response → `omlMerge` so the diff opens in Service Studio.

## Verify end-to-end

1. Open a module in Service Studio.
2. From the repo root, start a new `kiro-cli` session; confirm the skill is loaded (`/context show`) and the MCP server is connected (`/mcp`).
3. Ask: *"List the entities in the open module."* → expect a `getDataModel` call.
4. Ask for a trivial change → confirm a non-empty `mutatedOmlPath` and a follow-up `omlMerge` (Compare-and-Merge window opens in Service Studio).

## Troubleshooting

| Symptom | Cause / fix |
| --- | --- |
| Skill not listed by `/context show` | Three usual causes: wrong nesting (must be `~/.kiro/skills/<name>/SKILL.md`, not a loose `.md`); the folder name doesn't match the `name` in the frontmatter; or you didn't start a **new** session after copying it. |
| Skill and `AGENTS.md` both ignored | You're running a **custom agent** (`--agent <name>`). Custom agents load neither skills nor steering by default — they need explicit `skill://` and `file://` entries in the agent's `resources`. Use the default agent. |
| MCP server won't connect | Service Studio not running / no module open, or a field-name error — it must be `url`, not `serverUrl`, and there is no `type` field. Approve the connection in Service Studio. |
| MCP server not found at all | `kiro-cli` was launched outside the repo root, so the workspace `.kiro/settings/mcp.json` wasn't picked up. Either `cd` to the repo root or add the entry to `~/.kiro/settings/mcp.json`. |
| Connection refused on `41820` specifically | Service Studio binds `41820` by default, but if that port was already taken it may be listening on a nearby one. Confirm what it actually bound — `netstat -ano \| findstr LISTENING` (Windows) or `lsof -nP -iTCP -sTCP:LISTEN` (macOS / Linux) — and put that port in the config. MCP clients need a literal URL, so there is no auto-discovery. |
