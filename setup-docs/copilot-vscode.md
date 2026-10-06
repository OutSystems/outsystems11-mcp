# Service Studio OML skill → GitHub Copilot (VS Code)

This package ports the Claude Code `servicestudio-mcp-oml` skill to **GitHub Copilot in VS Code**. Because you connect to the *same* Service Studio in-process MCP server, the tool names and the entire lambda contract are **identical** — only the packaging changes.

## What VS Code uses (already at the repo root)

Open this repository as your VS Code workspace and everything below is discovered automatically — no copying required:

```
<repo root>/
├─ .vscode/
│  └─ mcp.json                              ← connects Copilot to the MCP server
├─ .github/
│  ├─ chatmodes/
│  │  └─ servicestudio-mcp-oml.chatmode.md  ← PRIMARY: the skill as a custom chat mode
│  ├─ copilot-instructions.md               ← ALT: always-on instructions for the workspace
│  └─ prompts/
│     └─ oml-edit.prompt.md                 ← convenience /oml-edit prompt
└─ servicestudio-mcp-oml/                        ← the knowledge base
   ├─ SKILL.md
   ├─ reference/*.md      (contract, verbs, patterns, traditional web, …)
   ├─ examples/*.cs       (86 ready-to-use full lambdas)
   ├─ docs/*              (Model API signatures + BuiltinFunctions.json)
   └─ integration-examples/*.md
```

## Prerequisites

1. **VS Code** 1.102 or newer (MCP support is generally available from 1.102).
2. **GitHub Copilot** + **GitHub Copilot Chat** extensions, signed in with a    plan that includes **agent mode** and **MCP tools**.
3. **Service Studio (OutSystems 11)** running, with the target module **open**, and its in-process MCP server **started** — it does not start automatically: in Service Studio, go to **Edit > MCP Configuration...** and click **Start MCP Server** (default port `41820`).
   (This is the same server Claude Code uses. Every new client connection must be **approved in Service Studio** the first time.)

## Install

Pick a scope:

- **Per project (recommended):** open this repository as the VS Code workspace (or copy `.vscode/`, `.github/`, and `servicestudio-mcp-oml/` into an existing workspace root). Everything is discovered automatically.
- **Per user (all workspaces):** put the chat mode, prompt, and instructions in your VS Code user profile instead — run **`Chat: Configure Chat Modes`**,  **`Chat: Configure Prompt Files`**, and **`Chat: New Instructions File`** from the Command Palette and paste the file contents; add the MCP server with **`MCP: Add Server`** (or `MCP: Open User Configuration`).

### Step 1 — connect the MCP server

The provided `.vscode/mcp.json` does this for a workspace:

```json
{
  "servers": {
    "servicestudio": {
      "type": "http",
      "url": "http://127.0.0.1:41820/mcp"
    }
  }
}
```

Open the workspace, then run **`MCP: List Servers`** → `servicestudio` → **Start**. On first connect, approve the connection in Service Studio. Confirm the tools are live with **`MCP: List Servers` → `servicestudio` → Show Output**,
or open the Chat **Tools** picker (🛠️) and check that `servicestudio` tools (`applyModelApiCode`, `getDataModel`, `omlMerge`, …) appear.

### Step 2 — enable the skill packaging

Two ways to give Copilot the contract. **Use the chat mode** for a clean, scoped experience; the always-on instructions file is a simpler alternative.

**A. Chat mode (primary).** With `.github/chatmodes/servicestudio-mcp-oml.chatmode.md`  in place, open Copilot Chat, click the **mode dropdown** (top of the chat view, usually reads *Ask / Edit / Agent*), and pick **servicestudio-mcp-oml**. This loads the contract and scopes the tools. If the `servicestudio` tools aren't already ticked, open the 🛠️ **Tools** picker in that mode and enable them.

**B. Always-on instructions (alternative).**
`.github/copilot-instructions.md` is auto-included in every Copilot Chat request for the workspace — no mode switch needed. Ensure `"github.copilot.chat.codeGeneration.useInstructionFiles": true` in settings (it's the default). Good if the colleague only uses this workspace for OML work.

You can use both — the chat mode adds the detailed contract on top of the always-on baseline.

### Step 3 — settings to verify (Command Palette → *Preferences: Open User Settings (JSON)*)

```jsonc
{
  // enable prompt files (for /oml-edit) and custom instruction files
  "chat.promptFiles": true,
  "github.copilot.chat.codeGeneration.useInstructionFiles": true,
  // optional: allow the agent to auto-run the servicestudio MCP tools without a
  // prompt each time (only do this if the colleague trusts the setup)
  "chat.tools.autoApprove": false
}
```

Recent VS Code enables prompt/instruction files by default; set the flags explicitly if the features don't appear.

## Using it

- **Chat mode:** select **servicestudio-mcp-oml** from the mode dropdown and ask in   plain language — e.g. *"Add a `DueDate` Date attribute to the `Task` entity."* The agent reads the model first, sends the lambda, verifies, and merges.
- **Prompt:** type **`/oml-edit`** in chat and fill in the requested change. It runs the read → mutate → verify → merge loop for you.
- **On demand:** the agent opens `servicestudio-mcp-oml/reference/*.md` and `examples/*.cs` as needed — the same progressive-reference model as the Claude skill.

## Verify the setup end-to-end

1. Open a module in Service Studio.
2. In VS Code (servicestudio-mcp-oml mode), ask: *"List the entities in the open module."* → the agent should call `getDataModel` and return your entities.
3. Ask for a trivial change (e.g. add a description to an action) → confirm the agent calls `applyModelApiCode`, reports a non-empty `mutatedOmlPath`, then calls `omlMerge` and the Compare-and-Merge window opens in Service Studio.

## Troubleshooting

| Symptom | Cause / fix |
| --- | --- |
| MCP server won't start / no `servicestudio` tools | Service Studio not running, or module not open. Start it, open a module, then `MCP: List Servers → Start`. Approve the connection prompt in Service Studio. |
| Connection refused on `41820` specifically | Service Studio binds `41820` by default, but if that port was already taken it may be listening on a nearby one. Confirm what it actually bound — `netstat -ano \| findstr LISTENING` (Windows) or `lsof -nP -iTCP -sTCP:LISTEN` (macOS / Linux) — and put that port in `.vscode/mcp.json`. MCP clients need a literal URL, so there is no auto-discovery. |
