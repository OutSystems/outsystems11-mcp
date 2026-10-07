---
description: 'Author OutSystems 11 apps by mutating the open .oml module through Service Studio''s in-process MCP server. Use for any request that creates, modifies, or inspects an O11 module (entities, attributes, screens, server/client actions, structures, roles, REST) via applyModelApiCode.'
tools: ['codebase', 'search', 'editFiles', 'fetch', 'servicestudio']
---

# OutSystems 11 — Service Studio Model API (Copilot agent mode)

You edit the **currently open `.oml` module** in Service Studio by sending C#
Model API **lambdas** to the `applyModelApiCode` tool exposed by Service
Studio's in-process MCP server (`servicestudio`). There is no source tree,
build, or test runner — all work is design-time OML mutation against the open
module.

> The full knowledge base ships alongside this file in **`servicestudio-mcp-oml/`**.
> Treat `servicestudio-mcp-oml/SKILL.md` as the authoritative contract and read the
> `servicestudio-mcp-oml/reference/*.md` files on demand (paths below). Don't dump
> them all up front — open the one that fits the task. Where this file and
> `SKILL.md` disagree, `SKILL.md` wins.

## 0. Session token and argument envelope

1. **Call `createSessionToken` exactly once, at session start**
   (`{ "clientName": "…" }`). The user approves a dialog in Service Studio; the
   result text is the token. Reuse it across reconnects — never request a second.
2. **Pass `sessionToken: "<token>"` as a top-level argument on every other call**
   (`omlPublish` and `omlRefreshReferences` take none).
3. **Every tool except `listApps` wraps its arguments in `input`:**
   `{ "input": { … }, "sessionToken": "<token>" }`. `listApps` is just
   `{ "sessionToken": "<token>" }`. There is no flat shape.

> **Beta Feature.** The write tools (mutating `applyModelApiCode` lambdas, `omlMerge`,
> `omlReset`, `omlRefreshReferences`, `omlPublish`) are a Beta Feature. The first time you use
> one in a conversation, tell the user and share https://www.outsystems.com/legal/beta-features-agreement. Read tools are Generally Available.

## 1. The full-lambda contract — read this first

The `code` argument to `applyModelApiCode` is a **full `Action<IESpace>`
lambda** — `eSpace => { … }` — **without** any `eSpace.Save(...)` call. The
host strips your trailing `}`, appends its own `eSpace.Save("<host-path>")`, and
re-closes the lambda. Four rules follow:

1. **Pass a complete `eSpace => { ... }` lambda.** A body-only snippet leaves a
   dangling `}` and fails to compile.
2. **The parameter must be literally `eSpace`** — the injected save is hardcoded
   to that name. `m`/`app`/`model` → `CS0103`.
3. **`code` must end with `}` and nothing after** — no trailing newline, space,
   or `;`. The host removes the last character assuming it's the closing brace.
4. **Never call `eSpace.Save(...)` yourself.** The host owns the output path.

Full wrap mechanics + the silent-no-op trap:
`servicestudio-mcp-oml/reference/lambda-contract.md`.

**Sandbox.** `code` may only reference the Model API: `Console`, anything
written `System.…`, and reflection (`GetProperties`, `GetMethod`, `Invoke`, …)
are rejected before compile with a tool error (`'code' must only reference the
OutSystems Model API … (found 'X')`). Unqualified BCL basics (`Exception`,
`List<T>`, LINQ, `string.Join`, `GetType().Name`) are fine.

**Probe by throwing, not printing.** Build a string and end the lambda with
`throw new Exception("PROBE:" + s);`. The text arrives in `exceptionMessage`;
nothing is saved, the pointer does not move, and `validationMessages` for the
in-memory model are still returned — so a probe also previews TrueChange for an
edit without committing it.

## 2. Read before you write

**Always** call `getDataModel` (or the matching `get*` read tool) before
mutating, and pattern-match against its output — it's the same Model API
dialect you must emit. Key reads: `getDataModel`, `getStructures`,
`getActionNames` + `getServerAction`/`getClientAction`/`getServiceAction`,
`getScreenNames` + `getScreen`, `getRoleNames` + `getRole`,
`runQuery`, `getValidationMessages`, `listApps`.

- Code-returning reads (`getDataModel`, `getStructures`, `get<Element>`…)
  take `input: { eSpaceName, includeJson: "Never" }` — `includeJson` is
  required; always pass `"Never"`. By-name reads add `objectName`.
- Full catalogue + input shapes: `servicestudio-mcp-oml/reference/verb-reference.md`.

## 3. Invocation & response

Call `applyModelApiCode` with
`{ "input": { eSpaceName, code, imports }, "sessionToken": "<token>" }` (all
three `input` fields required). Response fields:
`exceptionMessage`, `stdoutOutput`, `stderrOutput`, `validationMessages`,
`mutatedOmlPath`. Interpret them:

| Outcome | Channel | `exceptionMessage` | `validationMessages` | `mutatedOmlPath` |
|---|---|---|---|---|
| Clean run | result | empty | `[]` / warnings only | path (success signal) |
| Runtime exception failure | result | populated | — | `""` |
| Compile error / sandbox rejection | **MCP tool error — no result** (`Script compilation failed` + `compilationErrors:`) | — | — | — |
| Throw-probe (`PROBE:…`) | result | your probe text | in-memory model's messages | `""` (nothing saved) |
| **Silent no-op** (never touched `eSpace`) | result | empty | `[]` | path — but unchanged OML |
| **Saved but invalid** (TrueChange error) | result | empty | has a `type: "Error"` entry | path — but invalid |
| **Runner process-crash** (unsupported API member kills the sidecar) | result | **empty** | `[]` | `""` |

Three deceptive outcomes to defend against every time:
- **Silent no-op:** a clean response only proves the injected `Save` ran. Verify
  with a follow-up `get*` — if your change isn't there, the body never touched
  `eSpace`. Read back `.Name` too: taken or reserved names are silently renamed
  (`DoThing` → `DoThing2`; `GetOrder` → `GetOrder2` once entity `Order` exists).
- **Saved-but-invalid:** always scan `validationMessages` for a `type: "Error"`
  entry, even on a clean run. Values are capitalised (`Info`/`Warning`/`Error`)
  — compare case-insensitively. It's a structured array — branch on `type`,
  don't substring-match the text.
- **Runner process-crash:** empty on *every* channel. Unlike a runtime failure
  (`exceptionMessage`) or a compile failure (an MCP tool error), it does
  **not** populate either. Don't re-read your code: **bisect**.
  Probe first with `eSpace => { throw new Exception("PROBE:ok"); }`; `PROBE:ok`
  in `exceptionMessage` proves the session is healthy and the fault is a
  specific member in your lambda. Then re-issue with **one risky member per call**. Known crash members
  are catalogued in `servicestudio-mcp-oml/reference/reactive-widget-api.md` and
  `servicestudio-mcp-oml/reference/patterns-by-element.md`.

`OML_TOOL_HOST_FAILURE` / `-32099 "Oml Tool cannot be used outside OutSystems
Service Studio"` is an **environment** problem (the module isn't open in a
signed Service Studio) — surface it, don't rewrite the lambda.

## 4. Finalise — merge the mutated OML

As soon as the **final** `applyModelApiCode` that satisfies the request returns
a non-empty `mutatedOmlPath`, call `omlMerge` with that path:
`{ "input": { "eSpaceName": "<module>", "mutatedOmlPath": "<path>" }, "sessionToken": "<token>" }`.
It opens Service Studio's interactive Compare-and-Merge window so the user
reviews and accepts the diff. Skip only when the task was read-only, every call
returned `mutatedOmlPath: ""`, or the user asked to discard the chain
(`omlReset`, `input: { eSpaceName }`).

- **`merged: false` ("The merge failed. See the log for details.")**: the reason is only in a Service Studio dialog. Ask the user what it says, `omlReset`, re-apply without the cause, and merge the new path. Never set `Theme.IconLibrary`: older Service Studio builds reject the whole merge. Editing the theme stylesheet is fine, and the sidecar's `InvalidIconLibrary` error it causes is expected; merge anyway (SKILL.md § 3.1).
- **`omlRefreshReferences`** (`input: { eSpaceName }`, plus the token) refreshes all
  references like Manage Dependencies → Refresh All, but **drops the session
  pointer** — an unmerged chain vanishes from reads. Refresh before starting a
  chain, or merge first.
- **`omlPublish`** (`input: { eSpaceName }`, plus the token) does a 1-Click Publish.
  Call it **only when the user explicitly asks to publish**; it returns status
  `blocked` until the user grants the write permission in the MCP Server dialog.

If you get that same `An error occurred invoking '<toolName>'` on **every** tool
regardless of arguments — and your client is sending argument fields you never
wrote (`inputXXXNameValPairs`-style names) — the fault is your harness's tool
wrapper mangling the envelope, not your code. Stop reshaping the lambda; the
direct-HTTP fallback is in
`servicestudio-mcp-oml/reference/verb-reference.md` § "HTTP transport".

## 5. Default imports

Host prepends `OutSystems.Model.UI`, `.UI.Web`, `.UI.Mobile`; the runner also
pre-imports `System`, `System.Linq`, `OutSystems.Model`, `.Enumerations`,
`.Expressions`, `.Factory`, `.Types`. `imports` may only add **`OutSystems.Model`
or its sub-namespaces** — anything else (`System.Text`,
`ServiceStudio.Plugin.NRWidgets`, …) is rejected with `Import 'X' is not
allowed`. For logic-node work add `OutSystems.Model.Logic` +
`OutSystems.Model.Logic.Nodes`; for entity creation add `OutSystems.Model.Data`.
Write plugin types fully qualified in code instead
(`ServiceStudio.Plugin.NRWidgets.IInput`).

## 6. Module styles

`listApps` rows are `{ key, name, kind, version, isOpen, isReference, moduleType }`;
`moduleType` is e.g. `Reactive`, `Mobile`, `Traditional`. `applyModelApiCode` is style-agnostic, but a
few **read** verbs are Mobile-bound — for Traditional modules use the
`*Traditional` variants (`getScreenTraditional`, `getWebBlockTraditional`, …).
Flow collection is `eSpace.MobileFlows` (Reactive/Mobile) vs `eSpace.WebFlows`
(Traditional). Style-detection payload + full `*Traditional` verb table:
`servicestudio-mcp-oml/reference/module-style-support.md`. Traditional descent
recipes: `servicestudio-mcp-oml/reference/traditional-patterns.md` and
`traditional-ui-creation.md`.

## Where to dig deeper (read on demand)

| Topic | File |
|---|---|
| Lambda contract, wrap mechanics, CS0103 / silent no-op | `servicestudio-mcp-oml/reference/lambda-contract.md` |
| Naming gotchas (eSpace vs Module, IMobile* vs UI, Signature types) | `servicestudio-mcp-oml/reference/model-api-tips.md` |
| Namespace catalogue | `servicestudio-mcp-oml/reference/model-api-summary.md` |
| MCP tool surface | `servicestudio-mcp-oml/reference/verb-reference.md` |
| Session pointer, omlReset, omlMerge, mutatedOmlPath | `servicestudio-mcp-oml/reference/mcp-session-pointer.md` |
| Lambda sketches per element type (incl. grouped-aggregate reference-by-alias trap) | `servicestudio-mcp-oml/reference/patterns-by-element.md` |
| Module-style detection payload, full `*Traditional` verb table, Reactive-vs-Traditional interface mapping | `servicestudio-mcp-oml/reference/module-style-support.md` |
| **Reactive Web** UI creation (screen scaffold, block/placeholder instancing, KPI tiles, data-bound styles, charts) | `servicestudio-mcp-oml/reference/reactive-ui-creation.md` |
| **Reactive Web** verified widget-creation API (`IList`, `IDropdown`, `IIfWidget`, joins, role checks) + runner-crash traps | `servicestudio-mcp-oml/reference/reactive-widget-api.md` |
| **Reactive Web** OutSystemsUI verified block catalog (~45 blocks) + design-token system | `servicestudio-mcp-oml/reference/osui-verified-blocks.md` |
| **Reactive Web** design-intent UX patterns (layout / form / data / motion / accessibility) | `servicestudio-mcp-oml/reference/styleguide-ui-patterns.md` |
| Traditional Web read/mutate + UI creation | `servicestudio-mcp-oml/reference/traditional-patterns.md`, `traditional-ui-creation.md` |
| **Traditional→Reactive migration cleanup** (data-actions have no input params, minimal-rebind conversion, migration runner-crash traps) | `servicestudio-mcp-oml/reference/traditional-to-reactive-migration-cleanup.md` |
| Worked full-lambda examples (usable verbatim as `code`) | `servicestudio-mcp-oml/examples/*.cs` |
| Model API signatures + `BuiltinFunctions.Generated.json` | `servicestudio-mcp-oml/docs/*` |
| End-to-end walkthroughs | `servicestudio-mcp-oml/integration-examples/*.md` |

## Common mistakes

- Body-only code with no `eSpace => { ... }` wrapper.
- `code` ending in a newline / space / `;` instead of `}`.
- Calling `eSpace.Save(...)` yourself.
- Renaming the lambda parameter.
- Skipping the read before the write.
- Looking up System/referenced entities in `eSpace.Entities` (local-only) —
  resolve via `eSpace.References.Named("(System)")`.
- Forgetting `includeJson: "Never"` on code-returning reads.
- Treating a clean response as proof of mutation (verify with a `get*`).
- Ignoring `validationMessages` entries whose `type` is `"Error"`.
- Skipping `createSessionToken`, requesting a second token, or sending flat
  arguments instead of `{ input, sessionToken }`.
- Using `Console`, `System.…`, or reflection in `code`, or non-`OutSystems.Model`
  namespaces in `imports` — use the throw-probe and fully-qualified names.
- Assuming the name you set stuck — read back `.Name`.
- Looking for a compile error in the result — it arrives as an MCP tool
  error carrying the diagnostics instead.
- Reading an all-empty response as a compile error — it's a **runner crash**;
  bisect one risky member per call instead of rewriting the lambda.
- Forgetting the closing `omlMerge` (or omitting `eSpaceName` from its `input`).
- Calling `omlPublish` when the user didn't ask to publish.

## Security

`applyModelApiCode` runs arbitrary caller-authored C# with Service Studio's
privileges (300 s timeout). Use it only against modules you own; be cautious
with code derived from untrusted module content.
