# 05 — Traditional Web read + mutate via MCP

**Transport**: MCP (Service Studio in-process MCP Server).
**Scenario**: Style-detect → enumerate Traditional screens → read one screen's structure → add a session variable + rename a screen, all in one `applyModelApiCode` lambda → surface back to Service Studio via the merge UI. The canonical walkthrough for a Claude Code agent that needs to mutate a Traditional Web module.

The Traditional read surface (5 single-object `*Traditional` + 5 `*NamesTraditional`), their Model-API-code output, the host-default `imports`, and the per-style descent patterns referenced below are all documented in [`../reference/traditional-patterns.md`](../reference/traditional-patterns.md). For Mobile / Reactive, see [`01-add-entity-via-mcp.md`](01-add-entity-via-mcp.md).

---

## User message

> In my open Traditional FloorPlan module, add a session variable called `LastViewedPlanId` (Integer) and rename the `Plan_List` screen to `Plans`.

## Turn

Four MCP calls, plus IDE-side finalisation. (Every call carries the `sessionToken` obtained once from `createSessionToken` — see [`01-add-entity-via-mcp.md`](01-add-entity-via-mcp.md) Call 0.)

### Call 1 — `listApps` (style detection)

Always start with `listApps` to confirm the module style. Picking the right read verb depends on it. `listApps` is the one tool whose arguments are **not** wrapped in `input`:

**Request**:

```jsonc
{ "sessionToken": "<token>" }
```

**Response** (abridged):

```jsonc
{
  "apps": [
    {
      "key": "9803f00d-...",
      "name": "FloorPlan",
      "kind": "eSpace",
      "isOpen": true,
      "isReference": false,
      "moduleType": "Traditional"
    }
  ]
}
```

`moduleType: "Traditional"` → use the `*Traditional` verbs for UI reads, NOT the Reactive/Mobile `getScreen` / `getWebBlock` (those are Mobile-bound).

### Call 2 — `getScreenNamesTraditional`

Find the screen the user named. Same output shape as the Mobile `getScreenNames`. This is a name-list read — it takes no `includeJson`, but still wraps `eSpaceName` in `input`.

**Request**:

```jsonc
{ "input": { "eSpaceName": "FloorPlan" }, "sessionToken": "<token>" }
```

**Response** (abridged — only the relevant entry):

```jsonc
{
  "result": "[ ..., { \"Name\": \"Plan_List\", \"Description\": \"\", \"UIFlow\": \"PlansFlow\", \"IsReferenced\": false }, ... ]"
}
```

`IsReferenced: false` → the screen is local to FloorPlan, safe to rename. (Renaming a referenced screen would fail at validation time.)

### Call 3 — `getScreenTraditional` (optional — confirm shape)

If you want to inspect the screen before mutating (recommended for non-trivial changes). This is a code-returning read, so `includeJson` is **required** — always pass `"Never"`:

**Request**:

```jsonc
{ "input": { "eSpaceName": "FloorPlan", "objectName": "Plan_List", "includeJson": "Never" }, "sessionToken": "<token>" }
```

**Response** is markdown-fenced **Model API C# code** containing the full screen tree (Preparation nodes, Widgets, …) — the same dialect you emit into `applyModelApiCode` (see [`../reference/traditional-patterns.md`](../reference/traditional-patterns.md) "Output shape — important"). Use it both to confirm the screen exists and to pattern-match its structure when composing the mutation lambda.

For this scenario, the screen rename is trivial — we can skip Call 3 if we trust Call 2's result.

### Call 4 — `applyModelApiCode` (single lambda, both mutations)

Combine both edits in one lambda. The host runs the snippet in-process via the bundled sidecar over its native Model API, so Traditional types work directly. `imports` is required but can be empty here — the default imports cover `OutSystems.Model.UI.Web`.

**Request**:

```jsonc
{
  "input": {
    "eSpaceName": "FloorPlan",
    "code": "eSpace => { var lastViewed = eSpace.CreateSessionVariable(false, \"LastViewedPlanId\"); lastViewed.DataType = eSpace.IntegerType; var screen = eSpace.WebFlows.SelectMany(f => f.Nodes).OfType<IWebScreen>().First(s => s.Name == \"Plan_List\"); screen.Name = \"Plans\"; }",
    "imports": []
  },
  "sessionToken": "<token>"
}
```

What the host literally hands to the sidecar — trailing `}` stripped, `Save` injected, lambda re-closed (matches the contract from § 1 of SKILL.md):

```csharp
eSpace => {
    var lastViewed = eSpace.CreateSessionVariable(false, "LastViewedPlanId");
    lastViewed.DataType = eSpace.IntegerType;
    var screen = eSpace.WebFlows.SelectMany(f => f.Nodes).OfType<IWebScreen>().First(s => s.Name == "Plan_List");
    screen.Name = "Plans";
    eSpace.Save("C:\\Users\\…\\<runId>-out.oml");
}
```

Note: `IWebScreen` is unqualified — the host-default imports include `OutSystems.Model.UI.Web`, so no `using` or FQN needed. The session variable's type is set after creation (`lastViewed.DataType = eSpace.IntegerType`); `CreateSessionVariable`'s first argument is `isReadOnly`, not the name.

**Response** (success, with a pre-existing validation warning):

```jsonc
{
  "exceptionMessage":   "",
  "stdoutOutput":       "",
  "stderrOutput":       "",
  "validationMessages": [
    {
      "id":        "HtmlInjection",
      "type":      "Warning",
      "message":   "HTML Injection",
      "detail":    "The expression is not escaped.",
      "ownerKey":  "…",
      "ownerPath": "/Common/SimpleColorPicker/If/False/Expression",
      "ownerType": "InlineExpression"
    }
  ],
  "mutatedOmlPath":     "C:\\Users\\…\\<runId>-out.oml"
}
```

Empty `exceptionMessage` + the out file was written → the session pointer advanced and the OML is saved on disk under `%TEMP%\ServiceStudio.MCPServer\`. The lone `validationMessages` entry has `type: "Warning"` (not `"Error"`; values are capitalised, so compare case-insensitively), and it's the pre-existing SimpleColorPicker HTML-injection note, unrelated to our changes — note it but don't act on it unless the user asked. (A `type: "Error"` entry would be the saved-but-invalid case and would warrant a fix.)

Before merging, confirm the rename actually took the name you asked for: the Model API silently suffixes a taken or reserved name (`Plans` → `Plans2`) instead of failing, so a quick `getScreenNamesTraditional` read-back of `.Name` is worth the call.

### Call 5 — auto-merge into Service Studio

Take the `mutatedOmlPath` from the last successful `applyModelApiCode` and call `omlMerge` with it — the default closing step (see [`../SKILL.md`](../SKILL.md) § 3.1):

```jsonc
// call tool omlMerge
{ "input": { "eSpaceName": "FloorPlan", "mutatedOmlPath": "C:\\Users\\…\\<runId>-out.oml" }, "sessionToken": "<token>" }
```

The mutations are merged into the live module — highlighted in Service Studio's merge window for the user to review and accept (or reject), or accepted with no dialog, per their setting.

---

## Variants

- **Read-only browse** (no mutations): stop after Call 2 / Call 3. Nothing to surface.
- **Rename only**, no session variable: drop the `CreateSessionVariable` line from Call 4's lambda. Same flow otherwise.
- **Larger structural change** (e.g. add a screen action): peek with Call 3 first, then build the lambda referencing the existing widget tree. Stay in one `applyModelApiCode` lambda when possible — the session pointer chains successive calls but a single lambda is atomic in the merge UI.

## Pattern-matching cheatsheet for Traditional lambdas

| Need | Snippet |
|---|---|
| Find a local screen | `eSpace.WebFlows.SelectMany(f => f.Nodes).OfType<IWebScreen>().First(s => s.Name == "...")` |
| Find a local web block | `eSpace.WebFlows.SelectMany(f => f.GetAllDescendantsOfType<IWebBlock>()).First(b => b.Name == "...")` |
| Iterate a screen's actions | `screen.ScreenActions` (NOT `ClientActions` — that's the Reactive name) |
| Add a session variable | `var v = eSpace.CreateSessionVariable(false, name); v.DataType = eSpace.IntegerType;` (first argument is `isReadOnly`; `eSpace.TextType`, `.BooleanType`, …) |
| Read or set a session variable | `eSpace.SessionVariables.Named(name)` (throws if missing) |
| Rename a UI element | Set `.Name` directly (the Model API handles all key/reference updates) |

Full recipe set: [`../reference/traditional-patterns.md`](../reference/traditional-patterns.md).
