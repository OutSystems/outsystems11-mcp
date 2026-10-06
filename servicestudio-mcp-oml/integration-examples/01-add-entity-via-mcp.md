# 01 — Add a `Customer` entity via MCP

**Transport**: MCP (Service Studio in-process MCP Server at `http://127.0.0.1:41820/mcp`).
**Scenario**: Add a `Customer` entity to the module open in Service Studio with a single `Name` text attribute (200 chars), invoked through the MCP tools. This is the **canonical path** for a Claude Code agent talking to a live Service Studio session — one MCP tool call per logical step, with the host taking care of snapshotting the open module, allocating the output path, spawning the bundled sidecar per call, and chaining successive calls through its session pointer. You supply a **full `eSpace => { ... }` lambda** in `code`; the host appends `eSpace.Save(...)` for you.

The session-pointer mechanics referenced below are documented in [`../reference/mcp-session-pointer.md`](../reference/mcp-session-pointer.md). The full MCP tool catalogue lives at [`../reference/verb-reference.md`](../reference/verb-reference.md). The lambda-shape contract is in [`../reference/lambda-contract.md`](../reference/lambda-contract.md).

---

## Wire shape — MCP tool call, structured-content response

```
agent → MCP server   call tool (e.g. applyModelApiCode) with { input: { …tool args… }, sessionToken }
MCP   ↻ host         snapshot open module → %TEMP%\ServiceStudio.MCPServer\<runId>-in.oml
                     take the agent's full lambda, strip the trailing }, append
                     eSpace.Save("<runId>-out.oml"); and re-close the lambda
                     run the bundled sidecar (stdio JSON-RPC), inject the in/out paths
MCP   → agent        result object as structured content (no JSON-RPC ok / result envelope)
```

Every call observes the host-managed session pointer. The **first** call after a fresh Service Studio session (or after `omlReset`) snapshots the live module; subsequent calls reuse the last-good output. The agent never threads `omlPath` / `outPath` itself; on a clean save the apply response returns the saved path as `mutatedOmlPath` for you to hand to `omlMerge`.

## User message

> Add a `Customer` entity to the module I have open in Service Studio with a single `Name` text attribute (200 chars).

## Turn

The agent obtains a session token, runs three sequential MCP tool calls, then surfaces the change via the IDE.

### Call 0 — `createSessionToken`

Once per agent session, before anything else. Service Studio shows an approval dialog; when the user approves, the result text is the token (a GUID string). Reuse it for every later call — including after a reconnect — and never request a second one.

**Request** (call tool `createSessionToken`):

```jsonc
{ "clientName": "Claude Code" }
```

**Response** (result text):

```
<token>
```

Every call below passes it as the top-level `sessionToken` argument, and every tool except `listApps` wraps its own arguments in an `input` object.

### Call 1 — `getDataModel`

Read the existing data model. **`includeJson` is a required arg** on code-returning read tools (it comes from the sidecar's `GetModelObjectCodeInput.IncludeJson`) — always pass `"Never"`. Like every tool except `listApps`, it goes inside `input` alongside `eSpaceName`.

**Request** (call tool `getDataModel` — the host snapshots the open module on this first call):

```jsonc
{ "input": { "eSpaceName": "MyModule", "includeJson": "Never" }, "sessionToken": "<token>" }
```

**Response** (MCP structured content — no envelope; formatted here for readability):

```csharp
(OutSystems.Model.IReference _System_) => {
    /*** creating IStaticEntity 'MenuItem' ***/
    var menuItem = testBasicTypesWithou.CreateStaticEntity("MenuItem");
    /* ... */
}
```

The agent pattern-matches against this — note the call shapes
(`CreateStaticEntity`, `CreateAttribute`, `DataType = ...`), **not** the
verbose receiver names (`testBasicTypesWithou`) or the lambda head
(`(OutSystems.Model.IReference _System_) =>`). Those belong to the read
verb's rendering and are irrelevant when emitting your own lambda.

### Call 2 — `applyModelApiCode`

Submit a **full `eSpace => { ... }` lambda**. Do **not** include an `eSpace.Save(...)` line — the host appends that for you. The `code` must end in `}` with no trailing whitespace, newline, or semicolon (the host removes the last character expecting it to be the closing brace, then re-closes the lambda after injecting `Save`).

**Request** (call tool `applyModelApiCode`):

```jsonc
{
  "input": {
    "eSpaceName": "MyModule",
    "code":    "eSpace => { var key = eSpace.Key.CreateNewKeyBasedOnThis(new object[] { \"Customer\" }); var customer = eSpace.CreateServerEntity(\"Customer\", key); var nameKey = key.CreateNewKeyBasedOnThis(new object[] { \"Name\" }); var name = customer.CreateAttribute(\"Name\", nameKey); name.DataType = eSpace.TextType; name.Length = 200; }",
    "imports": []
  },
  "sessionToken": "<token>"
}
```

What the host literally hands to the sidecar — your lambda with its trailing `}` replaced by the injected `Save` and a re-closing brace:

```csharp
eSpace => {
var key = eSpace.Key.CreateNewKeyBasedOnThis(new object[] { "Customer" });
var customer = eSpace.CreateServerEntity("Customer", key);
var nameKey = key.CreateNewKeyBasedOnThis(new object[] { "Name" });
var name = customer.CreateAttribute("Name", nameKey);
name.DataType = eSpace.TextType;
name.Length = 200;
eSpace.Save("C:\\Users\\…\\<runId>-out.oml");
}
```

**Response**:

```jsonc
{
  "exceptionMessage":   "",
  "stdoutOutput":       "",
  "stderrOutput":       "",
  "validationMessages": [],
  "mutatedOmlPath":     "C:\\Users\\…\\<runId>-out.oml"
}
```

A clean `exceptionMessage` plus a non-empty `mutatedOmlPath` ⇒ the new entity has been saved. The host advanced its session pointer to that path, so the next call reads from there automatically — and `mutatedOmlPath` is the path you hand to `omlMerge` to surface the change (see Call 4).

### Call 3 — verify

Re-read the data model to confirm the entity landed.

**Request** (call `getDataModel` again — `includeJson` still required, still forced to `"Never"`; the host uses the pointer from Call 2):

```jsonc
{ "input": { "eSpaceName": "MyModule", "includeJson": "Never" }, "sessionToken": "<token>" }
```

**Response** (excerpt):

```csharp
... var customer = testBasicTypesWithou.CreateServerEntity("Customer");
... var name = customer.CreateAttribute("Name");
name.DataType = testBasicTypesWithou.TextType;
name.Length = 200;
```

The `Customer` entity is present in the post-mutation data model. Done.

### Call 4 — auto-merge into Service Studio

This is the default closing step. Take the `mutatedOmlPath` from the last successful `applyModelApiCode` (Call 2) and call `omlMerge` with it — do it without being asked:

```jsonc
// call tool omlMerge — last action of any mutating task
{ "input": { "eSpaceName": "MyModule", "mutatedOmlPath": "C:\\Users\\…\\<runId>-out.oml" }, "sessionToken": "<token>" }
```

```jsonc
{ "merged": true, "message": "..." }
```

Service Studio merges the mutated OML into the open module. Depending on the user's setting, that either opens the Compare-and-Merge window for them to review and accept (or reject) the diff, or is accepted with no dialog; `merged` in the response tells you how it ended. Skip this only for read-only tasks or when every apply returned `mutatedOmlPath: ""`. See [`../SKILL.md`](../SKILL.md) § 3.1.

---

## What just happened

One token request and four MCP tool calls against the in-process MCP Server:

0. `createSessionToken` — once per agent session; the user approves it in Service Studio, and the token rides on every later call as `sessionToken`.
1. `getDataModel` (with `includeJson: "Never"`) — learn the dialect and entity-creation patterns. The host snapshotted the open module on this first call.
2. `applyModelApiCode` — submit a **full `eSpace => { ... }` lambda**. The host stripped the trailing `}`, appended `eSpace.Save(...)`, re-closed the lambda, ran it against the snapshot via the sidecar, saved to a host-allocated path, and advanced the session pointer.
3. `getDataModel` — verify. The host transparently read from the pointer set by call 2 — no agent-supplied path.
4. `omlMerge` — the agent passes the `mutatedOmlPath` from Call 2 to merge the change into the open module, either through Service Studio's Compare-and-Merge window or with no dialog, per the user's setting.

There is **no per-call process spawn** the agent has to engineer, no input-stream formatting, no exit-code parsing. The MCP transport delivers the result object directly as structured content; the host handles snapshotting, lambda finalisation, path injection, sidecar spawning, and pointer advancement.

## Things to remember

- **Envelope: `{ "input": { … }, "sessionToken": "<token>" }`** on every tool except `listApps` (which takes only `sessionToken`). `applyModelApiCode`'s `input` takes `eSpaceName`, `code` (a full lambda) and `imports` — all three required (`[]` when you need none). Code-returning read tools take `eSpaceName` + `includeJson` (always `"Never"`); single-element variants additionally take `objectName`. **Never** pass `omlPath` / `outPath` — the host manages them.
- **`code` is a full lambda.** Pass `eSpace => { ...statements... }`; do **not** include `eSpace.Save(...)`. The parameter must literally be `eSpace`, and the string must end with `}` (no trailing whitespace/newline/semicolon) — the host removes the last character (see [`../reference/lambda-contract.md`](../reference/lambda-contract.md)).
- **Success signal.** A clean (empty) `exceptionMessage` plus a non-empty `mutatedOmlPath` ⇒ the chain advanced and the next call picks up automatically. A populated `exceptionMessage` (or an MCP tool error) ⇒ the pointer is unchanged and `mutatedOmlPath` is `""`, so the next call resumes from the last-good state (or the live module if there was no prior success).
- **Default imports are generous.** The host adds 3 (`OutSystems.Model.UI`, `OutSystems.Model.UI.Web`, `OutSystems.Model.UI.Mobile`); the sidecar pre-imports 7 (`System`, `System.Linq`, `OutSystems.Model`, `.Enumerations`, `.Expressions`, `.Factory`, `.Types`). All 10 are in scope without listing them in `imports`. `imports` itself may only name `OutSystems.Model` or its sub-namespaces — the host rejects anything else.
- **`getDataModel`'s output uses verbose param names** like `(OutSystems.Model.IReference _System_) =>` and a body that references `testBasicTypesWithou` (the truncated module name). **Don't copy those names into your `applyModelApiCode` lambda** — use the literal identifier `eSpace`. Pattern-match on the *shape* of the calls (`CreateServerEntity`, `CreateAttribute`, `DataType = eSpace.TextType`), not on the receiver name.
- **The `examples/*.cs` files are already full `eSpace => { ... }` lambdas with no `eSpace.Save`** — exactly the shape `code` wants. Use one verbatim as `code` and lift its leading `using` directives into `imports` (only `OutSystems.Model.*` ones are allowed; fully qualify anything else in the code).
- **For multi-step edits**, see [`03-multi-step-edit.md`](03-multi-step-edit.md). The session pointer makes chaining transparent.
- **To deliberately drop the chain** (e.g. the user switches direction mid-task), see [`04-oml-reset.md`](04-oml-reset.md).
- **Finalising mutating tasks.** After the last successful mutation, call `omlMerge` with the `mutatedOmlPath` from the apply response to bring the change into the open module — the default closing step. Check `merged` in the response.
