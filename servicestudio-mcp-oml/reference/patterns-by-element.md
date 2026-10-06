# Common patterns by element type

The inline sketches below show the **statements that go inside** a full
`eSpace => { ... }` lambda. When you send them to `applyModelApiCode`, wrap
them in `eSpace => { ... }` (no `eSpace.Save(...)` — the host appends Save for
you). For full worked versions, the matching file in
[`../examples/`](../examples/) is authoritative — each is **already** a full
`eSpace => { ... }` lambda with **no** `eSpace.Save`, so you can use it verbatim
as `code` and lift its `using` directives into `imports`. See
[`lambda-contract.md`](lambda-contract.md) for the contract.

## Server entity + attribute

```csharp
var customer = eSpace.CreateServerEntity("Customer",
    eSpace.Key.CreateNewKeyBasedOnThis(new object[] { "Customer" }));
var name = customer.CreateAttribute("Name");
name.DataType = eSpace.TextType;
name.Length = 200;
```

Full: [`../examples/AddAttribute.cs`](../examples/AddAttribute.cs),
[`../examples/AddForeignKeyAttribute.cs`](../examples/AddForeignKeyAttribute.cs)
(both already full `eSpace => { ... }` lambdas with no `eSpace.Save` — usable verbatim as `code`).

`CreateServerEntity` also has a **single-arg** overload — `eSpace.CreateServerEntity("Customer")`
— which is verified working and saves you the key line.

### Creating an entity from scratch: identifier rules (re-verified live 2026-09-30)

A brand-new entity has **no attributes and no identifier**. Do it in this order:

```csharp
var e = eSpace.CreateServerEntity("AuditLog");                 // single-arg is fine
var aId = e.CreateAttribute("Id");
aId.DataType = eSpace.LongIntegerType;
aId.IsAutoNumber = OutSystems.Model.Enumerations.AutoNumber.Yes;  // an enum, not a bool
e.IdentifierAttribute = aId;                                   // typed setter works
var n1  = e.CreateAttribute("EntityName"); n1.DataType = eSpace.TextType; n1.IsMandatory = true;
// … remaining attributes …
e.ExposeReadOnly = true;
e.Public = false;
```

1. **An entity with no attributes / no identifier does not survive the save.** Create
   it, and the next read finds it **gone** (`.First(…)` → *"Sequence contains no
   matching element"*). Entity + attributes + identifier must land in **one lambda** —
   you cannot create the shell in one call and fill it in the next.
2. **Set the identifier with the typed `entity.IdentifierAttribute = attr`.** Verified
   on 2026-09-30 for server and static entities (`IdentifierType` then resolves to
   `<Entity> Identifier`). Older guidance said this setter crashed and prescribed a
   reflection write to the impl's `Identifier` property; reflection is now rejected by
   the MCP sandbox ([`lambda-contract.md`](lambda-contract.md) § "Sandbox") and the typed
   setter is the way.
3. **Set the identifier before touching entity actions** — the eight auto-actions
   (`CreateAuditLog`, `GetAuditLog`, `UpdateAuditLog`, `CreateOrUpdateAuditLog`,
   `DeleteAuditLog`, `DeleteAllAuditLogs`, `GetAuditLogForUpdate`,
   `CreateOrUpdateSomeAuditLogs`) are generated from it. Read them through the typed
   getters: `e.CreateAction`, `e.GetAction`, `e.CreateOrUpdateAction`,
   `e.DeleteAction`, `e.DeleteAllAction`, …
4. **`IsAutoNumber` is an `AutoNumber` enum** — `= AutoNumber.Yes` works; `= true`
   doesn't compile.
5. **One Binary Data attribute per entity** — a second one is a TrueChange error
   `InvalidEntity_OneBin`.
6. **FK defaults:** an attribute typed `OtherEntity.IdentifierType` starts with
   `DeleteRule = Protect` and `IsMandatory = false` (`DeleteRule` ∈ `Protect` /
   `Delete` / `Ignore` — there is no Cascade).

Attribute creation itself is clean and typed throughout: `e.CreateAttribute("X")` (no
key argument) then `.DataType` / `.IsMandatory` / `.Length`.

## Find an existing entity, mutate it

```csharp
var customer = eSpace.Entities
    .OfType<OutSystems.Model.Data.IServerEntity>()
    .Named("Customer");
var email = customer.CreateAttribute("Email");
email.DataType = eSpace.EmailType;
```

Cast to `IServerEntity` (DB-backed), `IClientEntity` (local storage), or
`IStaticEntity` (static records) depending on the entity. See
[`model-api-tips.md`](model-api-tips.md) § "Signature Data Types" for the
`IEntity` vs `IEntitySignature` distinction.

> **`eSpace.Entities` is local-only.** It does **not** contain System or other
> *referenced* entities. For a foreign key to a referenced entity (e.g. the
> standard `User`), see the **"FK to a System / referenced entity"** section
> below — you must resolve it through `eSpace.References.Named(...)` and cast to
> the `*Signature` interface.

**Don't sweep the collection untyped.** `eSpace.Entities` mixes server, client
and static entities, so a server-entity property setter applied to whatever the
iteration happens to yield throws
`System.InvalidOperationException: Object is not a Server or Client entity`
(catchable — it surfaces in `exceptionMessage`, it does not crash the runner).
The `OfType<>()` filter above is what makes the pattern safe; keep it even when
you are iterating rather than picking one entity by name:

```csharp
// ✅ filtered — only local server entities reach the setter
foreach (var e in eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>())
{
    e.Description = "reviewed";
}

// ❌ untyped sweep — throws on the first static or client entity
foreach (var e in eSpace.Entities) { /* server-entity setters here */ }
```

## FK to a System / referenced entity (e.g. `User`)

`eSpace.Entities` is **local-only** — System and other *referenced* entities are
not in it. Reach a referenced entity through its reference, then cast to the
**`*Signature`** interface (referenced entities are read-only signatures, not the
local `IServerEntity`):

```csharp
// 1. The local entity that will own the FK.
var order = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Order");
var fkAttr = order.CreateAttribute("CreatedBy");

// 2. Resolve the referenced System entity (eSpace.Entities is LOCAL-ONLY).
//    PRECONDITION: the module already references (System); if not, the next line
//    throws like a missing-entity .Named() — see trap 2.
var _System_ = eSpace.References.Named("(System)");
//    Referenced entities are SIGNATURES → IServerEntitySignature, NOT IServerEntity.
var user = _System_.Entities.OfType<OutSystems.Model.Data.IServerEntitySignature>().Named("User");

// 3. The payoff: bind the FK's type to User's identifier type.
fkAttr.DataType = user.IdentifierType;
fkAttr.IsMandatory = true;
// Do NOT call eSpace.RefreshDependency(user.GlobalKey) in the MCP host — it
// throws there and is unnecessary for an already-resident reference.
```

References live under `eSpace.References.Named("<ModuleName>")`. Verified for
`"(System)"` (FK to `User`); `"RichWidgets"` / `"OutSystemsCharts"` are
plausible but **unverified** for this FK path.

**Why the `*Signature` cast matters.** A referenced entity surfaces only as
`IServerEntitySignature`. Cast to the local `IServerEntity` (or the wrong
signature kind — e.g. `IStaticEntitySignature` for a non-static entity) and
`OfType<>` silently filters it out, so `.Named("User")` then throws
`Unable to find object with Name equal to User in collection` — a "not found"
that really points at the wrong cast, not a wrong name. See
[`model-api-tips.md`](model-api-tips.md) § "Signature Data Types" for the
`IEntity` vs `IEntitySignature` rule.

**Two fail-hard traps — both are the same `.Named<T>` throw-on-miss** (it throws;
it does not return `null`):

1. **Skipping the `References` hop.** `eSpace.Entities…Named("User")` throws
   `Unable to find object with Name equal to User in collection`, because `User`
   is not a local entity.
2. **No `(System)` reference resident.** `eSpace.References.Named("(System)")`
   itself throws `Unable to find object with Name equal to (System) in
   collection` when the module hasn't loaded the `(System)` reference. Adding the
   reference re-enters the `RefreshDependency`-throws territory noted above —
   resolve that separately.

> **Source note:** the worked example
> [`../examples/AddDashboardScreenWithDonutChart.cs`](../examples/AddDashboardScreenWithDonutChart.cs)
> shows this idiom at lines 14-15 but calls `eSpace.RefreshDependency(user.GlobalKey)`
> on **line 16** — the call this recipe omits. That line is the ODC / full-lambda
> form; in the MCP host it throws and is unnecessary for an already-resident
> reference, so don't port it.

## Server action

```csharp
var act = eSpace.CreateServerAction("CalculateDiscount");
var input = act.CreateInputParameter("Amount");
input.DataType = eSpace.DecimalType;
var output = act.CreateOutputParameter("Discounted");
output.DataType = eSpace.DecimalType;
```

The typed signature API above is verified — `CreateInputParameter("X").DataType = …`,
`CreateOutputParameter`, and `CreateLocalVariable("R").DataType = someEntity` all work
with no key argument and no reflection. Likewise `assignNode.CreateAssignment("var", "expr")`
(the two-string overload) for assignments.

> **A freshly created server / client / screen action has an EMPTY `Nodes`
> collection — no Start, no End.** Create them with the typed factory:
>
> ```csharp
> var start = act.CreateNode<OutSystems.Model.Logic.Nodes.IStartNode>();
> // … nodes via ConnectedBelow(…) …
> act.CreateNode<OutSystems.Model.Logic.Nodes.IEndNode>().ConnectedBelow(lastNode);
> ```
>
> Other kinds are **not** empty (verified 2026-09-30) — check `Nodes` before adding a Start:
>
> | Created with | Default nodes |
> |---|---|
> | `eSpace.CreateServerAction`, `CreateClientAction`, `screen.CreateScreenAction` | none |
> | `eSpace.CreateServiceAction` | 1 |
> | exposed REST method (`restService.CreateAction`) | **Start + End** — adding another Start gives `InvalidFlow_TooManyNodes: More than one Start found`; delete them first (`m.Nodes.ToList().ForEach(x => x.Delete())`) or build from the existing Start |
>
> A missing Start is a TrueChange error `InvalidFlow_NoStart`. Unlike a bare entity, an
> action **with a signature but no body does survive the save** — so splitting "create
> signature" and "build body" across two calls is safe.

> **An exception handler needs its own End.** Its path may not reach the main End —
> TrueChange reports `InvalidFlow_BadErrHandlerPath` / `InvalidFlow_MalformedAction`.
> Create a second `IEndNode` for every handler path; set `AbortTransaction = false` when the
> handler recovers. Details and the rest of the logic nodes (SQL, For Each, REST, email, timers):
> [`logic-and-integrations.md`](logic-and-integrations.md).

Full versions including node placement:
[`../examples/AddServerAction*.cs`](../examples/),
[`../examples/AddClientActionWithValidation.cs`](../examples/) (full-lambda
form).

## Aggregate in a server action (auto-rename trap)

Create the aggregate node inside the action, add a source entity, then (optionally) a
filter. Build **every** downstream expression from the node's *actual* name, read back
**after** the filter — never from the string you passed to `CreateNode`:

```csharp
var act = eSpace.ServerActions.Named("GetEventRegistrationCount");
var startNode = act.CreateNode<OutSystems.Model.Logic.Nodes.IStartNode>();

var registration = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Registration");

// Create + source + filter.
var agg = act.CreateNode<OutSystems.Model.Logic.Nodes.IAggregateNode>("GetRegistrations").ConnectedBelow(startNode);
agg.AsDatabaseAggregate.CreateSource(registration);
agg.AsDatabaseAggregate.CreateFilter("Registration.EventId = EventId");

// CRITICAL: re-read the name. CreateFilter may have auto-renamed the node (see below).
var aggName = agg.Name;            // e.g. "GetRegistrationsByEventId", NOT "GetRegistrations"

var assign = act.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>().ConnectedBelow(agg);
assign.CreateAssignment("Count", aggName + ".List.Length");   // build from aggName, never the literal
```

**The auto-rename trap (empirically verified in Service Studio, 2026-06-05).**
Service Studio re-derives an aggregate's name on `CreateFilter` **only when both** conditions
hold: (1) the node's current name equals Service Studio's *default* for that source —
`Get<EntityPlural>` (e.g. `GetZzBookings` for an entity whose plural label is `ZzBookings`);
**and** (2) the filter is in the navigation shape `«Entity».«Attr» = «matching input/variable»`
(the `GetXByY` convention). When both hold, the node is renamed to `Get<EntityPlural>By<Attr>`.
A constant/literal filter does **not** rename it, and any custom (non-default) name is always
preserved — so the behaviour depends on *both* the name you chose and the filter shape:

| `CreateNode` name | filter | resulting `node.Name` |
|---|---|---|
| _(unnamed)_ | any | `Aggregate1` — Model-API default is `AggregateN`, never auto-renamed |
| `GetZzBookings` (== default `Get<Plural>`) | `ZzBooking.Name = "x"` (literal) | `GetZzBookings` — not renamed |
| `GetZzBookings` (== default `Get<Plural>`) | `ZzBooking.EventId = EventId` (attr = input) | `GetZzBookingsByEventId` — **renamed** |
| custom name (e.g. `CountBookings`) | any | preserved — not renamed |

Any expression that hard-codes the originally-passed name then produces a **saved-but-invalid**
model. Reproduced live: an assignment `Cnt = GetNonExistentAgg.List.Length` returned an empty
`exceptionMessage` and a saved OML, but `validationMessages` contained a `type: "Error"` entry
(`"Invalid Expression … Can't identify 'GetNonExistentAgg' element"`) — the
saved-but-invalid case (see SKILL.md § 3, "Saved-but-invalid"). The `var aggName = agg.Name;`
read-back above is immune regardless of whether the host renamed the node.

Full worked version (no filter, so no rename):
[`../examples/AddServerActionWithAggregate.cs`](../examples/AddServerActionWithAggregate.cs);
filter-only sketches: [`../examples/AddFilter.cs`](../examples/AddFilter.cs),
[`../examples/Filter.cs`](../examples/Filter.cs) (already full `eSpace => { ... }` lambdas with no `eSpace.Save` — usable verbatim as `code`).

## Grouped aggregate: reference output columns by their alias (not the entity path)

Once an aggregate has a **Group By**, its output columns are addressed by the
**alias you passed to `CreateGroupByAttribute("…")` / `CreateAggregatedAttribute("…")`** —
`Agg.List.Current.«Alias»` — **not** by the source-entity path. Referencing the
grouped attribute through its entity (`Agg.List.Current.«Entity».«Attr»`) is a
**saved-but-invalid** trap: the mutation returns an empty `exceptionMessage` and a
saved OML, but `getValidationMessages` then reports
`ExpressionUnknownObject: "Can't identify '«Entity»' element in expression."`
(verified live 2026-07-20 — cost a whole build/validate round-trip).

```csharp
var byStage = scr.CreateScreenAggregate(false, "GetForecastByStage");
var dba = byStage.AsDatabaseAggregate;
var oppSrc   = dba.CreateSource(opportunity);                 // IServerEntity
var stageSrc = dba.CreateSource(opportunityStage);            // IStaticEntity — both derive IEntity
var j = dba.CreateJoin(); j.LeftSource = oppSrc; j.RightSource = stageSrc;
j.JoinType = OutSystems.Model.Enumerations.JoinType.Inner;
j.SetCondition("Opportunity.OpportunityStageId = OpportunityStage.Id");
dba.CreateGroupByAttribute("StageLabel").SetAttribute("OpportunityStage.Label");   // alias = "StageLabel"
var f = dba.CreateAggregatedAttribute("StageForecast");                            // alias = "StageForecast"
f.SetAttribute("Opportunity.ExpectedRevenue"); f.AggregationType = AggregationType.Sum;

// ✅ reference by alias:            GetForecastByStage.List.Current.StageLabel
//                                  GetForecastByStage.List.Current.StageForecast
// ❌ NOT the entity path:           GetForecastByStage.List.Current.OpportunityStage.Label   (fails validation)
```

**`AggregationType` member names are literal — the average is `Avg`, not `Average`.**
`AggregationType.Average` **compiles** and then hard-crashes the sidecar runner (empty
`exceptionMessage`, empty `mutatedOmlPath`, mutation rolled back — verified live
2026-08-03). `Sum`, `Count`, `Max` and `Avg` are verified live; `Min` and `None` are listed
in the generated enum but untested. Take the
spelling from [`../docs/OutSystems.Model.Enumerations.Generated.cs`](../docs/OutSystems.Model.Enumerations.Generated.cs)
rather than guessing a synonym; see
[`reactive-widget-api.md`](reactive-widget-api.md) § "Runner-crash traps".

The alias is whatever string you chose — it does **not** have to match the attribute
name. (`examples/AddDashboardScreenWithDonutChart.cs` binds a chart to `"Label"`/`"Count"`
— those are the group-by/aggregated *aliases*, which is why the binding resolves.)
`getValidationMessages` after the UI mutation is the cheapest way to catch a wrong path.

## Group an aggregate by a computed value (e.g. month buckets)

`CreateGroupByAttribute("Alias").SetAttribute(...)` accepts an **expression**, not only an
attribute path — so you can bucket without a real column. Use only **SQL-translatable** functions
(`Year(x)`, `Month(x)`, `Day(x)`, arithmetic, string concat), NOT `FormatDateTime` (not translatable
in an aggregate). Verified 2026-07-28:

```csharp
dba.CreateGroupByAttribute("MonthLabel").SetAttribute("Month(Order.CreatedOn) + \"/\" + Year(Order.CreatedOn)");
var mc = dba.CreateAggregatedAttribute("MonthCount"); mc.SetAttribute("Order.Id"); mc.AggregationType = AggregationType.Count;
```

Traps:
- **`CreateCalculatedAttributeInGroupBy` throws "not possible … without a group by"** when the aggregate has no
  `CreateGroupByAttribute` yet — it adds an extra column to an *existing* group-by, it is NOT the group key.
- **Sorting a grouped aggregate by a group-by ALIAS fails validation.** `dba.CreateSort().SetAttribute("MonthLabel")`
  saves but `getValidationMessages` then reports `ExpressionUnknownObject: Can't identify 'MonthLabel'` (verified
  2026-07-28).
- **✅ Sorting by the group-by EXPRESSION works** (verified live 2026-07-31, validation clean). Repeat the same
  expression you grouped on, verbatim, in the sort — not its alias. Add a sortable numeric key alongside the
  display label and sort on the key's expression:
  ```csharp
  dba.CreateGroupByAttribute("MonthKey").SetAttribute("Year(Order.CreatedOn) * 100 + Month(Order.CreatedOn)");
  dba.CreateGroupByAttribute("MonthLabel").SetAttribute("Month(Order.CreatedOn) + \"/\" + Year(Order.CreatedOn)");
  var ms = dba.CreateSort(); ms.SetAttribute("Year(Order.CreatedOn) * 100 + Month(Order.CreatedOn)");
  ms.SortDirection = Sort.Ascending;
  ```
  This matters for any time-series chart: `mapTo` preserves list order, so without a working sort the line/column
  chart renders months out of sequence. Sorting in the consuming widget is no longer necessary.
- **Date vs Date Time in a filter: `= NullDate()` validates, `<` / `>` against `CurrDate()` does NOT.** A Date Time
  attribute compared to `CurrDate()` fails `OperatorIncompatibleTypes: Cannot apply '<' operator to 'Date Time'
  together with 'Date'` (verified 2026-07-31) — use **`CurrDateTime()`**. Because the equality form silently passes,
  a filter can look fine and still break on the ordering comparison. **Print the attribute's `DataType` in the same
  lambda that builds the filter** instead of guessing which of Date / Date Time / Time it is.

## Record → Text: `JSONSerialize` is a flow NODE, not an inline function

You cannot write `JSONSerialize(OrderRecord)` inside an argument expression — the
inline form does not exist in OutSystems 11 and fails at publish/runtime, not at
edit time, so the Model API gives you no warning. Whenever a record / entity row /
structure / list has to land in a `Text` parameter, add a node:

```csharp
var jsonNode = iAction.CreateNode<OutSystems.Model.Logic.Nodes.IJSONSerializeNode>("JSONSerializeOrder");
jsonNode.SetData("OrderRecord");   // typed setter (verified 2026-09-30); the old reflection SetRecord is sandboxed out

// Splice it in, then reference its output downstream as <NodeName>.JSON:
auditNode.SetArgumentValue(snapshotParam, "JSONSerializeOrder.JSON");
```

The node name becomes the output reference prefix, and the single implicit `Text`
output is always called `JSON`. The inverse direction (`Text` → record) is
`IJSONDeserializeNode`, same family. Both live in
`OutSystems.Model.Logic.Nodes` alongside `IAssignNode` / `IIfNode` — built-in flow
operators are **nodes**, not actions, so they never appear in `eSpace.SystemActions`
or `eSpace.References`. The full list of node interfaces is in
[`../docs/OutSystems.Model.Logic.Nodes.Generated.cs`](../docs/OutSystems.Model.Logic.Nodes.Generated.cs)
(plus `OutSystems.Model.Logic.Mobile.Nodes` for the Reactive `ISendEmailNode`).

After adding one, re-run the layout pass — the flow gained a node.

## Raising exceptions: user **and role** exceptions (not system exceptions)

`raiseNode.Exception` accepts a **user** exception (`eSpace.UserExceptions.Named(...)` /
`eSpace.CreateUserException(...)`). Assigning a built-in **system** exception (`eSpace.SecurityException`,
`DatabaseException`, …) throws *"ISystemException cannot be used as the value for … RaiseExceptionNode.Exception"*
(verified 2026-07-28). To signal a security failure, create/raise a user exception (e.g. named `SecurityException`).

**It also accepts a role exception** (`OutSystems.Model.Logic.IRoleException`) — verified
live 2026-07-30 across 15 role-gate splices. This is what you want for an authorisation
gate, since the platform already ships one per role:

```csharp
// Role exceptions are AUTO-ASSOCIATED — one per role. There is NO CreateRoleException
// method on the runtime Role impl; look the existing one up instead.
var roleEx = eSpace.RoleExceptions.First(re => re.Role != null && re.Role.Name == "OrderRepresentative");  // typed (verified 2026-09-30)

var raise = iAction.CreateNode<OutSystems.Model.Logic.Nodes.IRaiseExceptionNode>();
raise.Exception = roleEx;
raise.SetExceptionMessage("\"Not authorized.\"");   // OutSystems string literals keep their quotes
```

`IRoleException` has no `Name` of its own — match on `.Role.Name`. Note the `getRoleExceptions` read tool
**returns an empty body even when role exceptions exist** — don't use it to test for
presence; enumerate `eSpace.RoleExceptions` instead.

**Role-gate splice shape.** To gate an action, insert an `IIfNode` between the Start node
and its current target, so the check runs before any business logic:

```csharp
var iStart = (OutSystems.Model.Logic.Nodes.IStartNode)startNode;
var oldTarget = iStart.Target;
var gate = iAction.CreateNode<OutSystems.Model.Logic.Nodes.IIfNode>(null, null);
gate.SetCondition("CheckOrderManagerRole() or CheckOrderRepresentativeRole()");
iStart.Target    = gate;
gate.TrueTarget  = oldTarget;    // original flow continues
gate.FalseTarget = raise;
// then verify: gate.Links.Count should be 2 (the .Target no-op trap applies)
```

`Check<RoleName>Role()` is the generated expression function for each role.

## Recovering from a saved-but-invalid action

If a prior mutation left the model with a validation error (the saved-but-invalid
outcome — surfaced either as a `type: "Error"` entry in `applyModelApiCode`'s
`validationMessages`, or via the same in `getValidationMessages` — e.g.
an expression built from an aggregate name the host auto-renamed), delete the broken
action and recreate it rather than patching the bad expression in place (verified working):

```csharp
eSpace.ServerActions.First(a => a.Name == "GetEventRegistrationCount").Delete();
// …then recreate the action correctly, reading aggName back as shown above.
```

Re-read with `getValidationMessages` after the recreate to confirm no entry has `type` equal to `"Error"`.

## Silent renames — read `.Name` back after every create

The model **never refuses a name**; it quietly picks another one (verified live 2026-09-30):

- **A taken name gets a numeric suffix.** Creating a second server action `DoThing` yields `DoThing2`.
- **Entity-action names are taken once the entity has an identifier.** With entity `Order` identified,
  `eSpace.CreateServerAction("GetOrder")` → `GetOrder2`, `"CreateOrUpdateOrder"` → `CreateOrUpdateOrder2`.
  (Before the identifier exists, `GetOrder` is accepted — and then collides later.)
- **Attribute names are cut to 28 characters** — `ThisIsAVeryLongAttributeNameBeyondTwentyEight` becomes
  `ThisIsAVeryLongAttributeName`, and every expression you write with the long name is unbound.
- **Default-named aggregates are renamed when a filter is added** — see the next section.
- An input parameter named exactly like an entity (`Order`) is accepted and not flagged by TrueChange;
  avoid it anyway — it shadows the entity in expressions.

So: after creating anything you will reference by name, use the object's `.Name` (not the string you
asked for) when building expressions, and report it in your throw-probe or read-back. Rename back with
the typed setter if needed (`action.Name = "…"`, `agg.Name = "GetOrders"` — verified).

## Auto-created children — find and update, never add

Several setters create child objects for you; adding your own is a duplicate:

- `execNode.Action = someAction` creates one argument per input — set values with
  `execNode.SetArgumentValue(param, "expr")`.
- `blockInstance.SourceBlock = block` creates the arguments and placeholders.
- `staticRecord.SetAttributeValue(attr, "expr")` updates the value slot the record already has; the value is
  an **expression string** (`"\"Pending\""` for Text, `"1"` for Integer).
- `CreateRole("X")` adds the new role to the `Roles` of **every existing screen** (verified: `Screen1`,
  `Login`, `InvalidPermissions` all listed `Manager` afterwards) — revisit screen access after adding a role.

## Screen access: public screens and role gating

Verified live 2026-09-30 on a Reactive module:

- A **new screen** starts with `Registered` **plus every app role** in `screen.Roles`.
- **Role-gate** a screen by removing what must not see it: `scr.Roles.Remove(eSpace.RegisteredRole)`
  (and any other role), then make sure the intended role is present.
- **Public (anonymous) screen: add the Anonymous system role — `scr.Roles.Add(eSpace.AnonymousRole)`.**
  That is what Service Studio's *Anonymous* role checkbox stores; the template's public screens
  (`Login`, `InvalidPermissions`) carry it. Service Studio also keeps `Registered` ticked alongside it
  (the merge adds it if you don't), so a public screen reads `Anonymous | Registered | …`.
- **Don't use `scr.AnonymousAccess`.** It is not what Service Studio's screen properties show:
  setting it to `true` looks right in the saved copy (and silences a TrueChange warning), but
  **`omlMerge` drops it** — after the merge the live screen is still authenticated-only (verified
  live 2026-09-30: set, merged with `merged: true`, read back `AnonymousAccess = False`).
- **Verify access on the live module after the merge**, not only on the `mutatedOmlPath` copy: after
  `omlMerge` the pointer is cleared, so a read (or throw-probe) shows what Service Studio actually kept.
- System roles: `eSpace.AnonymousRole`, `eSpace.RegisteredRole`, `eSpace.SystemRoles`. `eSpace.Roles` lists
  only the module's own roles.
- The `AnonymousAccess_NoPermissionsNoRoles` warning ("only accessible by authenticated users, but there are
  no roles") appears on screens of a module that has no roles of its own; it disappears once a role exists.
  Don't read it as proof of whether a screen is public.

Example: [`../examples/SetScreenAccess.cs`](../examples/SetScreenAccess.cs).

## Screens, screen actions, widgets

Screens and widgets use `IMobileScreen`, `IMobileWidget` (the Model API
names — even for non-mobile reactive screens). See
[`../examples/AddEditScreen.cs`](../examples/AddEditScreen.cs) and the rest
of the `Add*Screen*.cs` family.

For **building a Reactive screen from scratch** — layout scaffold, KPI tiles from the
`Card` block, data-bound `List` rows, dynamic inline styles (progress/CSS-bar charts),
and the chart-vs-no-chart-dependency decision — see
[`reactive-ui-creation.md`](reactive-ui-creation.md). (Traditional Web UI creation is a
separate file, [`traditional-ui-creation.md`](traditional-ui-creation.md).)

## Node positioning in flows

Use the `Above` / `Below` / `ToTheLeftOf` / `ToTheRightOf` helpers on
`OutSystems.Model`, or the `ConnectedAbove` / `ConnectedBelow` variants
when you also want to wire the Target. **Don't** set `Target` manually
when a `Connected*` helper would do — the latter handles both logical and
visual layout. See [`model-api-tips.md`](model-api-tips.md) §
"Manipulating Nodes in Logic Flows".

**Completion gate:** any action with new or spliced nodes must get a full BFS
layout pass before merge (`ArrangeAllNodes()` alone is not enough), and splice
links must be verified because `.Target` silently no-ops on fresh
`CreateNode`'d nodes. Full rule in SKILL.md § "Flow layout is a completion
gate".
