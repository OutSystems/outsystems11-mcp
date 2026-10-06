# OutSystems 11 MCP

**Maintain and evolve your OutSystems 11 estate with any coding agent — governed by the O11 lifecycle.**

OutSystems 11 is the stable, trusted foundation your core operations run on. The OutSystems 11 MCP lets you modernize it with confidence: external coding agents read and evolve your O11 applications through a local MCP server embedded in Service Studio — so you get AI speed without the cost, disruption, or risk of a replatform.

## Why it matters

Years of logic, entities, and integrations shouldn't sit frozen in the platform. The OutSystems 11 MCP opens your application model to the AI tools your teams already use — turning a static estate back into a living, evolvable asset, in place.

- **Evolve in place** — no migration, no replatform. Work on the estate you already run.
- **Governed by the O11 lifecycle** — every change flows through the review and quality gates you already trust.
- **Yours, and only yours** — runs locally in your environment, including on-premises and air-gapped deployments. Nothing leaves your environment.
- **Open and neutral** — works with any MCP-compatible coding agent. Optimized today for Claude Code, with GitHub Copilot (CLI and VS Code), Google Antigravity, Kiro, Cursor and Codex CLI also supported.

## What it is

| Aspect | Details |
| --- | --- |
| **Tool** | A local MCP server, embedded in Service Studio (11.55.91 and later) — the entry point for your agent. |
| **Data** | Your O11 application model, read and written via the OutSystems Model API. |
| **Reach** | Any MCP-compatible agent; optimized today for Claude Code, and packaged for GitHub Copilot (CLI and VS Code), Google Antigravity, Kiro, Cursor and Codex CLI. |
| **Capabilities** | Read and analyze — **Generally Available**. Model writes — **Beta**. |
| **Guardrails** | Every change runs through the O11 lifecycle — nothing leaves your environment. |

## Capability status

| Capability | Status | What it covers |
| --- | --- | --- |
| **Read** | **Generally Available (GA)** | Every read tool — `getDataModel`, `getScreen`, `getServerAction`, `runQuery`, `getValidationMessages`, `listApps`, … |
| **Write** | **Beta** | Mutating `applyModelApiCode` lambdas (new or changed entities, screens, actions, …), plus `omlMerge`, `omlReset`, `omlRefreshReferences` and `omlPublish`. |

The write capabilities above are Beta Features. OutSystems provides Beta Features to collect customer feedback on non-final capabilities. A Beta Feature can change significantly, including through breaking changes, or OutSystems can discontinue it. For the terms that apply, refer to the [OutSystems Beta Features Agreement](https://www.outsystems.com/legal/beta-features-agreement).

Beta write capabilities are fully usable, but their behavior may still change between Service Studio releases. By default, every write is staged and reviewed through Service Studio's interactive Compare-and-Merge window before anything is accepted into your module.

## What you can do

**Understand your estate (GA)** — with zero risk. Just ask your agent:

- **Understand an application** — "Walk me through what this module does and how it's structured."
- **Map dependencies** — "What does this module depend on, and what breaks if I change this action?"
- **Generate documentation** — onboarding guides and process flows for the module open in Service Studio.
- **Surface risk** — flag security, performance, and compliance concerns.
- **Baseline quality** — find dead code and gaps in test coverage.

**Evolve your estate (Beta)** — bounded Model API writes, such as new entities, screens and actions, reviewed through Service Studio's Compare-and-Merge window before anything is accepted.

## What's in this repository

This repo distributes the OutSystems 11 MCP skill — it teaches your agent how to read and evolve the OutSystems 11 module open in Service Studio.

| Layer | Cross-tool standard? | In this repo |
| --- | --- | --- |
| **Skill / knowledge base** | ◑ `SKILL.md` format — shared by Claude Code, Antigravity, Kiro, Codex and Cursor | [`servicestudio-mcp-oml/`](servicestudio-mcp-oml/) — start with `SKILL.md` |
| **Agent instructions** | ✅ `AGENTS.md` — cross-tool standard, read by Copilot, Antigravity, Kiro, Cursor, Codex… and Claude Code | `AGENTS.md`, plus `.github/copilot-instructions.md` for Copilot |
| **MCP client config** | ✗ no standard — every client's file location and schema differ | committed where the client reads from the repo; samples in [`mcp/`](mcp/) where it reads from your home directory |

```
outsystems11-mcp/
├─ servicestudio-mcp-oml/        # the skill: SKILL.md + reference/ + examples/ + docs/
├─ AGENTS.md                     # cross-tool agent instructions (the contract, condensed)
├─ CLAUDE.md                     # Claude Code's native instructions file
├─ ARCHITECTURE.md               # system boundary + the tenets governing skill content
├─ CONTRIBUTING.md               # how to change this repo
├─ MCP-DESIGN-CHOICES.md         # MCP host design choices & caveats worth knowing before you use the Service Studio MCP
├─ .github/
│  ├─ copilot-instructions.md    # Copilot always-on instructions
│  ├─ chatmodes/                 # VS Code Copilot chat mode
│  └─ prompts/                   # VS Code Copilot /oml-edit prompt
├─ .mcp.json                     # committed MCP config (Claude Code, in-repo)
├─ .claude/settings.json         # pre-approves the servicestudio MCP server
├─ .vscode/mcp.json              # committed MCP config (Copilot in VS Code)
├─ .kiro/settings/mcp.json       # committed MCP config (Kiro, in-repo)
├─ .cursor/mcp.json              # committed MCP config (Cursor, in-repo)
├─ mcp/                          # MCP config samples for home-directory clients
│  ├─ antigravity.mcp_config.json
│  ├─ copilot-cli.mcp-config.json
│  └─ codex.config.toml
├─ scripts/                      # install.sh / install.ps1 — installs the skill for Claude Code
└─ setup-docs/                   # per-tool setup guides
```

## Getting started (Claude Code scenario)

### 1. Install Service Studio

The MCP server your agent connects to is **embedded in Service Studio** — the skill on its own has nothing to talk to without it. Install or update to **Service Studio 11.55.91 or later**; the MCP server ships in the standard release, with no separate build or activation required.

### 2. Start the MCP server in Service Studio

The MCP server is embedded in Service Studio, but it does **not** start automatically — you start it per session, from the module you want to work on:

1. Open the module in Service Studio.
2. Go to **Edit > MCP Configuration...**.
3. Confirm the **MCP server port** (default `41820`) and click **Start MCP Server**.

The dialog then shows the **MCP server URL** and the exact **Claude Code command** to register it, each with its own **Copy** button — you can copy the command straight from here instead of retyping it in step 3 below.

### 3. Register the MCP server

Before you open Claude Code and add the skill, register the Service Studio MCP server so your agent knows where to reach it. In a terminal, run the command you copied in step 2 (or type it out):

```
claude mcp add --transport http servicestudio http://127.0.0.1:41820/mcp
```

This registers an HTTP MCP server named `servicestudio` pointing at the local endpoint exposed by Service Studio. Service Studio must be running with a module open **and its MCP server started** (step 2) for the connection to succeed.

> This is what you want for everyday use: the skill installs globally, so you'll be running Claude Code from your own projects. If you are working **inside this repository** instead, you can skip the command — the committed `.mcp.json` already declares the same server and `.claude/settings.json` pre-approves it.

### 4. Add the skill to Claude Code

Clone this repository and run the installer for your platform. It copies `servicestudio-mcp-oml/` into your Claude Code skills directory (`%USERPROFILE%\.claude\skills\` on Windows, or` ~/.claude/skills/` on macOS/Linux), overwriting any existing copy. Re-run the same command any time to update to the latest version.

```
git clone https://github.com/OutSystems/outsystems11-mcp.git
cd outsystems11-mcp
```

**macOS / Linux / Git Bash on Windows**

```
bash scripts/install.sh
```

**Windows PowerShell**

```
powershell -ExecutionPolicy Bypass -File scripts\install.ps1
```

Start a new Claude Code session afterward so the skill loads.

> Using a different agent? See [Using other coding agents](#using-other-coding-agents) — steps 1, 2 and 5 still apply, only the skill install and MCP registration differ.

### 5. Authorize Service Studio

Open the module you want to work on. The first time your agent issues a command, Service Studio shows an access prompt — approve it to establish the trust handshake between your agent and Service Studio.

If the connection between Claude and Service Studio isn't working, try running `/mcp reconnect servicestudio` in your Claude Code session.

### 6. Verify

Start a new Claude Code session, with a Reactive module open in Service Studio, and ask:

> Summarize the module open in Service Studio.

If you get a summary back, you're ready.

> **Good to know:** your coding agent reaches its model over the network. Everything else — the MCP server and your application model — runs locally in your environment.

## Using other coding agents

Claude Code is the optimized path, and the steps above describe it. The same skill and the same MCP server work with other MCP-compatible agents — each just reads its instructions and its MCP config from a different place:

| Tool | Instructions | MCP config | Setup guide |
| --- | --- | --- | --- |
| **Copilot CLI** (`copilot`) | `AGENTS.md` (auto, from the repo root) | `~/.copilot/mcp-config.json` — sample in [`mcp/`](mcp/) | [setup-docs/copilot-cli.md](setup-docs/copilot-cli.md) |
| **Copilot in VS Code** | `.github/copilot-instructions.md` + chat mode | `.vscode/mcp.json` (committed — nothing to install) | [setup-docs/copilot-vscode.md](setup-docs/copilot-vscode.md) |
| **Antigravity** (`agy`) | skill in `~/.gemini/skills/` or `AGENTS.md` | `~/.gemini/config/mcp_config.json` — sample in [`mcp/`](mcp/) | [setup-docs/antigravity.md](setup-docs/antigravity.md) |
| **Kiro** (`kiro-cli`) | skill in `~/.kiro/skills/` or `AGENTS.md` | `.kiro/settings/mcp.json` (committed — nothing to install) | [setup-docs/kiro.md](setup-docs/kiro.md) |
| **Codex CLI** | skill in `.agents/skills/` or `~/.codex/skills/`, or `AGENTS.md` | `~/.codex/config.toml` — sample in [`mcp/`](mcp/) | [setup-docs/codex.md](setup-docs/codex.md) |
| **Cursor** | skill in `.cursor/skills/` or `.agents/skills/`, or `AGENTS.md` | `.cursor/mcp.json` (committed — nothing to install) | [setup-docs/cursor.md](setup-docs/cursor.md) |

Steps 1, 2 and 5 of Getting started apply to every tool — you still need Service Studio 11.55.91 or later, its MCP server started, and the first-connection approval in Service Studio. Only the skill install and the MCP registration differ. Note that `scripts/install.sh` / `install.ps1` install for **Claude Code only**, so the other tools are a manual copy; each guide above has the exact commands.
