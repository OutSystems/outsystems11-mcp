# 04 — `omlReset` drop-and-restart walkthrough

**Transport**: MCP (Service Studio in-process MCP Server).
**Scenario**: The agent has built a two-step mutation chain. The user changes direction mid-task and wants the agent to start over from the live module instead of layering more mutations onto the existing chain. The agent calls `omlReset`, then resumes work — the first call after the reset re-snapshots the live module. Part 2 shows the same tool fired *by mistake*, and how the discarded work comes back.

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

`discardedChanges: true` ⇒ an unmerged chain was thrown away. The working copy that held the `Customer` + `Email` work is **not deleted** — it is still sitting at `<run2>-out.oml`, the `mutatedOmlPath` Step 2 handed back. That path is your undo handle, so keep it.

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

From here, any subsequent `applyModelApiCode` will mutate the freshly-snapshotted state and advance the pointer along a new chain.

---

## Part 2 — recovering from an accidental reset

> Wait, no — I didn't mean to throw away the Customer work.

The reset dropped the *pointer*, not the *file* — `<run2>-out.oml` is still on disk. Recovery is one call: hand `omlMerge` the same `mutatedOmlPath` that Step 2 returned.

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

**Response**:

```jsonc
{ "merged": true, "message": "..." }
```

`Customer` and its `Email` attribute come back as incoming changes — presented in Service Studio's Compare-and-Merge window for the user to pick from, or accepted outright if their setting allows it. Nothing is lost.

This works because `omlReset` drops only the tracked pointer, never the file, and `omlMerge` needs nothing more than a real OML on disk. The window is the current Service Studio session: startup wipes the MCP temp folder, so recover before restarting Studio.

---

## What just happened

Five MCP tool calls — two successful mutations, one explicit reset, one verifying read, one recovery:

1. `applyModelApiCode` — added `Customer`. Out file written, pointer advances.
2. `applyModelApiCode` — added `Email`. Out file written, pointer advances.
3. `omlReset` — dropped the chain. The discarded OML is left on disk rather than deleted.
4. `getDataModel` — first call after reset, so the host re-snapshots the live module. The verifying read sees the original state.
5. `omlMerge` with Step 2's `mutatedOmlPath` — the discarded work comes back through the merge UI.

There is **no `revertChanges` verb**. `omlReset` covers the drop-the-chain case; routine failed-call recovery is already a no-op on the pointer because a failed call writes no out file and so doesn't advance.

## When NOT to call `omlReset`

Don't call it as a defensive measure or "between unrelated tasks". A failed `applyModelApiCode` never advances the pointer in the first place, so there is nothing to clean up. Reach for `omlReset` only when:

- The user **explicitly** wants to drop the in-flight chain.
- The agent has built a chain it wants to throw away and restart against the live module.
- A debugger / test harness wants a clean session before exercising a scenario.

Don't reach for `omlReset` when the user wants to **accept** the chain — that's surfacing it through Service Studio's merge UI.

## Things to remember

- **`omlReset` takes `input: { eSpaceName }`** (plus `sessionToken`) and returns `{ discardedChanges, eSpaceName }`. A module that isn't open raises `MODULE_NOT_LOADED`.
- **`discardedChanges: true` means an unmerged chain was thrown away**, so it is safe to report as such. `discardedChanges: false` means there was nothing in flight and the call did nothing — resetting a module that only holds a plain snapshot of the live model is a no-op, not a failure.
- **The discarded copy is left on disk, not deleted** — at the `mutatedOmlPath` the last apply returned. **Keep that path**: it is the only handle on the discarded work, and `omlReset` does not hand it back to you.
- **Recovery is same-session.** Service Studio startup wipes `%TEMP%\ServiceStudio.MCPServer`, so merge the copy back before restarting Studio.
- **First call after reset re-snapshots the live module.** No agent action required — just issue the next MCP call as usual.
- **`omlRefreshReferences` drops the pointer too** — an unmerged chain vanishes from reads just as after a reset (the `-out.oml` stays on disk, so the same `omlMerge` recovery applies). Refresh references before starting a chain, or merge first.
- **Pointers are per module.** Resetting one module never touches another's chain, and switching modules in Service Studio does **not** implicitly drop a chain — if you want one gone, reset it explicitly by name.
- **`omlMerge` is the "accept" counterpart to `omlReset`** — opposite intent (surface into the IDE rather than discard), and it doubles as the undo for a reset.
