# 02 — Recovery from the silent-no-op trap (and other failure modes)

**Transport**: MCP (Service Studio in-process MCP Server).
**Scenario**: Claude submits a lambda that compiles cleanly and runs, but never actually mutates `eSpace` (e.g. a read-only statement while exploring the API). The host's appended `eSpace.Save(...)` still runs, producing a "saved" OML whose bytes are identical to the input. The response *looks* like a clean save, but a follow-up read shows nothing changed. Claude diagnoses the silent-no-op trap, learns the right way to explore (the throw-probe), retries with statements that actually mutate `eSpace`, succeeds.

This walkthrough teaches the **dominant failure mode** on the MCP path. The wire response on Attempt 1 carries no error fields — the only signal is that the verifying read doesn't reflect the mutation. See [`../reference/lambda-contract.md`](../reference/lambda-contract.md) § "Statements that don't reference `eSpace`" for the full mechanics.

---

## User message

> Add a `Customer` entity to the module I have open in Service Studio.

## Turn

### Attempt 1 — lambda that never mutates `eSpace` (silently no-ops)

Claude is exploring before mutating and submits a lambda that only *reads* — collecting the existing entity names into a local — forgetting that a local variable reports nothing back and that the appended `eSpace.Save(...)` will still run:

**Request** (call tool `applyModelApiCode`):

```jsonc
{
  "input": {
    "eSpaceName": "MyModule",
    "code":    "eSpace => { var names = string.Join(\", \", eSpace.Entities.Select(e => e.Name)); }",
    "imports": []
  },
  "sessionToken": "<token>"
}
```

What the host literally hands to the sidecar — trailing `}` stripped, `Save` injected, lambda re-closed:

```csharp
eSpace => {
var names = string.Join(", ", eSpace.Entities.Select(e => e.Name));
eSpace.Save("C:\\…\\<runId>-out.oml");
}
```

The lambda compiles and runs cleanly. Nothing mutates `eSpace`. The host's appended `eSpace.Save(...)` then persists the **unmodified** eSpace to disk.

(Had Claude tried to *print* the names with `System.Console.WriteLine`, the call would not even have run: `Console` and anything written `System.…` are rejected by the code sandbox before compile — see "Sandbox rejection" below.)

**Response** (looks successful at a glance):

```jsonc
{
  "exceptionMessage":   "",
  "stdoutOutput":       "",
  "stderrOutput":       "",
  "validationMessages": [],
  "mutatedOmlPath":     "C:\\…\\<runId>-out.oml"
}
```

`exceptionMessage` is empty and `validationMessages` is empty. The out file was written, so `mutatedOmlPath` is populated and the session pointer advanced. Pattern-matching on the response alone, the agent thinks the mutation landed — and the names it computed went nowhere.

### Diagnosing — verify by reading back

The skill's "read your work back" guidance kicks in. The agent calls `getDataModel`:

**Request**:

```jsonc
{ "input": { "eSpaceName": "MyModule", "includeJson": "Never" }, "sessionToken": "<token>" }
```

**Response** (excerpt — `Customer` is absent):

```csharp
/*** creating IStaticEntity 'MenuItem' ***/
var menuItem = testBasicTypesWithou.CreateStaticEntity("MenuItem");
/* ...everything that was in the original module... */
/* no Customer entity */
```

The follow-up read is the only place the silent no-op surfaces. The clean response was telling the truth — the out file *was* written — but it points at a "saved" OML whose bytes are identical to the input. The fix:

| Symptom | Cause | Fix |
|---|---|---|
| Empty `exceptionMessage`, empty `validationMessages`, populated `mutatedOmlPath`, but the change isn't visible to a follow-up `get*` read | Lambda compiled and ran but never mutated `eSpace` | Rewrite the lambda so it actually mutates `eSpace`; the host will save the new state on the next call |

### Exploring correctly — the throw-probe

To *see* something from inside a lambda, don't print (the sandbox forbids `Console`) and don't rely on a local: build a string and **throw it**. The text comes back in `exceptionMessage`, nothing is saved, and the session pointer does not move — so a probe can never masquerade as a mutation.

**Request**:

```jsonc
{
  "input": {
    "eSpaceName": "MyModule",
    "code":    "eSpace => { var names = string.Join(\", \", eSpace.Entities.Select(e => e.Name)); throw new Exception(\"PROBE:\" + names); }",
    "imports": []
  },
  "sessionToken": "<token>"
}
```

**Response**:

```jsonc
{
  "exceptionMessage":   "PROBE:<comma-separated entity names>",
  "stdoutOutput":       "",
  "stderrOutput":       "",
  "validationMessages": [ /* TrueChange messages for the in-memory model — still returned */ ],
  "mutatedOmlPath":     ""
}
```

Look for the `PROBE:` prefix in `exceptionMessage` to tell your probe apart from a genuine runtime throw. Because `validationMessages` are still computed for the in-memory model, putting a real edit *before* the `throw` previews its TrueChange result without committing it.

### Attempt 2 — full lambda that mutates `eSpace` (SUCCEEDS)

Same intent, now a lambda that actually touches `eSpace`:

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
  "mutatedOmlPath":     "C:\\…\\<runId>-out.oml"
}
```

Verifying read against `getDataModel` now shows the `Customer` entity. Done.

---

## Other recoverable failure modes (the ones with explicit error signals)

The silent no-op is the only failure mode that doesn't surface in the response. Everything else surfaces either as a populated `exceptionMessage` (a runtime throw) or as an MCP tool error with no result at all. Sandbox rejections and compile failures are the latter: code that doesn't compile never runs, so the diagnostics arrive in the tool error's message, one per line prefixed `compilationErrors:`. Missing-identifier and bad-statement errors are compile failures, so they take that path too. In each of these cases the out file is **not** written, so the session pointer stays at the previous good state.

### Sandbox rejection — `Console`, `System.…`, reflection, or a disallowed import

**Response** — an MCP tool error, not a result. The code is scanned *before* compile; any reference to `Console`, any token written `System.…` (e.g. `System.Text.StringBuilder`), or a reflection member (`GetProperties`, `GetMethod`, `Invoke`, …) is refused:

```
'code' must only reference the OutSystems Model API - direct file, process, environment, reflection, registry, interop, or network access is not allowed (found 'Console')
```

An `imports` entry outside `OutSystems.Model` is refused the same way:

```
Import 'ServiceStudio.Plugin.NRWidgets' is not allowed. Only 'OutSystems.Model' (or one of its sub-namespaces) may be imported.
```

Remove the offending member. Use the throw-probe above instead of printing, unqualified BCL basics (`Exception`, `List<T>`, LINQ, `string.Join`, `obj.GetType().Name`) instead of `System.…` types, and write plugin types fully qualified in the code (`ServiceStudio.Plugin.NRWidgets.IInput`) instead of importing their namespace.

### `CS1061` — wrong API name

**Response** — an MCP tool error (`OML_TOOL_CODE_REJECTED`), not a result:

```
[-32602] Script compilation failed
compilationErrors: (1,17): error CS1061: 'IESpace' does not contain a definition for 'AddEntity' and no accessible extension method 'AddEntity' accepting a first argument of type 'IESpace' could be found ...
```

You called a method that doesn't exist. The Model API uses `CreateServerEntity` / `CreateClientEntity` / `CreateStaticEntity`, not `AddEntity`. Read the [`../docs/OutSystems.Model.Generated.cs`](../docs/OutSystems.Model.Generated.cs) namespace docs or call `getDataModel` to see how existing entities are constructed.

### `CS0103` — wrong identifier (e.g. `m` instead of `eSpace`)

**Response** — an MCP tool error (`OML_TOOL_CODE_REJECTED`), not a result:

```
[-32602] Script compilation failed
compilationErrors: (1,16): error CS0103: The name 'm' does not exist in the current context
```

You used a parameter name other than `eSpace`. The lambda parameter must literally be `eSpace` (the host strips and re-closes around your `eSpace => { ... }`); rename references in your lambda to `eSpace`.

### `CS0103` (alt) — unknown type name

**Response** — an MCP tool error (`OML_TOOL_CODE_REJECTED`), not a result:

```
[-32602] Script compilation failed
compilationErrors: (1,17): error CS0103: The name 'IServerEntity' does not exist in the current context
```

You referenced a type without bringing in its namespace. The default imports (3 host + 7 sidecar) cover most root types, but interfaces under `OutSystems.Model.Data` (`IServerEntity`, `IStaticEntity`, `IAttribute`), `OutSystems.Model.Logic.Nodes`, etc. need to be passed via `imports`:

```jsonc
{ "input": { "eSpaceName": "MyModule", "code": "eSpace => { … }", "imports": ["OutSystems.Model.Data"] }, "sessionToken": "<token>" }
```

Only `OutSystems.Model` and its sub-namespaces may be imported (see "Sandbox rejection" above).

### `CS0201` — bare expression instead of a statement

**Response** — an MCP tool error (`OML_TOOL_CODE_REJECTED`), not a result:

```
[-32602] Script compilation failed
compilationErrors: (2,1): error CS0201: Only assignment, call, increment, decrement, await, and new object expressions can be used as a statement
```

Your lambda body contained a bare expression (`42;`, a property access without an assignment, etc.) where C# requires a statement. Use an assignment, a method call, or remove the orphan expression.

### Runtime exception (compile clean, runs, throws)

```jsonc
{
  "exceptionMessage":   "Object reference not set to an instance of an object. — Unable to find object with Name equal to MyProcess in collection",
  "stdoutOutput":       "",
  "stderrOutput":       "",
  "validationMessages": [],
  "mutatedOmlPath":     ""
}
```

Note this one is a genuine **runtime** throw (compiled cleanly, ran, then threw), so it surfaces through `exceptionMessage` in a normal result — unlike the sandbox and compile failures above, which return no result at all. (A throw-probe takes this same channel on purpose; the `PROBE:` prefix is how you tell the two apart.) The lambda compiled, ran (potentially producing partial state), and threw inside your code. Common causes: `Named("Foo")` against a name that doesn't exist (returns null, then null-deref'd), wrong `Where`/`First` predicate, casting to the wrong interface (e.g. `IClientEntity` for a database-backed entity).

The host folds the sidecar's JSON-RPC error envelope into `exceptionMessage`: it carries the top-level message (typically the .NET exception's `.Message`) and, where the sidecar emitted richer detail (`error.data.innerMessage`), that diagnostic too. Read it fully — sometimes the generic part ("Object reference not set...") sits next to the specific cause ("Unable to find object with Name equal to ...").

When an exception is thrown, the host's appended `eSpace.Save(...)` never runs, so **no out file is written** and the session pointer stays at the previous good state.

### Saved-but-invalid (validation errors)

A distinct case: the lambda ran, mutated `eSpace`, and the out file **was** written (so `exceptionMessage` is empty), but the resulting model has validation problems. These surface as entries in `validationMessages`:

```jsonc
{
  "exceptionMessage":   "",
  "stdoutOutput":       "",
  "stderrOutput":       "",
  "validationMessages": [
    {
      "id":        "RequiredPropertyValue",
      "type":      "Error",
      "message":   "Required Property Value",
      "detail":    "Unknown 'MenuItem Identifier' Data Type in 'ActiveMenuItemId' Input Parameter.",
      "ownerKey":  "…",
      "ownerPath": "/Common/Menu/ActiveMenuItemId",
      "ownerType": "SerializableInputParameter"
    }
  ]
}
```

A `validationMessages` entry whose `type` is `"Error"` is the saved-but-invalid case: the pointer **did** advance (out file written, empty `exceptionMessage`), but the model carries errors you should fix. A non-empty array alone doesn't mean the save failed, since `"Warning"` and `"Info"` entries are normal; branch on `type` (compare case-insensitively).

---

## What just happened

A two-attempt recovery loop driven by reading the work back:

1. Lambda that never mutates `eSpace` → response looks clean (empty `exceptionMessage`, empty `validationMessages`, out file written), but the verifying `getDataModel` doesn't show the change. Silent no-op caught by reading back, not by inspecting the response. Exploration belongs in a throw-probe, which returns its text in `exceptionMessage` and saves nothing.
2. Retry with statements that actually mutate `eSpace` → real mutation, verifying read confirms.

The MCP host **did** advance its session pointer on Attempt 1 (the out file was written with an empty `exceptionMessage`), so Attempt 2 reads from the silent-no-op OML — which happens to be byte-identical to the live module's initial state, so no harm. (If the silent no-op had layered atop earlier real changes, the pointer would still advance, and the silent-no-op OML would carry those earlier changes intact. The trap is "no new change", not "lost old changes".)

## Things to remember

- **The silent no-op is the only failure mode the response can't tell you about.** Always verify mutations by reading back via the matching `get*` tool. If the change isn't there, the lambda never touched `eSpace`.
- **"Out file written + empty `exceptionMessage`" is necessary but not sufficient for "the mutation landed".** Sufficient = pointer advanced AND a follow-up read shows the change.
- **Probe by throwing, never by printing.** `throw new Exception("PROBE:" + s);` returns `s` in `exceptionMessage`, saves nothing, leaves the pointer where it was, and still returns `validationMessages`. `Console`, `System.…` and reflection are rejected by the sandbox before compile.
- **Pass a full lambda, no `Save`.** The `examples/*.cs` files are already full `eSpace => { ... }` lambdas with no `eSpace.Save` — paste the lambda verbatim as `code` (and lift the file's `using` directives into `imports` — only `OutSystems.Model.*` namespaces are allowed there). The `code` must end with `}` (no trailing whitespace/newline/semicolon).
- **Populated `exceptionMessage` (or an MCP tool error)** ⇒ the lambda ran and threw; no out file was written, so the input OML and the chain's last-good state are preserved automatically. Retry freely.
- **An MCP tool error carrying `compilationErrors:` lines, or a sandbox / import rejection** ⇒ the lambda never ran and there is no result; no out file was written. Retry freely.
- **`validationMessages` with a `type: "Error"` entry** ⇒ saved-but-invalid: the pointer advanced but the model has errors. Fix them.
- **No `revertChanges` verb exists** — the host does not advance the pointer when no out file is written, so the input OML (and the chain's last-good state) is preserved for the error cases. For the silent no-op, the pointer DID advance but the saved bytes equal the input. To deliberately drop a successfully-built chain, call `omlReset` (see [`04-oml-reset.md`](04-oml-reset.md)). To surface a chain back into the IDE, call `omlMerge` with the `mutatedOmlPath` from the apply response so the user can review and accept it.
