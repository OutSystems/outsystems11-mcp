# 04 — `omlReset` drop-and-restart walkthrough

**Transport**: MCP (Service Studio in-process MCP Server).
**Scenario**: The agent has built a two-step mutation chain. The user changes direction mid-task and wants the agent to start over from the live module instead of layering more mutations onto the existing chain. The agent calls `omlReset`, which ends the edit session: reads after it see the live module, and the agent starts a new session before it edits again. Part 2 shows the same tool fired *by mistake*, and why the discarded work can't come back.

> **Edit session.** The chain below runs inside an edit session started with `startOmlEditSession` ([`../SKILL.md`](../SKILL.md) § 2.1). `omlReset` ends that session along with the unmerged edits, so **call `startOmlEditSession` again before the next `applyModelApiCode`**, and `endOmlEditSession` after the final merge. `endOmlEditSession` discards the same way and is the simpler call. Neither can be undone.

For routine multi-step edits (where you just want each call to read the last good state), do nothing — the session pointer chains successive calls automatically (see [`03-multi-step-edit.md`](03-multi-step-edit.md)). To **accept** a chain into the live IDE rather than drop it, call `omlMerge` with the `mutatedOmlPath` the apply response returns. For the full pointer mechanics, see [`../reference/mcp-session-pointer.md`](../reference/mcp-session-pointer.md). All `code` payloads below are **full `eSpace => { ... }` lambdas** that end in `}` and do **not** include `eSpace.Save(...)` — see [`../reference/lambda-contract.md`](../reference/lambda-contract.md).

---

## User messages

> Add a `Customer` entity to the module I have open in Service Studio.

…then…

> Also add an `Email` attribute to `Customer`.

…and after the second mutation lands successfully…

> Actually, scrap that. I want to start fresh from the live module — no `Customer`, no `Email`.

## Turn

### Step 1 — first mutation lands

The agent emits the standard `applyModelApiCode` lambda:

**Request**:

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

**Response**:

```jsonc
{
  "exceptionMessage":   "",
  "stdoutOutput":       "",
  "stderrOutput":       "",
  "validationMessages": [],
  "mutatedOmlPath":     "C:\\…\\<run1>-out.oml"
}
```

Empty `exceptionMessage` + out file written ⇒ pointer advances to `<run1>-out.oml`.

### Step 2 — second mutation lands

```jsonc
{
  "input": {
    "eSpaceName": "MyModule",
    "code":    "eSpace => { var customer = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named(\"Customer\"); var emailKey = customer.Key.CreateNewKeyBasedOnThis(new object[] { \"Email\" }); var email = customer.CreateAttribute(\"Email\", emailKey); email.DataType = eSpace.TextType; email.Length = 200; }",
    "imports": ["OutSystems.Model.Data"]
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
  "mutatedOmlPath":     "C:\\…\\<run2>-out.oml"
}
```

Pointer advances again, to `<run2>-out.oml`.

### Step 3 — the user changes direction

The user wants to drop everything and start from the live module. The agent calls `omlReset`, naming the module:

**Request** (call tool `omlReset`):

```jsonc
{ "input": { "eSpaceName": "MyModule" }, "sessionToken": "<token>" }
```

**Response**:

```jsonc
{
  "discardedChanges": true,
  "eSpaceName":       "MyModule"
}
```

`discardedChanges: true` ⇒ an unmerged chain was thrown away for good. The file that held the `Customer` + `Email` work, `<run2>-out.oml`, stays on disk until Service Studio restarts, but it is not an undo handle: `omlMerge` refuses it (Part 2).

### Step 4 — the next call re-snapshots the live module

The agent now starts a new line of work. Since the pointer is empty, the host snapshots the live module from Service Studio and treats it as the input:

**Request** (a fresh `getDataModel` — `includeJson` still required, always `"Never"`):

```jsonc
{ "input": { "eSpaceName": "MyModule", "includeJson": "Never" }, "sessionToken": "<token>" }
```

**Response** (excerpt — no `Customer`, no `Email`; it's the pristine live state again):

```csharp
(OutSystems.Model.IReference _System_) => {
    /*** creating IStaticEntity 'MenuItem' ***/
    var menuItem = testBasicTypesWithou.CreateStaticEntity("MenuItem");
    /* ... no Customer, no Email ... */
}
```

The reset also ended the edit session, so before its next `applyModelApiCode` the agent calls `startOmlEditSession` again. An apply before that fails with `No OML edit session is started. Call startOmlEditSession before making edits.` The new session starts from the live module, and the pointer advances along a new chain from there.

---

## Part 2 — an accidental reset can't be undone

> Wait, no — I didn't mean to throw away the Customer work.

`<run2>-out.oml` is still on disk, so handing it to `omlMerge` looks like a way back. It isn't.

**Request** (call tool `omlMerge`):

```jsonc
{
  "input": {
    "eSpaceName":     "MyModule",
    "mutatedOmlPath": "C:\\…\\<run2>-out.oml"
  },
  "sessionToken": "<token>"
}
```

**Response** — an MCP tool error (`INVALID_ARGUMENT`), not a result:

```
'mutatedOmlPath' does not match the working copy currently tracked for module 'MyModule'.
```

`omlMerge` only accepts the working copy the module currently tracks, and the reset removed it. (This error comes from reading the server code, `OmlMergeTool`, on 2026-11-20; it hasn't been verified live.) Tell the user the work is gone and offer to redo it: the agent still has the code it sent in Steps 1 and 2. To avoid this, merge anything the user might want back before resetting.

---

## What just happened

Five MCP tool calls — two successful mutations, one explicit reset, one verifying read, one refused merge:

1. `applyModelApiCode` — added `Customer`. Out file written, pointer advances.
2. `applyModelApiCode` — added `Email`. Out file written, pointer advances.
3. `omlReset` — dropped the chain. The discarded OML is left on disk rather than deleted.
4. `getDataModel` — first call after reset, so the host re-snapshots the live module. The verifying read sees the original state.
5. `omlMerge` with Step 2's `mutatedOmlPath` — refused, because the reset removed the tracked working copy. The discarded work is gone.

There is **no `revertChanges` verb**. `omlReset` covers the drop-the-chain case; routine failed-call recovery is already a no-op on the pointer because a failed call writes no out file and so doesn't advance.

## When NOT to call `omlReset`

Don't call it as a defensive measure or "between unrelated tasks". A failed `applyModelApiCode` never advances the pointer in the first place, so there is nothing to clean up. Reach for `omlReset` only when:

- The user **explicitly** wants to drop the in-flight chain.
- The agent has built a chain it wants to throw away and restart against the live module.
- A debugger / test harness wants a clean session before exercising a scenario.

Don't reach for `omlReset` when the user wants to **accept** the chain — that's surfacing it through Service Studio's merge UI.

## Things to remember

- **`omlReset` takes `input: { eSpaceName }`** (plus `sessionToken`) and returns `{ discardedChanges, eSpaceName }`. A value that matches no open module raises `MODULE_NOT_LOADED`; a name open in several tabs raises `MODULE_AMBIGUOUS` (pass the `tabId`).
- **`discardedChanges: true` means an unmerged chain was thrown away**, so it is safe to report as such. `discardedChanges: false` means there was nothing in flight and the call did nothing — resetting a module that only holds a plain snapshot of the live model is a no-op, not a failure.
- **A reset can't be undone.** The discarded copy stays on disk until Service Studio restarts, but `omlMerge` refuses it. **Merge anything the user might want back before resetting.**
- **A reset that discards edits ends the edit session.** Reads then see the live module, but call `startOmlEditSession` before the next `applyModelApiCode`. A reset that returns `discardedChanges: false` leaves the session open, so keep working in it.
- **`omlRefreshReferences` with unmerged edits ends the session too**, just like a reset, and those edits are lost the same way. With none, it leaves the session's working copy without the refreshed references. Refresh before `startOmlEditSession`, or merge and end the session first.
- **Pointers are per module.** Resetting one module never touches another's chain, and switching modules in Service Studio does **not** implicitly drop a chain — if you want one gone, reset it explicitly by name.
- **`omlMerge` is the "accept" counterpart to `omlReset`** — opposite intent (surface into the IDE rather than discard). It can't undo a reset.
