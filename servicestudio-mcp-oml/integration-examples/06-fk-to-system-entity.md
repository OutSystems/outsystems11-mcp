# 06 — FK to a System / referenced entity (`User`) via MCP

**Transport**: MCP (Service Studio in-process MCP Server).
**Scenario**: Add a foreign-key attribute that points at the standard System
`User` entity — the single most common real task, and one that fails in two
non-obvious ways if you reach for `eSpace.Entities` (which is **local-only**).
This transcript is a **real captured run** against a live module (`FloorPlan`,
a Traditional Web app) on 2026-06-03. The request envelopes have since been
updated to the current contract (a `sessionToken` from `createSessionToken`,
arguments wrapped in `input`, capitalised validation `type` values); the model
output and messages are as captured.

The recipe itself lives in
[`../reference/patterns-by-element.md`](../reference/patterns-by-element.md) §
"FK to a System / referenced entity". The lambda-shape contract is in
[`../reference/lambda-contract.md`](../reference/lambda-contract.md); the
`IEntity` vs `IEntitySignature` rule is in
[`../reference/model-api-tips.md`](../reference/model-api-tips.md) § "Signature
Data Types".

> **Note on this run.** `FloorPlan`'s only *local* entities are static
> (`MenuItem`, `MenuSubItem`) — its server entities are all *referenced* from
> `FloorPlan_CS`. So to exercise the canonical "FK on a local `IServerEntity` →
> System `User`" shape, this run first creates a throwaway local server entity
> `FkVerifyTmp` to host the FK. In a production task you'd skip that and resolve
> the **existing** local entity instead (`eSpace.Entities.OfType<IServerEntity>().Named("Order")`);
> only the System-`User` resolution lines change nothing.

---

## User message

> Add a `CreatedBy` foreign key pointing at the System `User` entity, in the
> module I have open in Service Studio.

## Turn

### Call 1 — `getDataModel`

Read the model first. **`includeJson` is required** — always pass `"Never"`.

**Request** (call tool `getDataModel`; the host snapshots the open module on this first call):

```jsonc
{ "input": { "eSpaceName": "FloorPlan", "includeJson": "Never" }, "sessionToken": "<token>" }
```

**Response** (excerpt — the relevant lines):

```csharp
// (System)/User is RESIDENT as a referenced signature — NOT in eSpace.Entities:
var user = (OutSystems.Model.Data.IServerEntitySignature)floorPlan.AddDependency(
    Services.ModelServices.ParseGlobalKey("uXCIR2Atc0+es3zYuZSnNw*LYIWpF+Ey0aicHD144NUTA"));

// The only LOCAL entities are static:
var menuItem = floorPlan.CreateStaticEntity("MenuItem");
var menuSubItem = floorPlan.CreateStaticEntity("MenuSubItem");
```

Two things to read off this: (1) `User` shows up under the module's
**references**, rendered as an `IServerEntitySignature` — confirming it is *not*
reachable through `eSpace.Entities` and that `(System)` is already a resident
reference (so the recipe's precondition holds). (2) The verbose receiver name
`floorPlan` and the `AddDependency(ParseGlobalKey(...))` form belong to the read
verb's *rendering*; when you emit a lambda you use the literal `eSpace` and the
human-readable `eSpace.References.Named("(System)")` idiom.

### Call 2 — `applyModelApiCode`

Submit a **full `eSpace => { ... }` lambda** (ending in `}`, no `eSpace.Save(...)`).
The System-`User` resolution is the three middle lines; the rest creates the
throwaway owner entity for this run.

**Request** (call tool `applyModelApiCode`):

```jsonc
{
  "input": {
    "eSpaceName": "FloorPlan",
    "code": "eSpace => { var verifyTarget = eSpace.CreateServerEntity(\"FkVerifyTmp\", eSpace.Key.CreateNewKeyBasedOnThis(new object[] { \"FkVerifyTmp\" })); var fkAttr = verifyTarget.CreateAttribute(\"CreatedBy\"); var _System_ = eSpace.References.Named(\"(System)\"); var user = _System_.Entities.OfType<OutSystems.Model.Data.IServerEntitySignature>().Named(\"User\"); fkAttr.DataType = user.IdentifierType; fkAttr.IsMandatory = true; }",
    "imports": []
  },
  "sessionToken": "<token>"
}
```

Note what is **absent**: no `eSpace.RefreshDependency(user.GlobalKey)` — in the
MCP host that throws, and it is unnecessary for an already-resident reference.

**Response** (real):

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

Empty `exceptionMessage` + a non-empty `mutatedOmlPath` ⇒ the FK was saved and the
session pointer advanced; that path is what you hand to `omlMerge`. The lone
`validationMessages` entry has `type: "Warning"` — **pre-existing** (the
`SimpleColorPicker` HTML-injection note), not introduced by the FK — confirmed
by Call 3b below. There is no `type: "Error"` entry, so this is not a
saved-but-invalid case.

### Call 3 — verify (the load-bearing read-back)

An empty `exceptionMessage` only proves the host's `Save` ran — it is
indistinguishable from a silent no-op. Read the model back and confirm the FK
exists **and** that its `DataType` is `User`'s identifier type.

**Request** (`getDataModel` again, `includeJson` still required, still `"Never"`):

```jsonc
{ "input": { "eSpaceName": "FloorPlan", "includeJson": "Never" }, "sessionToken": "<token>" }
```

**Response** (excerpt — the FK is present and correctly typed):

```csharp
/*** creating IServerEntity 'FkVerifyTmp' ***/
var fkVerifyTmp = floorPlan.CreateServerEntity("FkVerifyTmp");

var createdBy = fkVerifyTmp.CreateAttribute("CreatedBy");
createdBy.DeleteRule = OutSystems.Model.Enumerations.DeleteRule.Protect; // default
createdBy.IsMandatory = true;
// (System)/User
var user = (OutSystems.Model.Data.IServerEntitySignature)floorPlan.AddDependency(
    Services.ModelServices.ParseGlobalKey("uXCIR2Atc0+es3zYuZSnNw*LYIWpF+Ey0aicHD144NUTA"));
createdBy.DataType = user.IdentifierType;   // ← bound to User's identifier type ✓
```

### Call 3b — `getValidationMessages`

`getValidationMessages` takes no `includeJson` (an optional `filter` is allowed):

```jsonc
{ "input": { "eSpaceName": "FloorPlan" }, "sessionToken": "<token>" }
```

**Response** (real) — only the pre-existing warning; **no new** errors from the FK:

```jsonc
[
  {
    "id":        "...",
    "type":      "Warning",
    "message":   "HTML Injection",
    "detail":    "type: InlineExpression, location: /Common/SimpleColorPicker/If/False/Expression",
    "ownerKey":  "...",
    "ownerPath": "/Common/SimpleColorPicker/If/False/Expression",
    "ownerType": "..."
  }
]
```

### Call 4 — auto-merge into Service Studio

Take the `mutatedOmlPath` from the last successful `applyModelApiCode` and call
`omlMerge` with it — the default closing step (see [`../SKILL.md`](../SKILL.md)
§ 3.1). *(In this verification run, `FkVerifyTmp` is a throwaway — reject it in
the merge.)*

```jsonc
// call tool omlMerge
{ "input": { "eSpaceName": "FloorPlan", "mutatedOmlPath": "C:\\Users\\…\\<runId>-out.oml" }, "sessionToken": "<token>" }
// → { "merged": true, "message": "..." }
```

Service Studio merges the diff into the module — through the Compare-and-Merge
window for the user to review and accept (or reject), or with no dialog, per
their setting.

---

## Pass predicate (this is what "verified" means here)

1. `applyModelApiCode` → empty `exceptionMessage`, no `type: "Error"` entry in
   `validationMessages`, and the out file was written (pointer advanced). ✅
2. Read-back via `getDataModel` shows the `CreatedBy` FK **and**
   `createdBy.DataType = user.IdentifierType` with `user` = `(System)/User`. ✅
3. `getValidationMessages` shows no *new* errors (the one warning pre-dates the
   change). ✅

## Things to remember

- **`eSpace.Entities` is local-only.** `eSpace.Entities…Named("User")` throws
  `Unable to find object with Name equal to User in collection`. Go through
  `eSpace.References.Named("(System)")`.
- **Cast referenced entities to `IServerEntitySignature`**, not the local
  `IServerEntity` — the wrong cast makes `OfType<>` silently filter the entity
  out, so `.Named("User")` throws a misleading "not found".
- **`fkAttr.DataType = user.IdentifierType`** is the payoff line — it binds the
  FK to `User`'s identifier type.
- **Do not call `eSpace.RefreshDependency(...)`** in the MCP host for an
  already-resident reference — it throws and is unnecessary.
- **Two throw points, both the same `.Named<T>` throw-on-miss**: a missing
  `(System)` reference makes even `References.Named("(System)")` throw. See the
  recipe's "two fail-hard traps".
