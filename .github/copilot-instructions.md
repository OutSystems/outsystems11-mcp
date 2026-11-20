# OutSystems 11 OML editing via the Service Studio MCP server

This workspace authors OutSystems 11 apps by mutating the **currently open
`.oml` module** through Service Studio's in-process MCP server (`servicestudio`,
`http://127.0.0.1:41820/mcp`). There is no source tree, build, or test runner —
work is design-time OML mutation via the `applyModelApiCode` tool.

> **Beta Feature.** The write tools (mutating `applyModelApiCode` lambdas, `startOmlEditSession`,
> `endOmlEditSession`, `omlMerge`, `omlReset`, `omlRefreshReferences`, `omlPublish`) are a Beta Feature. The first time you use
> one in a conversation, tell the user and share https://www.outsystems.com/legal/beta-features-agreement. Read tools are Generally Available.

Full contract and reference material live in **`servicestudio-mcp-oml/`**. Consult
`servicestudio-mcp-oml/SKILL.md` and the `servicestudio-mcp-oml/reference/*.md` files
(read the one that fits the task; don't load them all). The rules below are a
condensed restatement; `SKILL.md` wins if they ever disagree.

## Contract check, session token and argument envelope

- **Check the MCP contract first.** This skill is **written for MCP contract 2**. The first
  line of the server's instructions says `Service Studio <version>, MCP contract <N>.` If N is
  higher, the second line says where to get the latest skill: tell the user, offer to install
  it, and install it only if they agree. If N is lower or the line is missing (contract 1),
  follow `servicestudio-mcp-oml/reference/contracts.md`. On any contract, where a tool's live
  description differs from this skill, follow the description. If you can't see the instructions,
  `createSessionToken`'s description says the same; neither saying anything means contract 1.
- **Call `createSessionToken` exactly once, first** (`{ "clientName": "…" }`;
  the user approves a dialog; the result text is the token). Pass it as a
  top-level `sessionToken` on every later call; reuse it, never request another.
- **Every tool except `listOpenModules` wraps its arguments in `input`:**
  `{ "input": { … }, "sessionToken": "<token>" }`; `listOpenModules` (`listApps` on
  early contract 1 builds) is just `{ "sessionToken": "<token>" }`. `applyModelApiCode` →
  `input: { eSpaceName, code, imports }` (all required); `omlMerge` →
  `input: { eSpaceName, mutatedOmlPath }`; `omlReset`, `startOmlEditSession` and
  `endOmlEditSession` → `input: { eSpaceName }`.
- **Pass an open module's `tabId` from `listOpenModules` as `eSpaceName`.** A key or
  name open in several tabs fails with `MODULE_AMBIGUOUS`; retry with the `tabId` the
  user means. A module that isn't open: `searchApps`, then `openModule` with its key,
  only when the user names it (ask when several match). On `ENVIRONMENT_REQUIRED`, ask
  the user which environment and pass it as `environment` to both. Close what you opened with
  `closeModule`, never a module the user opened; before closing a merged module, tell
  the user Studio will ask them to save. Early contract 1 builds (`listApps`) have none of these
  tools: ask the user to open the module (SKILL.md § 0.1, `servicestudio-mcp-oml/reference/contracts.md`).
- **Summarise a module with one probe**
  (`servicestudio-mcp-oml/examples/ProbeModuleInventory.cs`), not one getter per element.
  The probe throws before saving, so use it even when asked not to change anything. It still
  needs an edit session: start one, probe, end it, and give the Beta notice for those calls.
  Don't probe inside a session you didn't start (SKILL.md § 2).
- **Keep the developer's layout when you edit an existing flow.** Shift the nodes below the
  insertion point and place the new node in the gap. Use the full layout pass only on flows
  you created.

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
- **Sandbox:** `code` may only reference the Model API — `Console`, anything
  written `System.…`, and reflection (`GetProperties`, `GetMethod`, `Invoke`, …)
  are rejected before compile. `Exception`, `List<T>`, LINQ, `string.Join`,
  `GetType().Name` are fine.
- **Probe by throwing:** end the lambda with
  `throw new Exception("PROBE:" + s);` — the text returns in `exceptionMessage`,
  nothing is saved, and `validationMessages` are still returned.
- **Read before you write:** call `getDataModel` (or the matching `get*`) first
  and pattern-match its output. Code-returning reads require `includeJson`
  (always pass `"Never"`).
- **Edit in place; never replay read-back.** `get*` output is *creation* code;
  replaying it makes a silently renamed duplicate (`X2`, outside its folder).
  Look the element up (`eSpace.ServerActions.Named("X")`, which finds actions in
  folders) and mutate it. If a requested widget won't create, fix it or report
  it — never silently build a different one.
- **A clean response is not proof of mutation.** Verify with a follow-up `get*`
  (guard against the *silent no-op*; read back `.Name` — taken or reserved names
  are silently renamed, e.g. `GetOrder` → `GetOrder2`), and scan
  `validationMessages` for any `type: "Error"` entry (capitalised — compare
  case-insensitively) to catch *saved-but-invalid*. Runtime errors surface in
  `exceptionMessage`; code that does not compile never runs and comes back as
  an MCP tool error (`Script compilation failed` + `compilationErrors:`), not as
  a result; success is a non-empty `mutatedOmlPath`.
- **Empty everything = runner process-crash.** If `exceptionMessage` is empty,
  `validationMessages` is `[]`, **and** `mutatedOmlPath` is `""`, an unsupported
  API member killed the sidecar. Recover by **bisecting**: probe with
  `eSpace => { throw new Exception("PROBE:ok"); }` (`PROBE:ok` back proves the
  session is healthy), then re-issue with one risky member per call. Known crash
  members: `servicestudio-mcp-oml/reference/reactive-widget-api.md`.
- **Finalise with `omlMerge`:** after the final successful mutation, call
  `omlMerge` with `input: { eSpaceName, mutatedOmlPath }` to open Service
  Studio's Compare-and-Merge window, then `endOmlEditSession`. Skip the merge only for
  read-only tasks or when nothing saved.
- **`merged: false` ("The merge failed. See the log for details.")**: the reason is only in a Service Studio dialog. Ask the user what it says, `endOmlEditSession` and `startOmlEditSession` again, re-apply without the cause, and merge the new path. Never set `Theme.IconLibrary`: contract 1 builds can reject the whole merge (`servicestudio-mcp-oml/reference/contracts.md`). Editing the theme stylesheet is fine, and the sidecar's `InvalidIconLibrary` error it causes is expected; merge anyway (SKILL.md § 3.1).
- **Call `omlRefreshReferences` outside an edit session** — inside one it ends a
  session with unmerged edits, or leaves the working copy without the refreshed
  references (SKILL.md § 3.2). **`omlPublish` only when
  the user explicitly asks to publish** (returns `blocked` until the user grants
  the write permission).
- **The *same* `An error occurred invoking '<toolName>'` on every tool means the
  harness, not your lambda.** If the failure is identical across tools and
  arguments and your client is adding argument fields you never sent
  (`inputXXXNameValPairs`-style), the tool wrapper is corrupting the envelope.
  Don't rewrite working code — see
  `servicestudio-mcp-oml/reference/verb-reference.md` § "HTTP transport".
- `OML_TOOL_HOST_FAILURE` / `-32099` means no module is open in a signed
  Service Studio — an environment issue, not a code bug. Don't retry the lambda.

## Imports

Host pre-imports `OutSystems.Model.UI(.Web/.Mobile)`, `System`, `System.Linq`,
`OutSystems.Model(.Enumerations/.Expressions/.Factory/.Types)`. `imports` may
only add `OutSystems.Model` sub-namespaces — e.g. `OutSystems.Model.Logic` +
`OutSystems.Model.Logic.Nodes` for flow nodes, `OutSystems.Model.Data` for
entity creation. Anything else is rejected; write plugin types fully qualified
in code (`ServiceStudio.Plugin.NRWidgets.IInput`).

## Module styles

`listOpenModules` reports each module's `moduleType`. `applyModelApiCode` is
style-agnostic, but for **Traditional** modules use the `*Traditional` read
verbs (`getScreenTraditional`, etc.) and `eSpace.WebFlows` instead of
`eSpace.MobileFlows`.
