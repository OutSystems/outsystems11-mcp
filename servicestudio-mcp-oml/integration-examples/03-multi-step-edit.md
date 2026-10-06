# 03 — Multi-step edit (auto-chain via the session pointer)

**Transport**: MCP (Service Studio in-process MCP Server).
**Scenario**: User asks for two related mutations — add a `Customer` entity, *then* add an `Email` attribute to it. The key teaching point: **the MCP host auto-chains successive `applyModelApiCode` calls via its per-module session pointer**. Each successful call writes a new out file and advances the pointer to it; the next call observes the pointer and reuses it as its input — invisibly to the agent, which never sees the path.

See [`../reference/mcp-session-pointer.md`](../reference/mcp-session-pointer.md) for the pointer mechanics. All `code` payloads below are **full `eSpace => { ... }` lambdas** that end in `}` and do **not** include `eSpace.Save(...)` — see [`../reference/lambda-contract.md`](../reference/lambda-contract.md).

---

## User message

> Add a `Customer` entity to the module I have open in Service Studio. Then add an `Email` text attribute (length 200, mandatory) to that `Customer` entity.

## Default — auto-chain (two calls, no path threading)

Two separate `applyModelApiCode` calls. The host snapshots the open module on Call 1, advances the pointer on success, and Call 2 transparently reads from that pointer. (Every call carries the `sessionToken` obtained once from `createSessionToken` — see [`01-add-entity-via-mcp.md`](01-add-entity-via-mcp.md) Call 0.)

### Call 1 — create the `Customer` entity

**Request** (call tool `applyModelApiCode` — full lambda, no paths, no `Save`):

```jsonc
{
  "input": {
    "eSpaceName": "MyModule",
    "code":    "eSpace => { var key = eSpace.Key.CreateNewKeyBasedOnThis(new object[] { \"Customer\" }); eSpace.CreateServerEntity(\"Customer\", key); }",
    "imports": []
  },
  "sessionToken": "<token>"
}
```

**Response** (structured content — result object directly):

```jsonc
{
  "exceptionMessage":   "",
  "stdoutOutput":       "",
  "stderrOutput":       "",
  "validationMessages": [],
  "mutatedOmlPath":     "C:\\Users\\…\\<run1>-out.oml"
}
```

Empty `exceptionMessage` + the out file was written ⇒ the host advanced the session pointer to that new file and returned its path as `mutatedOmlPath`.

### Call 2 — add the `Email` attribute

The critical detail under MCP: **the agent does not thread paths between calls** (it never receives them). The host's session pointer already points at Call 1's output, so Call 2 only needs to send `eSpaceName`, `code` and `imports` inside `input`:

```jsonc
{
  "input": {
    "eSpaceName": "MyModule",
    "code":    "eSpace => { var customer = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named(\"Customer\"); var emailKey = customer.Key.CreateNewKeyBasedOnThis(new object[] { \"Email\" }); var email = customer.CreateAttribute(\"Email\", emailKey); email.DataType = eSpace.TextType; email.Length = 200; email.IsMandatory = true; }",
    "imports": ["OutSystems.Model.Data"]
  },
  "sessionToken": "<token>"
}
```

Note: `imports: ["OutSystems.Model.Data"]` because we're using `OutSystems.Model.Data.IServerEntity` for the cast — that namespace is not in the default imports (3 host + 7 sidecar). `imports` accepts only `OutSystems.Model` and its sub-namespaces.

**Response**:

```jsonc
{
  "exceptionMessage":   "",
  "stdoutOutput":       "",
  "stderrOutput":       "",
  "validationMessages": [],
  "mutatedOmlPath":     "C:\\Users\\…\\<run2>-out.oml"
}
```

The pointer advances again.

### Verify

`getDataModel` with the required `includeJson` (always `"Never"`) — the host reads from the latest pointer position:

```jsonc
{ "input": { "eSpaceName": "MyModule", "includeJson": "Never" }, "sessionToken": "<token>" }
```

Excerpt:

```csharp
/*** creating IServerEntity 'Customer' ***/
var customer = testBasicTypesWithou.CreateServerEntity("Customer");
/* ... */
var email = customer.CreateAttribute("Email");
email.DataType = testBasicTypesWithou.TextType;
email.IsMandatory = true;
email.Length = 200;
```

Both mutations landed.

### Finalise — auto-merge into the open module

After the **last** successful `applyModelApiCode` call (here, Call 2), take its `mutatedOmlPath` and call `omlMerge` with it — the default closing step of any mutating task (see [`../SKILL.md`](../SKILL.md) § 3.1). Do it without being asked; only the *final* `mutatedOmlPath` of the chain goes into `omlMerge`, not the intermediate ones.

```jsonc
// call tool omlMerge
{ "input": { "eSpaceName": "MyModule", "mutatedOmlPath": "C:\\Users\\…\\<run2>-out.oml" }, "sessionToken": "<token>" }
```

Service Studio merges that OML into the open module — through its merge window for the user to review and accept (or reject), or with no dialog, per their setting.

---

## Composition — both mutations in one lambda

You can also fold both mutations into a single `applyModelApiCode` call — one lambda whose body builds both elements:

```jsonc
{
  "input": {
    "eSpaceName": "MyModule",
    "code":    "eSpace => { var key = eSpace.Key.CreateNewKeyBasedOnThis(new object[] { \"Customer\" }); var customer = eSpace.CreateServerEntity(\"Customer\", key); var emailKey = key.CreateNewKeyBasedOnThis(new object[] { \"Email\" }); var email = customer.CreateAttribute(\"Email\", emailKey); email.DataType = eSpace.TextType; email.Length = 200; email.IsMandatory = true; }",
    "imports": []
  },
  "sessionToken": "<token>"
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

Reasons to prefer composition:

- The two mutations share local state (here, the `customer` variable from creation flows directly into the attribute creation — no `Named("Customer")` lookup needed, no `OutSystems.Model.Data` import).
- You want exactly one row in the audit log instead of two.
- You're scripting outside an interactive session and don't need intermediate inspection.

Under MCP, composition is mostly stylistic — the session pointer already rolls back to the previous good state on a failed call (no out file written ⇒ pointer unchanged), so atomicity is rarely an issue across two successive calls.

---

## Mid-chain reset

Sometimes you've made one successful change but want to discard it and start over from the live module. Insert an `omlReset` between two `applyModelApiCode` calls:

```jsonc
// Call 1: successful applyModelApiCode — pointer advances
{ "input": { "eSpaceName": "MyModule", "code": "eSpace => { /* first mutation */ }", "imports": [] }, "sessionToken": "<token>" }
// → { "exceptionMessage": "", ..., "mutatedOmlPath": "C:\\…\\<run1>-out.oml" }

// Call 2: omlReset — drop the chain (the discarded copy is left on disk, not deleted)
{ "input": { "eSpaceName": "MyModule" }, "sessionToken": "<token>" }
// → { "discardedChanges": true, "eSpaceName": "MyModule" }

// Call 3: next applyModelApiCode re-snapshots the live module
{ "input": { "eSpaceName": "MyModule", "code": "eSpace => { /* fresh mutation against the live state */ }", "imports": [] }, "sessionToken": "<token>" }
// → { "exceptionMessage": "", ..., "mutatedOmlPath": "C:\\…\\<run3>-out.oml" }
```

If the reset turns out to be a mistake, `omlMerge` with Call 1's `mutatedOmlPath` brings the dropped mutation back — the reset dropped the pointer, not the file.

See [`04-oml-reset.md`](04-oml-reset.md) for a fuller walkthrough of the drop-and-restart scenario.

**`omlRefreshReferences` also drops the pointer.** Refreshing references (`input: { eSpaceName }` plus the token) mid-chain makes the unmerged chain vanish from reads, exactly like a reset (the `-out.oml` stays on disk). Refresh *before* Call 1, or `omlMerge` first.

---

## What just happened

Two `applyModelApiCode` calls auto-chained through the host's session pointer:

1. Call 1 snapshotted the open module, finalised the agent's lambda (stripped trailing `}`, injected `Save`, re-closed), mutated, wrote the out file, advanced the pointer to it.
2. Call 2 read from the pointer (transparent), mutated, wrote a new out file, advanced the pointer again.

The agent passed only `eSpaceName`, a full `code` lambda and `imports` (inside `input`, alongside its `sessionToken`) on every call. No `omlPath`. No `outPath`. No intermediate-file bookkeeping. No `eSpace.Save(...)` in the lambda.

## Things to remember

- **Successful mutations chain automatically.** Pass `input: { eSpaceName, code, imports }` (a full `eSpace => { ... }` lambda; `imports` may be `[]`) plus `sessionToken`; you never thread paths between calls.
- **A failed call doesn't break the chain.** Populated `exceptionMessage` (or an MCP tool error) ⇒ no out file written ⇒ pointer unchanged ⇒ the next call still reads from the last-good state. (Watch out for the silent-no-op trap, where the response is clean and the out file is written but the OML wasn't actually mutated — see [`02-recovery-from-compile-error.md`](02-recovery-from-compile-error.md).)
- **Composition is for ergonomic locality**, not atomicity — atomicity comes from the pointer.
- **Use `OfType<IServerEntity>()` before `.Named()`** when filtering by entity kind, and pass `OutSystems.Model.Data` in `imports` for that cast.
- **To deliberately drop a chain mid-flow**, call `omlReset` — see [`04-oml-reset.md`](04-oml-reset.md).
- **Finalising the chain.** After the *last* successful `applyModelApiCode` call, pass its `mutatedOmlPath` to `omlMerge` to bring the change into the open module — the default closing step; check `merged` in the response. See [`../SKILL.md`](../SKILL.md) § 3.1.
