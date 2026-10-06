# The full-lambda contract

## In one sentence

The `code` argument you pass to the `applyModelApiCode` MCP tool is a **full
`eSpace => { ... }` lambda — but without the `eSpace.Save(...)` call**. The
host strips the trailing `}` from your `code`, appends
`eSpace.Save("<host-path>")`, and re-closes the lambda before handing it to the
sidecar's `applyModelApiCode` verb.

> This is the **inverse** of the older contract, which took a bare *body* and
> wrapped it for you. If you have old "body-only" snippets, wrap them in
> `eSpace => { ... }` and drop any `eSpace.Save(...)` line.

## The canonical shape

```csharp
eSpace => {
    var customer = eSpace.Entities
        .OfType<OutSystems.Model.Data.IServerEntity>()
        .Named("Customer");
    customer.Description = "PII";
}
```

That's what you send (as the `code` string, ending in `}`). The host
rewrites it before forwarding it to the sidecar:

```csharp
eSpace => {
    var customer = eSpace.Entities
        .OfType<OutSystems.Model.Data.IServerEntity>()
        .Named("Customer");
    customer.Description = "PII";
eSpace.Save("C:\\Users\\…\\<runId>-out.oml");
}
```

Three consequences worth memorising:

1. **The lambda parameter must be named `eSpace`.** The injected
   `eSpace.Save(...)` is hardcoded to that name. Any other name produces
   `CS0103: The name 'X' does not exist in the current context`.
2. **Your `code` must end with `}` and nothing after it.** The host removes the
   **last character**, assuming it is the closing brace. A trailing newline,
   space, or `;` corrupts the wrap.
3. **Don't write `eSpace.Save(...)` yourself.** The host appends one against a
   path it owns; yours is redundant and risks malforming the wrap.

## How the host wraps your code

The host removes the last character of your `code` (the closing `}`), appends
`eSpace.Save("<host-allocated-path>")`, and re-closes the lambda.

So if your `code` is `eSpace => { customer.Description = "PII"; }`, the literal
string the host hands to the sidecar's `applyModelApiCode` verb is:

```csharp
eSpace => { customer.Description = "PII"; 
eSpace.Save("C:\\Users\\…\\<runId>-out.oml");
}
```

That's what the sidecar compiles as an `Action<IESpace>`.

## What the runtime is actually doing

Inside the sidecar, the verb compiles the lambda text **with
`Action<IESpace>` as the expected delegate type**, then invokes the compiled
delegate with the live in-memory `IESpace` loaded from the snapshot the host
wrote. The host-injected `eSpace.Save("...")` then writes the mutated module
back to the path the host allocated. There is no IPC inside that step — the
eSpace lives in the sidecar process and the lambda mutates it in place.

The host (in Service Studio) and the sidecar talk
over **stdio JSON-RPC**: one child process per call, request in, single
response line out (see [`mcp-session-pointer.md`](mcp-session-pointer.md) and
the architecture notes in `SKILL.md`).

### The `Action<IESpace>` target

The compilation target type is `System.Action<OutSystems.Model.IESpace>` —
passing a non-lambda (e.g. a literal `42`) yields:

```
error CS0029: Cannot implicitly convert type 'int' to 'System.Action<OutSystems.Model.IESpace>'
```

That single error reveals the target type, namespace, and `Action<T>` shape.
You'll rarely see `CS0029` through the MCP host, because the host always emits
a syntactically valid `Action<IESpace>` lambda — provided your `code` is a
well-formed `eSpace => { ... }` ending in `}`.

## What happens with each `code` shape

### Full lambda that mutates `eSpace` — works (this is the contract)

```csharp
eSpace => {
    var customer = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Customer");
    customer.Description = "PII";
}
```

After wrap, it compiles, runs, mutates, and the host's Save persists. The
session pointer advances; `exceptionMessage` is empty.

### Full lambda that never references `eSpace` — compiles but no-ops

```csharp
eSpace => { var n = eSpace.Name; }
```

After wrap this is a valid `Action<IESpace>`. The body executes, the injected
Save persists the **unmodified** eSpace. The
response looks clean and the pointer advances — but the OML on disk is
byte-identical to the input. This is the **silent no-op**. Defend against it by
always actually mutating `eSpace`.

### Body-only statements (no `eSpace => { ... }` wrapper) — compile failure

```csharp
customer.Description = "PII";
```

The host removes the last character (the `;`) and appends
`eSpace.Save(...);}`, producing:

```csharp
customer.Description = "PII"
eSpace.Save("...");
}
```

There is no lambda head and a dangling `}` — the sidecar can't compile it as an
`Action<IESpace>`. (This was the *old* contract; it is now inverted.) Fix: wrap
your statements in `eSpace => { ... }`.

### `code` not ending in `}` — corrupted wrap

If your `code` ends in a newline, space, or `;` after the closing brace, the
host strips that trailing character instead of the brace, leaving your real `}`
in place and appending the Save **outside** the lambda. Result: a compile
error. End the string exactly with `}`.

### Lambda using a name other than `eSpace` — `CS0103`

```csharp
m => { var customer = m.Entities.Named("Customer"); customer.Description = "PII"; }
```

The injected save still references `eSpace`, so:
`error CS0103: The name 'eSpace' does not exist in the current context`
(and your `m` references won't match the injected save). Use `eSpace`.

### Summary

| `code` shape | Compiles? | Mutates OML? | Response |
|---|---|---|---|
| Full lambda that mutates `eSpace` | Yes | Yes | clean; pointer advances |
| Full lambda that never touches `eSpace` | Yes | No | clean-looking but OML unmodified — **silent no-op** |
| Body-only statements (no wrapper) | No | No | MCP tool error carrying the diagnostics; pointer unchanged |
| `code` not ending in `}` | No | No | rejected by the host before the sidecar runs — `INVALID_ARGUMENT`, telling you the body must end in `}` |
| Lambda parameter ≠ `eSpace` | No (`CS0103`) | No | MCP tool error carrying the diagnostics |
| Mentions `Console`, a `System.…` name, reflection, or imports a non-`OutSystems.Model` namespace | No — rejected by the sandbox before compiling | No | MCP tool error naming the offending token (§ "Sandbox" below) |
| Ends with `throw new Exception("PROBE:" + s)` | Yes | No (by design) | `exceptionMessage` = your text; `validationMessages` still returned; pointer unchanged |

## Why the MCP host injects the Save

The host owns the output path so it can advance the session pointer
deterministically (see [`mcp-session-pointer.md`](mcp-session-pointer.md)) —
you never name the save destination. The standalone verb does
NOT inject a save; used directly, you write the full lambda **with**
an explicit `eSpace.Save(...)` line. So the only difference between the two
contracts is the save: write the full lambda either way, but omit the save when
going through the MCP `applyModelApiCode` tool.

## The response shape and recovery loop

`applyModelApiCode` returns:

```jsonc
{
  "exceptionMessage":   "",  // runtime error text; non-empty means the code ran and threw, so nothing was saved
  "stdoutOutput":       "",
  "stderrOutput":       "",
  "validationMessages": [],  // ValidationMessage[] — TrueChange errors/warnings against the resulting model
  "mutatedOmlPath":     ""   // path to the saved OML on a clean run; "" when nothing was saved. Hand it to `omlMerge`
}
```

Each `validationMessages` entry carries `id`, `type` (`"Info"` / `"Warning"` /
`"Error"` — capitalised; compare case-insensitively), `message`, `detail`, `ownerKey`, `ownerPath` and `ownerType`. There
is still no `hasValidationErrors` / `validationErrorCount` field, but you no
longer scan text for it — filter on `type` equal to `"Error"`. Runtime failures
surface through `exceptionMessage` — or, if the sidecar returns a JSON-RPC
error envelope, as an MCP tool error (`OML_TOOL_VERB_FAILURE` /
`OML_TOOL_PROTOCOL_ERROR`). **A compile failure has no result at all** — code
that doesn't compile never runs, so it returns an MCP tool error
(`OML_TOOL_CODE_REJECTED`) whose message carries the diagnostics, one per line
prefixed `compilationErrors:`. Don't look for them in the result.
The session pointer only advances when the out file was written and
`exceptionMessage` is empty — which is
exactly when `mutatedOmlPath` is populated. **`mutatedOmlPath` alone doesn't
prove the mutation was meaningful**: both a clean run and a silent no-op
(below) populate it, since the host-injected save writes a file either way.
Failed attempts keep reads pointed at the last good state.

> **The sixth outcome: a runner process-crash gives you empty everything.** If
> an unsupported API member kills the sidecar, `exceptionMessage` is **empty**,
> `validationMessages` is `[]`, `mutatedOmlPath` is
> `""`, and **`stdoutOutput` is discarded**. Nothing you did before the crash
> reaches you. Bisect with one risky member per call; a
> `eSpace => { throw new Exception("PROBE:ok"); }` probe returning `PROBE:ok`
> proves the session is healthy and the fault is in your
> code. See SKILL.md § 3 outcome table for the full treatment and the list of
> known crash members.

| Signal | Cause | Fix |
|---|---|---|
| Tool error mentions `CS0103: The name 'eSpace' does not exist` | Lambda parameter isn't `eSpace`, or body-only code left no `eSpace` in scope | Use a full `eSpace => { ... }` lambda |
| Tool error mentions `CS1061: 'IESpace' does not contain 'Foo'` | Wrong Model API method / property name | Call `getDataModel` to learn the actual surface; correct the snippet |
| Tool error mentions `CS0201` / other syntax error, or the body must end in `}` | `code` wasn't a clean `eSpace => { ... }` ending in `}` | Re-check the wrapper, the trailing `}`, and statement validity |
| `exceptionMessage` populated at runtime (e.g. `Named("Foo")` on a missing entity, a wrong cast) | The lambda compiled but threw | Add a null guard or fix the cast. For a System / referenced entity, `eSpace.Entities` is local-only — resolve via `eSpace.References.Named("(System)")` and cast to the `*Signature` interface (see [`patterns-by-element.md`](patterns-by-element.md) § "FK to a System / referenced entity") |
| Clean response, but a follow-up `get*` doesn't reflect the mutation | The silent no-op — the lambda compiled and ran but never touched `eSpace` | Rewrite so the body actually mutates `eSpace` |
| Tool error `'code' must only reference the OutSystems Model API … (found 'X')` or `Import 'X' is not allowed` | The sandbox rejected the snippet before compiling | Remove `X`: use the typed Model API, the throw-probe instead of `Console`, and fully qualified `ServiceStudio.Plugin.*` types instead of importing them (§ "Sandbox") |
| `validationMessages` has a `type: "Error"` entry | Saved-but-invalid — the model compiled and saved but has a TrueChange error | Fix the offending element (often a hard-coded aggregate name that was auto-renamed); see [`patterns-by-element.md`](patterns-by-element.md) |

The recovery is always the same shape: read the response, edit the lambda,
retry. Failed calls don't advance the pointer, so successive attempts resume
from the last-good state without corrupting the input.

## Default imports — what's already in scope

The host prepends 3 UI namespaces to your `imports` before forwarding to the
sidecar:

- `OutSystems.Model.UI`
- `OutSystems.Model.UI.Web`
- `OutSystems.Model.UI.Mobile`

The bundled sidecar additionally pre-imports the core Model API namespaces in
its runner, so these are in scope too:

- `System`, `System.Linq`, `System.Collections.Generic` (so `List<T>`, `Exception`,
  `string.Join`, LINQ work unqualified), `OutSystems.Model`, `OutSystems.Model.Enumerations`,
  `OutSystems.Model.Expressions`, `OutSystems.Model.Factory`,
  `OutSystems.Model.Types`

Other **`OutSystems.Model.*`** namespaces — `OutSystems.Model.UI.Mobile.Widgets`,
`OutSystems.Model.Logic.Nodes`, `OutSystems.Model.Data`, etc. — may be passed via
the `imports` array. **Nothing outside `OutSystems.Model` is accepted** (see
"Sandbox"): write `ServiceStudio.Plugin.NRWidgets.IInput`,
`ServiceStudio.Plugin.RESTService.IRestService`, … fully qualified in the code.

```jsonc
{
  "input": {
    "eSpaceName": "MyModule",
    "code":    "eSpace => { /* ... uses IMobileScreen + a widget ... */ }",
    "imports": ["OutSystems.Model.UI.Mobile.Widgets"]
  },
  "sessionToken": "<token>"
}
```

The host merges your `imports` after the 3 defaults (deduplicated,
case-insensitive). `imports` is required on the wire; `[]` is fine.

## Sandbox

Since the 2026-09 Service Studio builds the host screens every call **before
compiling** (verified live 2026-09-30):

| Rejected | Error | Use instead |
|---|---|---|
| `Console.WriteLine(…)` | `'code' must only reference the OutSystems Model API - direct file, process, environment, reflection, registry, interop, or network access is not allowed (found 'Console')` | the throw-probe below |
| any `System.…`-qualified name (`System.Text.StringBuilder`, `System.Enum.GetNames`, `System.Collections.Generic.List`, `System.Diagnostics…`) | same, `(found 'System')` | the unqualified type (`List<T>`, string concatenation), or the typed enum member from [`../docs/`](../docs/) |
| reflection — `GetProperties`, `GetProperty`, `GetMethod`, `Invoke`, `AppDomain` … | same, `(found '.GetProperties')` | the typed member — see the replacement table below |
| `imports` outside `OutSystems.Model.*` | `Import 'X' is not allowed. Only 'OutSystems.Model' (or one of its sub-namespaces) may be imported.` | fully qualify the type in the code |
| C# 12 collection expressions — `[a, b]`, `List<T> l = [...]` | `'code' is not valid C#: Invalid expression term '['` (the pre-parser targets an older language version) | `new[] { a, b }`; for OutSystems list/record literals use `ExpressionDefinition.Parse("[ { Value: 1, Label: \"a\" } ]")` |
| `ServiceStudio.Model.*` internals | `CS0234` (not referenced) | not reachable — the capability is unsupported |

`obj.GetType().Name` is still allowed (useful to print a node or widget kind), and so is
`obj.GetType().GetInterfaces()` (verified 2026-10-01): use it to find which interfaces a widget
actually implements when a cast or member fails to compile.

### The throw-probe — how to see values now

```csharp
eSpace => {
    var s = "screens=" + string.Join(",", eSpace.MobileFlows.SelectMany(f => f.Nodes).OfType<IMobileScreen>().Select(x => x.Name));
    throw new Exception("PROBE:" + s);
}
```

The response has `exceptionMessage: "PROBE:screens=…"` (verbatim), `mutatedOmlPath: ""`,
the pointer does not move, and **`validationMessages` still lists TrueChange
messages for the in-memory model** — so you can build an edit, throw at the
end, and read what TrueChange *would* say before committing it. Full
template: [`../examples/ProbeModelWithException.cs`](../examples/ProbeModelWithException.cs).

### Typed replacements for old reflection recipes (all verified live 2026-09-30)

| Old recipe | Typed replacement |
|---|---|
| `AddStartEndNodes` via `ServiceStudio.Model.API.V1.ModelExtensions` | `action.CreateNode<OutSystems.Model.Logic.Nodes.IStartNode>()` and `…IEndNode>()` |
| set `Identifier` by reflection because `IdentifierAttribute =` "crashes" | `entity.IdentifierAttribute = idAttr;` works (server and static entities) |
| leave `IsAutoNumber` alone | `idAttr.IsAutoNumber = OutSystems.Model.Enumerations.AutoNumber.Yes;` |
| set a Link's default text via reflected `Text` | `link.Widgets.OfType<ITextWidget>().First().Text = "Go";` |
| rename an aggregate back via `SetPropertyValue` | `agg.Name = "GetOrders";` |
| `System.Enum.GetNames(typeof(X))` to discover members | read the enum in [`../docs/OutSystems.Model.Enumerations.Generated.cs`](../docs/OutSystems.Model.Enumerations.Generated.cs) (or the plugin enum files) |
| read entity actions via reflection on `EntityActions` | typed `entity.GetAction`, `.CreateAction`, `.CreateOrUpdateAction`, `.DeleteAllAction`, … |

## Mental model

The sidecar is asking *"is this text an `Action<IESpace>`?"*. You answer by
sending a well-formed `eSpace => { <statements> }` ending in `}`; the host turns
it into `eSpace => { <statements> eSpace.Save("...") }` and the sidecar compiles
and runs it against the snapshot. Mutate `eSpace`, end in `}`, skip the save.
