# MCP tool reference

Service Studio runs an **in-process MCP server** over HTTP at
`http://127.0.0.1:41820/mcp`. It does not start automatically — the user starts
it from Service Studio, on demand — and every connection is then gated behind
a per-connection user-consent prompt. A connection attempt before the user has
started it fails with `ConnectionRefused`. **Tool names are camelCase, with no
`oml_` prefix.**

Tools fall into three buckets:

- **Native host tools** that act on the live Service Studio session directly:
  `createSessionToken`, `listApps`, and the host-only `omlReset` / `omlMerge` /
  `omlRefreshReferences` / `omlPublish`.
- **Sidecar-backed read tools** — `getDataModel`, `getScreen`, `runQuery`,
  `getValidationMessages`, … Each one snapshots the open module to
  `%TEMP%\ServiceStudio.MCPServer\<runId>-in.oml`, runs the bundled sidecar, and returns
  the result. One child process per call.
- **The write tool** `applyModelApiCode`, which works the same way but also
  injects the save and advances the session pointer on success.

The sidecar verb name equals the MCP tool name (each tool class sets
`VerbName = McpToolNames.<X>`). The host injects `omlPath` (the snapshot, or the
session-pointer path) into every sidecar call, so the agent never threads OML
paths between calls (see [`mcp-session-pointer.md`](mcp-session-pointer.md)).

> **Source of truth:** `Host/McpToolNames.cs` for the names, and each
> `Tools/*.cs` class for its input record + description. Keep this reference in
> sync with those files.

## Session token and the `input` envelope

Verified live against Service Studio 11 on 2026-09-30.

1. **`createSessionToken`** `{ clientName?: string }` — call it **once, at the very
   start of the agent session, before any other tool**. Service Studio shows the user
   an approval dialog naming `clientName`; once approved, the result text is the
   token. Keep it for the whole session. Losing the connection or repeating
   `initialize` does **not** invalidate it — reuse it; only a genuinely new agent
   session requests a new one.
2. **Every other tool takes `sessionToken` as a top-level argument** (the schema also
   tags it `x-mcp-header: SessionToken`). A call with no token, or a token this
   server didn't issue, is rejected. **Send it on every call**, including
   `omlPublish`, `omlRefreshReferences` and `getRoleExceptions` — their schemas don't declare it, but send it anyway: `getRoleExceptions` rejects a call without it ("This tool requires a valid session token", verified 2026-10-01).
3. **Every tool except `listApps` wraps its own arguments in `input`:**

```jsonc
{ "input": { "eSpaceName": "MyModule", "includeJson": "Never" }, "sessionToken": "<token>" }   // getDataModel
{ "sessionToken": "<token>" }                                                                // listApps
```

The old flat shape (`{ "eSpaceName": …, … }` at top level) no longer deserializes.

## `eSpaceName`

**Every tool except `listApps` and `createSessionToken` takes a required
`input.eSpaceName:string`** — the target module's `name` from `listApps`. To keep
the Args columns below scannable, neither `eSpaceName` nor the `input` / `sessionToken`
envelope is repeated in every row — assume they're always there.

## HTTP transport

Any standard MCP client (Claude Code, the Antigravity IDE / CLI, Copilot, Kiro,
Codex CLI, Cursor) negotiates all of this for you. Read this section only if you are posting to the
endpoint with a **raw HTTP client** — which you would do solely to work around a
broken harness tool wrapper (see the ⚠️ note in
[`../SKILL.md`](../SKILL.md#common-mistakes)).

- **`Content-Type: application/json`** on every POST.
- **`Accept` must list both content types** — `application/json,
  text/event-stream`. The endpoint answers **HTTP 406** if either is missing.
  Responses arrive as SSE: parse the `data: {…}` line.
- `initialize` → server `outsystems-studio`. Its response carries **no
  `Mcp-Session-Id` header** (verified 2026-09-30); authorization is the session
  token, not a transport session. Send `notifications/initialized`, then call
  `createSessionToken` once.
- **Persist the token across processes, not just in memory.** If you drive the
  endpoint from one-shot shell / `curl` invocations, every call is a fresh process,
  so an in-memory token dies when the script exits and you would prompt the user
  again. Write it to a file and read it back at the top of every script. Keep that
  file in the OS temp dir (alongside `%TEMP%\ServiceStudio.MCPServer\`) — **not** in
  the user's repository: it is an authorization token.
- **On rejection, request a new token once** — a token goes stale when Service
  Studio restarts or the user revokes the agent. Don't loop re-sending a dead token,
  and don't request a new token unconditionally on every call.
- Pass your **own real client identity** as `clientName` (and in `initialize`'s
  `clientInfo`). The approval dialog names the client, so the user must be
  approving what is actually calling them.

## `includeJson`

The **code-returning read tools** (those whose input record is
`GetModelObjectCodeInput` or `GetModelObjectByNameCodeInput`) declare a
**required** `includeJson` field with values `"Never"`, `"Always"`,
`"IfReferenced"`. **But the host overwrites it to `"Never"` before
calling the sidecar**, so the value you send is effectively ignored — yet the
field must be present or the MCP call fails to deserialize. **Always pass
`"Never"`.**

The name-list tools (`get…Names`), `listApps`, `runQuery`,
`getValidationMessages`, `omlReset`, and `omlMerge` take **no** `includeJson`.

## Write tool — `applyModelApiCode`

General-purpose mutate-via-C#. Pass a **full `eSpace => { ... }` lambda
without** `eSpace.Save(...)`; the host strips the trailing `}`, appends its own
save against a host-allocated path, re-closes, and forwards to the sidecar. See
[`lambda-contract.md`](lambda-contract.md).

**Args:**

- `input.eSpaceName` (string, required) — the target module's `name` from `listApps`.
- `code` (string, required) — a full `eSpace => { … }` lambda ending in `}`.
  No `eSpace.Save(...)`. Parameter must be named `eSpace`.
- `imports` (string[], required; `[]` is fine) — extra namespaces, **`OutSystems.Model`
  or its sub-namespaces only**; anything else is rejected with `Import 'X' is not
  allowed`. The host prepends `OutSystems.Model.UI`, `OutSystems.Model.UI.Web`,
  `OutSystems.Model.UI.Mobile` and dedup-merges your additions; the sidecar also
  pre-imports `System`, `System.Linq` and the core Model API namespaces. See
  [`../SKILL.md`](../SKILL.md) § 4 and § 1.1 (sandbox).

**Response:**

| Field | Type | Meaning |
|---|---|---|
| `exceptionMessage` | string | Runtime error text; non-empty means the code compiled and ran but threw, so nothing was saved |
| `stdoutOutput` | string | Captured stdout (your code can't write to it — `Console` is sandboxed out; use the throw-probe) |
| `stderrOutput` | string | Captured stderr — host log lines such as `[oml-tool][Information] Action Flow auto arrange completed` |
| `validationMessages` | `ValidationMessage[]` | TrueChange errors/warnings against the resulting model. Non-empty does NOT mean the save failed — filter on `type` equal to `"Error"` |
| `mutatedOmlPath` | string | Path to the saved OML on a clean run; `""` when nothing was saved. Pass to `omlMerge` (the default closing step) |

Each `ValidationMessage`:

| Field | Type | Meaning |
|---|---|---|
| `id` | string | Identifier of the validation rule |
| `type` | string | `"Info"`, `"Warning"` or `"Error"` (capitalised — compare case-insensitively) |
| `message` | string | Short description |
| `detail` | string | Long form |
| `ownerKey` | string | Key of the element the message is about |
| `ownerPath` | string | Readable path to that element |
| `ownerType` | string | That element's kind |

There is still **no** `hasValidationErrors` or `validationErrorCount` field, but
you no longer scan text for it — filter `validationMessages` on
`type` equal to `"Error"`. Runtime failures
surface through `exceptionMessage` (or as an MCP tool error such as
`OML_TOOL_VERB_FAILURE` if the sidecar returned a JSON-RPC error envelope).
**There is no `compilationErrors` field** — code that doesn't compile never
runs, so it returns an MCP tool error (`OML_TOOL_CODE_REJECTED`) carrying the
diagnostics instead of a result. The session pointer advances only when the out
file was written and `exceptionMessage` is empty.

## Read tools

The code-returning reads return a **string** of Model API C# code (the same
dialect you emit into `applyModelApiCode`'s `code` — pattern-match it). The
name-list reads return a **string** of JSON. `runQuery`, `getValidationMessages`,
and `listApps` return structured objects.

### Data model & structures

| Tool | Args | Returns |
|---|---|---|
| `getDataModel` | `includeJson:"Never"` | Entities defined here + entities reachable through references |
| `getStructures` | `includeJson:"Never"` | All structures and their attributes |

### Logic & events

| Tool | Args | Returns |
|---|---|---|
| `getActionNames` | `objectType:"client"\|"server"\|"service"` | JSON array of top-level action names |
| `getServerAction` | `objectName`, `includeJson:"Never"` | One server action with its flow nodes |
| `getClientAction` | `objectName`, `includeJson:"Never"` | One client action |
| `getServiceAction` | `objectName`, `includeJson:"Never"` | One service action |
| `getEvents` | `includeJson:"Never"` | Global events defined in this module |
| `getEventHandlers` | `includeJson:"Never"` | Global event handlers |
| `getTimers` | `includeJson:"Never"` | Timers defined in this module |
| `getIntegrations` | `includeJson:"Never"` | Integrations (REST / SOAP / SAP) defined or consumed |

> The parameter is `objectType` (not `actionType`); it comes from the sidecar's
> `GetModelObjectNameByTypeInput` record.

### UI (Reactive / Mobile)

These UI reads are **Mobile-bound** in the sidecar (they walk
`eSpace.MobileFlows`) and return empty / "not found" on Traditional modules —
use the `*Traditional` variants below when `listApps.moduleType` is `"Traditional"`
or `"TraditionalLibrary"`.

| Tool | Args | Returns |
|---|---|---|
| `getScreenNames` | — | JSON array of screen names |
| `getScreen` | `objectName`, `includeJson:"Never"` | One screen with layout, widgets, screen actions |
| `getWebBlockNames` | — | JSON array of web-block names |
| `getWebBlock` | `objectName`, `includeJson:"Never"` | One web block |
| `getThemeNames` | — | JSON array of theme names |
| `getTheme` | `objectName`, `includeJson:"Never"` | One theme |
| `getEmailTemplateNames` | — | JSON array of email-template names |
| `getEmailTemplate` | `objectName`, `includeJson:"Never"` | One email template |
| `getExternalSiteNames` | — | JSON array of external-site names |
| `getExternalSite` | `objectName`, `includeJson:"Never"` | One external site |

### UI (Traditional Web variants)

Local variants that walk `eSpace.WebFlows` and bind to the Web interfaces.
The single-object variants return **Model API C# code** (same dialect as the
Reactive/Mobile verbs). `getExternalSite*Traditional` bind to the
fully-qualified `OutSystems.Model.UI.Web.IExternalSite` (both Mobile and Web
namespaces define an `IExternalSite`).

| Tool | Args | Returns |
|---|---|---|
| `getScreenNamesTraditional` | — | JSON array of screen names (with `UIFlow`) |
| `getScreenTraditional` | `objectName`, `includeJson:"Never"` | One Traditional screen as Model API code |
| `getWebBlockNamesTraditional` | — | JSON array of web-block names |
| `getWebBlockTraditional` | `objectName`, `includeJson:"Never"` | One web block as Model API code |
| `getThemeNamesTraditional` | — | JSON array of theme names (no `UIFlow`) |
| `getThemeTraditional` | `objectName`, `includeJson:"Never"` | One theme as Model API code |
| `getEmailTemplateNamesTraditional` | — | JSON array of email-template names |
| `getEmailTemplateTraditional` | `objectName`, `includeJson:"Never"` | One email template as Model API code |
| `getExternalSiteNamesTraditional` | — | JSON array of external-site names |
| `getExternalSiteTraditional` | `objectName`, `includeJson:"Never"` | One external site as Model API code |

### Configuration / metadata

| Tool | Args | Returns |
|---|---|---|
| `getSettings` | `includeJson:"Never"` | App settings (a.k.a. SiteProperties — see `model-api-tips.md`) |
| `getRoleNames` | — | JSON array of role names |
| `getRole` | `objectName`, `includeJson:"Never"` | One role |
| `getRoleExceptions` | `includeJson:"Never"` | Role-related exceptions |
| `getUserExceptions` | `includeJson:"Never"` | User-defined exceptions |
| `getException` | `objectName`, `includeJson:"Never"` | One exception by name |
| `getResourceNames` | — | JSON array of resource names |
| `getResource` | `objectName`, `includeJson:"Never"` | One resource |
| `getImageNames` | — | JSON array of image names |
| `getImage` | `objectName`, `includeJson:"Never"` | One image |
| `getScriptNames` | — | JSON array of JavaScript script names |
| `getScript` | `objectName`, `includeJson:"Never"` | One script |
| `getLocales` | `includeJson:"Never"` | Locales configured for the module |

### State (per-session storage)

| Tool | Args | Returns |
|---|---|---|
| `getClientVariables` | `includeJson:"Never"` | Client variables (per-session client-side storage) |
| `getSessionVariables` | `includeJson:"Never"` | Session variables (per-session server-side storage) |

### Query / validation / serialization

| Tool | Args | Returns |
|---|---|---|
| `runQuery` | `query:string` (ModelQL; starts with `Root { … }`, selection sets mandatory; e.g. `Root { ServerActions { Name } }`), optional `executionOptions.includeTypeName:bool` | `QueryESpaceOutput` — matching model elements as JSON |
| `getValidationMessages` | optional `filter:string[]` (subset of `["info","warning","error"]`; empty/null = all; returned `type`s are capitalised) | `{ validationMessages: ValidationMessage[] }` — TrueChange validation against the current pointer |
| `getSerializedObject` | — | Raw JSON serialization of an object |
| `getSerializedObjectByKey` | `objectKey:string` | Object fetched by key, serialized as JSON |
| `getSerializedObjectByNameAndType` | `objectName:string`, `objectType:string` (fully-qualified model interface, e.g. `OutSystems.Model.Data.IServerEntity`) | Object fetched by name+type, serialized as JSON |

> `getValidationMessages` replaces the old
> `oml_get_true_change_errors_and_warnings`.

## Native host tools

| Tool | Args | Returns |
|---|---|---|
| `createSessionToken` | `clientName?` (top level, no `input`) | the token string — call once per agent session (see "Session token" above) |
| `listApps` | only `sessionToken` | `{ apps: [{ key, name, kind:"eSpace", version, isOpen, isReference, moduleType }] }` — open module + loaded references. `moduleType` (formerly `style`) ∈ `Reactive`, `Mobile`, `Traditional`, `ReactiveLibrary`, `MobileLibrary`, `TraditionalLibrary`, `Service`, `Unknown` (see [`../SKILL.md`](../SKILL.md) § 4.1) |

> **`searchModel` was removed** from the MCP host (no cross-element substring
> search tool exists anymore). Find elements by name via the matching
> `get*Names` list tool (or `getDataModel`/`getStructures`) and filter
> client-side, or write a targeted `runQuery` query instead.

## Host-only tools

| Tool | Args | Returns |
|---|---|---|
| `omlReset` | `eSpaceName:string` | `{ discardedChanges:bool, eSpaceName:string }` — throws away that module's unmerged chain; `discardedChanges:false` means there was none and nothing was done (not a failure). The discarded OML is **not deleted**, so an accidental reset is undone by passing the last apply's `mutatedOmlPath` to `omlMerge` (until the next Studio startup clears the temp folder). `MODULE_NOT_LOADED` if the module isn't open (see [`mcp-session-pointer.md`](mcp-session-pointer.md)) |
| `omlRefreshReferences` | `eSpaceName:string` (send `sessionToken` too, although the schema omits it) | `{ message, hasErrors:bool, hasWarnings:bool }` — refreshes every reference of the **live** module to its latest version (Manage Dependencies → Refresh All); does not add new dependencies. **Drops the session pointer** — an unmerged chain vanishes from reads (its `-out.oml` stays on disk). Verified 2026-09-30 |
| `omlPublish` | `eSpaceName:string` (send `sessionToken` too, although the schema omits it) | 1-Click Publish of the live module, waits for the result → `{ published:bool, status:"success"|…, reason:string, messages:["Uploading","Compiling","Deploying","Done"] }` (verified 2026-10-03, 16 s for a small module). The module must be open in Service Studio, otherwise it fails with "Module 'X' is not open in Service Studio". Needs the user's "allow automatic merge and publish" permission in the MCP Server dialog; while it is withheld it returns at once with `{ published:false, status:"blocked", reason:"Publishing requires a write permission the user has not granted. …", messages:[] }` and publishes nothing (verified 2026-10-03). **Only on explicit user request.** |
| `omlMerge` | `eSpaceName:string`, `mutatedOmlPath:string` | `{ merged:bool, message:string }` — merges the supplied OML into the named module and blocks until that resolves, then **clears that module's session pointer** so the next call re-snapshots the live module. Always check `merged` to verify the merge completed; on `true` the change is already in the module, so don't re-call with the same path (a successful merge deletes it, so a repeat fails with file-not-found regardless of whether the first call worked). Pass the `mutatedOmlPath` returned by the last successful `applyModelApiCode`; it's the default closing step (see [`mcp-session-pointer.md`](mcp-session-pointer.md)) |

> **The merge mode is the user's, not yours.** Whether `omlMerge` opens the
> Compare-and-Merge window for the user to review or accepts the changes with no
> dialog is a Service Studio setting. There is no argument for it.
>
> **Envelope note.** Like every tool except `listApps`, these take their arguments
> inside `input` — e.g. `{ "input": { "eSpaceName": …, "mutatedOmlPath": … }, "sessionToken": … }`.

## Common usage pattern

When responding to a "modify the OML to do X" request:

0. **Once per session:** `createSessionToken`, then send every call as `{ "input": {…}, "sessionToken": … }`.
1. **Read the existing state** with the matching read tool (`getDataModel`,
   `getScreen`, …). Output is C# in the dialect you'll emit. Pass the required
   args (`includeJson:"Never"`, `objectName` for single-element reads).
2. **Pattern-match** against that C# to see how existing elements are
   constructed (entity types, key minting, attribute setup, node placement).
3. **Compose a full `eSpace => { … }` lambda** (no save). The
   [`../examples/`](../examples/) files are already in exactly this shape —
   full `eSpace => { … }` lambdas with no `eSpace.Save` — so you can use one
   verbatim as `code` and lift its `using` directives into `imports`.
4. **Call `applyModelApiCode`** with `code` (+ any non-default `imports`). Don't
   pass `omlPath` — the host injects it.
5. **Inspect the response:**
   - `exceptionMessage` populated (or an MCP tool error) → runtime issue; fix
     and retry; the pointer didn't advance.
   - An MCP tool error carrying `compilationErrors:` lines (no result at all) →
     syntax / wrong API name; fix and retry; the pointer didn't advance.
   - Empty `exceptionMessage` and no `type: "Error"` in `validationMessages`
     → the pointer advanced; the next read reflects your change.
   - `validationMessages` has a `type: "Error"` entry → saved-but-invalid;
     fix it.
   - Clean response **but a follow-up read doesn't show your change** → the
     silent-no-op trap (lambda never referenced `eSpace`). See
     [`lambda-contract.md`](lambda-contract.md).
6. **Re-read** the modified state to confirm the mutation landed (the pointer
   makes this transparent — same tool, same args).
7. **Finalise with `omlMerge`:** after the last successful `applyModelApiCode`,
   call `omlMerge` with its `mutatedOmlPath` to bring the change into the module
   — the default closing step of any mutating task. Check `merged` in the
   response. Skip only when nothing was saved
   (`mutatedOmlPath: ""`), the task was read-only, or the user asked you to
   discard the chain (`omlReset`). See
   [`mcp-session-pointer.md`](mcp-session-pointer.md).

The recovery loop in step 5 is the dominant teaching signal — see
[`../integration-examples/02-recovery-from-compile-error.md`](../integration-examples/02-recovery-from-compile-error.md).
