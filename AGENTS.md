# OutSystems 11 OML editing via the Service Studio MCP server

> **Which task are you doing?** This repository serves two different jobs, and
> they need opposite instructions:
>
> - **Driving a live O11 module** — creating entities, screens or actions in the
>   module currently open in Service Studio. **This file is your contract; read
>   on.**
> - **Changing this repository** — editing `SKILL.md`, `reference/`, `examples/`
>   or the harness adapters. Then you are editing *documentation about* the
>   contract, not exercising it, and there is no live module involved. Read
>   [`CONTRIBUTING.md`](CONTRIBUTING.md) and [`CLAUDE.md`](CLAUDE.md) instead.

When **using** this skill you author OutSystems 11 apps by mutating the
**currently open `.oml` module** through Service Studio's in-process MCP server
(`servicestudio`, `http://127.0.0.1:41820/mcp`). There is no source tree, build,
or test runner involved in that work — it is design-time OML mutation via the
`applyModelApiCode` tool.

> **Beta Feature.** The write tools (mutating `applyModelApiCode` lambdas, `startOmlEditSession`,
> `endOmlEditSession`, `omlMerge`, `omlReset`, `omlRefreshReferences`, `omlPublish`) are a Beta Feature. The first time you use
> one in a conversation, tell the user and share https://www.outsystems.com/legal/beta-features-agreement. Read tools are Generally Available.

Full contract and reference material live in **`servicestudio-mcp-oml/`**. Consult
`servicestudio-mcp-oml/SKILL.md` and the `servicestudio-mcp-oml/reference/*.md` files —
read the one that fits the task; don't load them all at once. The rules below are a
condensed restatement of `SKILL.md`, which is the source of truth if the two ever
disagree.

## Contract check, session token and argument envelope

- **Check the MCP contract first.** This skill is **written for MCP contract 2**. The first
  line of the server's instructions says `Service Studio <version>, MCP contract <N>.` If N is
  higher, the second line says where to get the latest skill: tell the user, offer to install
  it, and install it only if they agree. If N is lower or the line is missing (contract 1),
  follow `servicestudio-mcp-oml/reference/contracts.md`. On any contract, where a tool's live
  description differs from this skill, follow the description. If you can't see the instructions,
  `createSessionToken`'s description says the same; neither saying anything means contract 1.
- **Call `createSessionToken` exactly once, first** (`{ "clientName": "…" }`).
  The user approves a dialog in Service Studio; the result text is the token.
  The call waits for that approval with no signal to you, so tell the user to
  approve the request in Service Studio before you call it.
  Pass it as a top-level `sessionToken` on every later call and reuse it across
  reconnects — never request a second one.
- **Every tool except `listOpenModules` wraps its arguments in `input`:**
  `{ "input": { … }, "sessionToken": "<token>" }`. `listOpenModules` (`listApps`
  on early contract 1 builds) is just `{ "sessionToken": "<token>" }`. There is no flat shape. Examples:
  `applyModelApiCode` → `input: { eSpaceName, code, imports }` (all three
  required); `omlMerge` → `input: { eSpaceName, mutatedOmlPath }`; `omlReset`,
  `startOmlEditSession` and `endOmlEditSession` → `input: { eSpaceName }`. (`omlRefreshReferences`, `omlPublish` and `getRoleExceptions` take
  `input: { eSpaceName }`; send the token to them too, even though their schemas
  don't declare it.)
- **Summarise a module with one probe**, not one getter per element:
  `servicestudio-mcp-oml/examples/ProbeModuleInventory.cs` returns the whole inventory in a
  few thousand characters. `getDataModel` on a large module can exceed 100k. The probe
  throws before saving, so use it even when the user says not to change anything. It
  still needs an edit session: start one, probe, end it, and give the Beta notice for
  those calls. Don't probe inside a session you didn't start (SKILL.md § 2).
- **Keep the developer's layout when you edit an existing flow.** Shift the nodes below the
  insertion point and place the new node in the gap. Use the full layout pass only on flows you
  created (SKILL.md § 3.1).
- **Address a module by its `tabId`.** Call `listOpenModules` and pass an open row's
  `tabId` as `eSpaceName`. Its `key` or `name` also work, but one open in several tabs
  fails with `MODULE_AMBIGUOUS`: retry with the `tabId` of the module the user means.
  `isSelected` marks "this module"; when several rows have it, ask. The list doesn't say
  which environment a module came from, and each module publishes to its own server
  (SKILL.md §§ 0.1, 8).
- **A module that isn't open:** find it with `searchApps`, and open it with `openModule`
  (`moduleKey` = its key) only when the user's request names it. When several match, ask
  and open nothing. When Studio's windows are logged in to several environments, both
  fail with `ENVIRONMENT_REQUIRED`: ask the user which one and pass it as `environment`
  to both. Use the returned `module.tabId`; on `opening`, call again with the same
  arguments. Close the
  modules you opened with `closeModule` when you are done, never one the user opened, and
  tell the user before closing a merged module, because Studio will ask them to save.
  Early contract 1 builds (`listApps`) have none of these tools and address modules by `name`:
  ask the user to open the module. Later contract 1 builds: `servicestudio-mcp-oml/reference/contracts.md`.

## Non-negotiable rules for `applyModelApiCode`

- **Writes need an edit session.** Call `startOmlEditSession` before the first
  `applyModelApiCode` on a module; without it the apply fails with `No OML edit session
  is started`. A merge doesn't end the session. Call `endOmlEditSession` once the last
  merge returned `merged: true` (SKILL.md § 2.1).
- **Outside an edit session, read one module one call at a time.** Parallel reads can
  fail with `OML not found: …-in.oml`; run the failed read again on its own (SKILL.md § 2).
- The `code` arg is a **full `Action<IESpace>` lambda**: `eSpace => { … }`.
  - The parameter must be literally **`eSpace`**.
  - **Never** call `eSpace.Save(...)` — the host appends its own save.
  - `code` must **end with `}`** and nothing after it (no trailing newline,
    space, or `;`).
- **Sandbox:** `code` may only reference the Model API. `Console`, anything
  written `System.…`, and reflection (`GetProperties`, `GetMethod`, `Invoke`, …)
  are rejected before compile with a tool error. Unqualified BCL basics
  (`Exception`, `List<T>`, LINQ, `string.Join`, `GetType().Name`) are fine.
- **Probe by throwing, not printing:** build a string and end the lambda with
  `throw new Exception("PROBE:" + s);`. The text comes back in
  `exceptionMessage`; nothing is saved, the pointer does not move, and
  `validationMessages` for the in-memory model are still returned.
- **Read before you write:** call `getDataModel` (or the matching `get*`) first
  and pattern-match its output. Code-returning reads require `includeJson`
  (always pass `"Never"`).
- **Edit in place; never replay read-back.** `get*` output is *creation* code
  (`eSpace.CreateServerAction("X")` …). Replaying it creates a silently renamed
  duplicate (`X2`, outside its folder) that TrueChange doesn't flag. Look the
  element up (`eSpace.ServerActions.Named("X")` finds actions in folders too) and
  mutate it.
- **Don't silently substitute.** If the widget or element you were asked for
  won't create (for example a Traditional TableRecords), fix the error or report
  it. Never quietly build something else instead.
- **A clean response is not proof of mutation.** Verify with a follow-up `get*`
  (guard against the *silent no-op*; read back `.Name` too — taken or reserved
  names are silently renamed, e.g. `GetOrder` → `GetOrder2`), and scan
  `validationMessages` for any entry whose `type` is `"Error"` (values are
  capitalised — compare case-insensitively) to catch *saved-but-invalid*.
  Runtime errors surface in `exceptionMessage`; code that does not compile never
  runs and comes back as an MCP tool error (`Script compilation failed` +
  `compilationErrors:` lines), not as a result; success is a non-empty
  `mutatedOmlPath`.
- **Empty everything = runner process-crash.** If `exceptionMessage` is empty,
  `validationMessages` is `[]`, **and** `mutatedOmlPath` is `""`, an unsupported
  API member killed the sidecar. Recover by **bisecting**: probe with
  `eSpace => { throw new Exception("PROBE:ok"); }` (`PROBE:ok` coming back proves
  the session is healthy), then re-issue with one risky member per call. Known
  crash members: `servicestudio-mcp-oml/reference/reactive-widget-api.md`.
- **Finalise with `omlMerge`:** after the final successful mutation, call
  `omlMerge` with `input: { eSpaceName, mutatedOmlPath }` to open Service
  Studio's Compare-and-Merge window, then `endOmlEditSession`. Skip the merge only for
  read-only tasks or when nothing saved.
- **`merged: false` ("The merge failed. See the log for details.")**: the reason is only in a Service Studio dialog. Ask the user what it says, `endOmlEditSession` and `startOmlEditSession` again, re-apply without the cause, and merge the new path. Never set `Theme.IconLibrary`: contract 1 builds can reject the whole merge (`servicestudio-mcp-oml/reference/contracts.md`). Editing the theme stylesheet is fine, and the sidecar's `InvalidIconLibrary` error it causes is expected; merge anyway (SKILL.md § 3.1).
- **Call `omlRefreshReferences` outside an edit session.** Inside one it ends a
  session with unmerged edits, or leaves the working copy without the refreshed
  references (SKILL.md § 3.2).
- **Call `omlPublish` only when the user explicitly asks to publish.** It needs a
  write permission granted in the MCP Server dialog and returns status
  `blocked` while that is withheld.
- **If *every* tool call fails with `An error occurred invoking '<toolName>'`,
  suspect your harness, not your lambda.** When the failure is identical across
  tools and arguments, and your client is adding argument fields you never sent
  (`inputXXXNameValPairs`-style names), the tool wrapper is mangling the
  envelope. Don't rewrite working code. The documented fallback — posting to
  `http://127.0.0.1:41820/mcp` directly — is in
  `servicestudio-mcp-oml/reference/verb-reference.md` § "HTTP transport". If you
  take it, **persist the `sessionToken` to a file** (in the OS temp dir, *not*
  the repo — it's an authorization token) and read it back in each script, so
  one-shot invocations don't call `createSessionToken` (and prompt the user)
  again.
- `OML_TOOL_HOST_FAILURE` / `-32099` means no module is open in a signed
  Service Studio — an environment issue, not a code bug. Don't retry the lambda.

## Imports

Host pre-imports `OutSystems.Model.UI(.Web/.Mobile)`, `System`, `System.Linq`,
`OutSystems.Model(.Enumerations/.Expressions/.Factory/.Types)`. `imports` may
only add **`OutSystems.Model` or its sub-namespaces** — e.g.
`OutSystems.Model.Logic` + `OutSystems.Model.Logic.Nodes` for flow nodes,
`OutSystems.Model.Data` for entity creation. Anything else (`System.Text`,
`ServiceStudio.Plugin.NRWidgets`, …) is rejected; write plugin types fully
qualified in code instead (`ServiceStudio.Plugin.NRWidgets.IInput`).

## Module styles

`listOpenModules` reports each module's `moduleType` (e.g. `"Reactive"`).
`applyModelApiCode` is style-agnostic, but for **Traditional** modules use the
`*Traditional` read verbs (`getScreenTraditional`, etc.) and `eSpace.WebFlows`
instead of `eSpace.MobileFlows`.

## Security

`applyModelApiCode` runs arbitrary caller-authored C# with Service Studio's
privileges. Use it only against modules you own; be cautious with code derived
from untrusted module content.
