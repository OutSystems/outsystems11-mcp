# Service Studio OML skill → GitHub Copilot CLI

Setup for the **agentic GitHub Copilot CLI** (the standalone `copilot` command). Run the CLI from the repo root. If you are on VS Code instead, see `setup-docs/copilot-vscode.md`. All paths below are relative to the repo root.

> ⚠️ This is **not** the old `gh copilot suggest` / `gh copilot explain` extension — that one can't drive MCP servers or edit modules. You need the agentic Copilot CLI (`npm install -g @github/copilot`, then run `copilot`).

## What applies in the CLI

| File | Applies to CLI? | Notes |
| --- | --- | --- |
| `AGENTS.md` | ✅ **primary** | Auto-loaded by the CLI from the working dir (and parents). This carries the skill contract. |
| `.github/copilot-instructions.md` | ✅ | Also read by the CLI; same content as `AGENTS.md`. Either is enough. |
| `servicestudio-mcp-oml/` | ✅ | The knowledge base. The agent reads these files from disk as needed. |
| `mcp/copilot-cli.mcp-config.json` | ✅ (reference) | The MCP server config in the CLI's format — import it or mirror it (see Step 1). |
| `.github/chatmodes/`, `.github/prompts/`, `.vscode/mcp.json` | ❌ | VS Code–only — ignore them for CLI use. See `setup-docs/copilot-vscode.md`. |

## Prerequisites

1. **Node.js 18+**, then install the CLI:
```
   npm install -g @github/copilot
```
   (Or via your platform's documented installer.) Verify with `copilot --version`.
2. Signed in with a GitHub plan that includes Copilot agent access 
   (`/login` inside the CLI, or `copilot` will prompt on first run).
3. **Service Studio (OutSystems 11)** running, with the target module **open**, and its in-process MCP server **started** — it does not start automatically: in Service Studio, go to **Edit > MCP Configuration...** and click **Start MCP Server** (default port `41820`). Approve the connection the first time.

## Step 1 — add the MCP server

The reliable, version-proof way is the interactive command **inside** a
`copilot` session:

```
/mcp add
```

Answer the prompts:

| Prompt | Value |
| --- | --- |
| Server name | `servicestudio` |
| Type / transport | `HTTP` (streamable HTTP) |
| URL | `http://127.0.0.1:41820/mcp` |
| Tools | allow all (`*`) |

Then run `/mcp` to confirm `servicestudio` is connected and its tools (`applyModelApiCode`, `getDataModel`, `omlMerge`, …) are listed.

**Config-file alternative.** The CLI keeps its MCP config in a JSON file under its config directory (commonly `~/.copilot/mcp-config.json`). The provided `mcp/copilot-cli.mcp-config.json` is in that format:

```json
{
  "mcpServers": {
    "servicestudio": {
      "type": "http",
      "url": "http://127.0.0.1:41820/mcp",
      "tools": ["*"]
    }
  }
}
```

Merge that `mcpServers` entry into the CLI's config file, or point the CLI at it per its docs. If the exact path differs on your colleague's version, `/mcp add`  above sidesteps the question entirely — run `copilot help` / `/help` to confirm
the current config location.

## Step 2 — put the instructions where the CLI will find them

The CLI auto-discovers `AGENTS.md` (and `.github/copilot-instructions.md`) from the **directory you launch `copilot` in**, walking up to parent dirs. So:

1. Keep `AGENTS.md` and the `servicestudio-mcp-oml/` folder together in one project directory.
2. Launch the CLI from that directory:
```
   cd /path/to/outsystems11-mcp
   copilot
```

That's it — no chat mode to select. The contract in `AGENTS.md` is active for every turn, and the agent opens `servicestudio-mcp-oml/reference/*.md` and `examples/*.cs` on demand.

## Using it

Just describe the change in natural language, e.g.:

```
> Add a DueDate Date attribute to the Task entity in the open module.
```

The agent should: read the model (`getDataModel`), send an `applyModelApiCode` lambda, verify the response, then call `omlMerge` so the diff opens in Service Studio for review.

To point the agent at deeper reference explicitly:

```
> Follow the REST recipe in servicestudio-mcp-oml/reference/patterns-by-element.md
  and expose the Order entity as a read-only REST endpoint.
```

## Verify end-to-end

1. Open a module in Service Studio.
2. From the project dir, run `copilot`, then ask: *"List the entities in the open module."* → expect a `getDataModel` call returning your entities.
3. Ask for a trivial change → confirm `applyModelApiCode` returns a non-empty `mutatedOmlPath` and the agent then calls `omlMerge` (Compare-and-Merge window opens in Service Studio).

## Troubleshooting

| Symptom | Cause / fix |
| --- | --- |
| `/mcp` shows no `servicestudio` or it won't connect | Service Studio not running / no module open. Start it, open a module, re-add. Approve the connection prompt in Service Studio. |
| Connection refused on `41820` specifically | Service Studio binds `41820` by default, but if that port was already taken it may be listening on a nearby one. Confirm what it actually bound — `netstat -ano \ | findstr LISTENING `(Windows) or` lsof -nP -iTCP -sTCP:LISTEN` (macOS / Linux) — and put that port in the config. MCP clients need a literal URL, so there is no auto-discovery. |
