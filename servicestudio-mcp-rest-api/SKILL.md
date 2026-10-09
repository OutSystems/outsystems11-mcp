---
name: servicestudio-mcp-rest-api
description: >-
  Expedites exposing a full REST API (an Exposed REST API integration, O11's
  "Integrations > REST" feature) over an existing O11 server entity via the
  Service Studio MCP `applyModelApiCode` tool — List/Get/Create/Update/Delete
  wired straight to the entity's own auto-generated CRUD actions, plus
  composite "create a header with detail lines in one call" endpoints for
  master-detail entities (ForEach + accumulator pattern). Captures gotchas
  discovered the hard way (undocumented `CreateIntegration<IRestService>` entry
  point, the "More than one Start" / "End must have at least one incoming
  connector" node trap, the leading-slash `URLPath` requirement, the disallowed
  `imports` namespace, the one-Body-input-per-method limit, the "don't splice a
  node into an already-merged flow" trap, and the real parameter names of an
  entity's Get/Create/Update/Delete actions) so a REST API gets built in one or
  two clean applies instead of three or four. Use
  whenever the user asks to "expose an entity as a REST API", "create a REST
  API for X", "add CRUD endpoints for X", "build a full API exposing this
  entity", "create an endpoint that creates an order with its lines", or
  similar, in an O11 module open in Service Studio. Companion to
  `servicestudio-mcp-oml` — depends on it for the full-lambda contract, tool
  names, and session-pointer mechanics; load that skill first if you haven't
  already.
license: MIT
compatibility: Requires the `servicestudio` MCP server connected to a running EAP Service Studio build with an O11 module open, and the `servicestudio-mcp-oml` skill (same `applyModelApiCode` contract, same `eSpaceName`/`sessionToken` conventions — this skill does not restate them).
metadata:
  author: arjanwaard
  origin: distilled from a live session building ShippingCompanyAPI over a ShippingCompany entity in module "aademo" (2026-09-11) — three applyModelApiCode attempts before it saved cleanly; extended the same session building OrderAPI's composite CreateOrder endpoint (Order + OrderLine header/detail) — one more gotcha (single Body input per method) found and folded in.
---

# Expose an O11 entity as a REST API

This is a companion to **`servicestudio-mcp-oml`** — read that skill first for the
full-lambda contract (`eSpace => { ... }`, no `eSpace.Save`), the tool catalogue
(`applyModelApiCode`, `getDataModel`, `runQuery`, `omlReset`, `omlMerge`), and the
session-pointer/silent-no-op traps. Everything below assumes you already know that
contract and is scoped to the one thing it doesn't cover: **Exposed REST APIs**
(`ServiceStudio.Plugin.RESTService` — a real, working part of the Model API that
isn't documented anywhere in `servicestudio-mcp-oml`'s reference files, because
nobody had gone looking for it until now).

## Contents

- [§ 1 — The five things that will bite you](#1-the-five-things-that-will-bite-you)
- [§ 2 — Before you write code: confirm the entity's action signatures](#2-before-you-write-code-confirm-the-entitys-action-signatures)
- [§ 3 — The recipe](#3-the-recipe)
- [§ 3.1 — Composite create for a master-detail entity pair](#31-composite-create-for-a-master-detail-entity-pair)
- [§ 4 — Worked examples](#4-worked-examples)
- [§ 5 — Recovery](#5-recovery)

## 1. The five things that will bite you

Each of these cost a failed `applyModelApiCode` call the first time. Read them
before you write code, not after the error comes back.

1. **Creating the API at all is undocumented but simple.** There is no
   `eSpace.RestServices` collection and no `eSpace.CreateRestService(...)` — the
   actual entry point is the *generic* integration factory:
   ```csharp
   var api = eSpace.CreateIntegration<ServiceStudio.Plugin.RESTService.IRestService>("MyAPI");
   ```
   `IRestService` (and its config enums — `Authentication`, `HttpSecurity`,
   `InternalAccess`, `ShowDocumentation`, `CrossSiteRequests`,
   `DefaultValueBehavior`, `Interoperability`, plus per-method `HTTPMethod`,
   `ReceiveIn`, `SendIn`) live in `ServiceStudio.Plugin.RESTService` and
   `ServiceStudio.Plugin.RESTService.Enumerations`. Full enum values and the
   `IRestService` / `IRestServiceAction` member list: [`reference/rest-service-api.md`](reference/rest-service-api.md).

2. **You cannot import that namespace.** `applyModelApiCode`'s `imports` array
   only accepts `OutSystems.Model` or its sub-namespaces —
   `ServiceStudio.Plugin.RESTService` is rejected outright:
   > `Import 'ServiceStudio.Plugin.RESTService' is not allowed. Only
   > 'OutSystems.Model' (or one of its sub-namespaces) may be imported.`
   Pass `imports: []` and fully-qualify every `ServiceStudio.Plugin.RESTService.*`
   type inline in the code string instead. This is more verbose but it's the
   only path that compiles.

3. **`api.CreateAction("Name")` already has a Start node and an End node.**
   Unlike `eSpace.CreateServerAction`, which starts genuinely empty, a fresh
   `IRestServiceAction` arrives with both wired in. This inverts the pattern
   you'd copy from `servicestudio-mcp-oml`'s server-action examples:
   - Creating a *second* Start (`action.CreateNode<IStartNode>()`) fails validation
     with `InvalidFlow_TooManyNodes: More than one Start found in <Name>`.
   - Creating a *second* End the naive way
     (`action.CreateNode<IEndNode>().ConnectedBelow(lastNode)`) leaves the
     **original** auto-End with no incoming connector, which saves cleanly but is
     invalid: `RequiredConnector_In: End must have at least one incoming connector`.
   - The fix is to reuse both:
     ```csharp
     var start = action.Nodes.OfType<OutSystems.Model.Logic.Nodes.IStartNode>().First();
     // ...build your flow ConnectedBelow(start)...
     var end = action.Nodes.OfType<OutSystems.Model.Logic.Nodes.IEndNode>().First();
     end.Below(lastNode);        // position only (Below, not ConnectedBelow — end already exists)
     lastNode.Target = end;      // wire the connector by hand
     ```

4. **`URLPath` needs a leading `/`.** `restAction.URLPath = "ShippingCompanies"`
   fails: `CustomObjectError: 'URL Path' property must start with '/'`. Use
   `"/ShippingCompanies"` / `"/ShippingCompanies/{Id}"`. (The sibling `URL`
   property, without "Path", is read-only/derived — don't set it.)

5. **Don't guess an entity action's parameter names — they're not what you'd
   expect.** `Get<Entity>`'s output parameter is named **`Record`**, not the
   entity's name (a very natural wrong guess). See § 2 for the one query that
   confirms all four CRUD actions' exact parameter names before you write the
   flow that calls them — cheaper than a failed apply + `omlReset` cycle.

6. **Only one input parameter per method may have `ReceiveIn = Body`.** Two
   `Body` inputs on the same `IRestServiceAction` (e.g. an `Order` record input
   *and* a separate `List<OrderLine>` input for a composite create endpoint)
   fails validation cleanly but late:
   > `CustomObjectError: Method '<Name>' can have only one input parameter with
   > 'Receive In' property set to 'Body'.`
   If an endpoint logically needs more than one thing in the body, wrap them in
   a **structure** first and take one structure-typed input instead:
   ```csharp
   var request = eSpace.CreateStructure("CreateOrderRequest");
   var reqOrder = request.CreateAttribute("Order"); reqOrder.DataType = order;         // a record field
   var reqLines = request.CreateAttribute("OrderLines"); reqLines.DataType = orderLineListType;  // a list field
   // ...one input parameter: requestInput.DataType = request; ReceiveIn = Body;
   // then reference fields as "Request.Order" / "Request.OrderLines" everywhere below.
   ```
   `URL`/`Header` inputs aren't affected by this limit — only `Body` is capped at one.

7. **Don't splice a node into an *already-merged* flow with `.ConnectedBelow(existingNode)`
   — it silently fails to rewire.** `ConnectedBelow` reliably sets `existingNode.Target`
   when building a *fresh* flow (existingNode's `Target` was still null), which is
   what every example in § 3 does. But calling it on a node that's already
   merged and already has a `Target` (e.g. inserting an "stamp the URL Id onto
   the body" Assign node between an existing `Start` and an existing
   `ExecuteServerActionNode` after the action was built and merged in an
   earlier call) leaves the **old** connector in place — even an explicit
   follow-up `existingNode.Target = newNode;` doesn't clear it reliably either.
   The result is a save-but-invalid pair of errors: the new node reports
   `RequiredConnector_In` (nothing points to it) and the old target reports
   `InvalidFlow_MalformedAction: Ambiguous paths to Run Server Action` (now two
   things point to it). Confirmed via a `runQuery` node-position dump — the
   spliced action had one extra node at a duplicate `(X, Y)` position, i.e. a
   real leftover.
   **Fix: don't splice — delete and recreate the action.**
   ```csharp
   api.Actions.Named("UpdateX").Delete();
   var updateX = api.CreateAction("UpdateX");   // fresh Start/End, build the whole flow as in § 3
   ```
   This is cheap (one REST action, a handful of nodes) and sidesteps the bug
   entirely. Only reach for manual splicing if you've confirmed in the specific
   case that the node you're inserting after genuinely has no `Target` yet.

## 2. Before you write code: confirm the entity's action signatures

Every O11 server entity with an identifier auto-generates `GetAction`,
`CreateAction`, `UpdateAction`, `DeleteAction` (plus a few more) as soon as it's
`Public`. Their parameter names are stable across entities but easy to misremember,
so confirm them with one `runQuery` call instead of guessing:

```jsonc
// tool: runQuery
{
  "eSpaceName": "<module>",
  "query": "Root { Entities { Name GetAction { Name OutputParameters { Name } InputParameters { Name } } CreateAction { Name InputParameters { Name } OutputParameters { Name } } UpdateAction { Name InputParameters { Name } } DeleteAction { Name InputParameters { Name } } } }"
}
```

Verified result shape (holds for any entity named `X`):

| Action | Input(s) | Output(s) |
|---|---|---|
| `GetX` (`entity.GetAction`) | `Id` | **`Record`** |
| `CreateX` (`entity.CreateAction`) | `Source` | `Id` |
| `UpdateX` (`entity.UpdateAction`) | `Source` (full record, `Id` included) | — |
| `DeleteX` (`entity.DeleteAction`) | `Id` | — |

There's no built-in "get all" action — build that endpoint with an
`IAggregateNode` instead (§ 3, List endpoint).

## 3. The recipe

Proven shape for "full CRUD API for entity X" — five REST actions on one
`IRestService` integration, plural URL segment, singular `{Id}` route for the
single-record ones:

| Action name | HTTP | `URLPath` | Body flows |
|---|---|---|---|
| `GetXs` | GET | `/Xs` | Aggregate over `X` → list output |
| `GetX` | GET | `/Xs/{Id}` | `Id` (URL) → `entity.GetAction` → `Record` |
| `CreateX` | POST | `/Xs` | `X` record (body) → `entity.CreateAction` → `Id` |
| `UpdateX` | PUT | `/Xs/{Id}` | `Id` (URL) + `X` record (body) → `entity.UpdateAction` |
| `DeleteX` | DELETE | `/Xs/{Id}` | `Id` (URL) → `entity.DeleteAction` |

Per-endpoint mechanics:

- **List** — no built-in action, so query directly:
  ```csharp
  var agg = listAction.CreateNode<OutSystems.Model.Logic.Nodes.IAggregateNode>("GetXList").ConnectedBelow(start);
  agg.AsDatabaseAggregate.CreateSource(entity);
  // ...Assign node: CreateAssignment("Xs", "GetXList.List")...
  ```
  List-typed output parameter: `eSpace.GetOrCreateListType(entity)`.

- **Get / Create / Update / Delete** — bind the entity's own CRUD action via
  `IExecuteServerActionNode`, exactly like a normal server action would (see
  `servicestudio-mcp-oml`'s `AddServerActionCreateOrUpdate.cs` /
  `AddServerActionDelete.cs` for the base pattern — same `SetArgumentValue`
  API applies here, just inside a REST action's own flow instead of a
  separate server action):
  ```csharp
  var exec = action.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("GetEntity").ConnectedBelow(start);
  exec.Action = entity.GetAction;
  exec.SetArgumentValue(entity.GetAction.InputParameters.Named("Id"), "Id");
  // ...Assign node: CreateAssignment("X", "GetEntity.Record")...
  ```

- **Record-typed parameters** — use the entity itself as `DataType` for a
  single-record input/output (`param.DataType = entity;`); use
  `eSpace.GetOrCreateListType(entity)` for the list output.

- **Config properties worth setting explicitly** on the `IRestService` (all in
  `ServiceStudio.Plugin.RESTService.Enumerations`, full values in
  [`reference/rest-service-api.md`](reference/rest-service-api.md)):
  `Authentication` (start `None` for a dev/demo API, revisit before shipping),
  `HttpSecurity = SSL`, `ShowDocumentation = Yes`,
  `CrossSiteRequests = Restrict`, `DefaultValueBehavior = Send`,
  `InternalAccess = No`, `Interoperability = No`.

Full ready-to-adapt lambda: [`examples/AddRestApiForEntity.cs`](examples/AddRestApiForEntity.cs) —
swap `ShippingCompany`/`ShippingCompanyAPI`/`ShippingCompanies` for your
entity/API/plural names, it's otherwise usable verbatim as the `code` argument
(`imports: []`).

## 3.1 Composite create for a master-detail entity pair

For a header/detail pair (e.g. `Order` + `OrderLine`, `Invoice` + `InvoiceLine`)
where the plain per-entity CRUD above isn't enough — the caller wants to submit
one order *and* its lines in a single POST — build one extra REST action on the
header entity's API instead of trying to compose two separate CRUD calls
client-side. Proven shape (`CreateOrder`, POST `/Orders`):

1. **Wrap the two logical inputs in a structure** (gotcha #6 above) — one
   `Body` input of a structure type with a record-typed field for the header
   and a list-typed field for the lines.
2. **Local variables** for the loop: an accumulator (`TotalAmount`, matches
   whatever rollup field the header has), the new header's identifier
   (`NewOrderId`, `DataType = headerEntity.IdentifierType`), and a scratch
   record (`NewLine`, `DataType = detailEntity`) to stamp the foreign key and
   any computed field onto before each `Create` call.
3. **Flow**: `Start → Assign(accumulator := 0) → ExecuteServerAction(header.CreateAction)
   → Assign(NewHeaderId := <execNode>.Id) → ForEach(over the list field) → ...cycle body...
   → Assign(stamp the rollup + Id back onto the header field) →
   ExecuteServerAction(header.UpdateAction) → Assign(output := NewHeaderId) → End`.
4. **Cycle body** (the ForEach's `CycleTarget`, chained with `ConnectedBelow`,
   looping back via the last node's `.Target = forEachNode`):
   `Assign(NewLine := <list>.Current; NewLine.<FK> := NewHeaderId; NewLine.<computed> := <expr on Current>)
   → ExecuteServerAction(detail.CreateAction, Source = NewLine)
   → Assign(accumulator += NewLine.<computed>)`.
   This is the same shape as `servicestudio-mcp-oml`'s `AddServerActionWithForEach.cs`
   (`forEachNode.CycleTarget = firstBodyNode`, last body node's `.Target = forEachNode`)
   — it isn't REST-specific, just applied inside a REST action's flow instead
   of a plain server action's.
5. **Nested field assignment on a record/structure works fine** —
   `assign.CreateAssignment("NewLine.OrderId", "NewOrderId")` and
   `assign.CreateAssignment("Request.Order.TotalAmount", "TotalAmount")` both
   validated cleanly. Don't be afraid of assigning into a sub-field the way you
   would in Service Studio's UI.

Full worked lambda: [`examples/AddCompositeCreateEndpoint.cs`](examples/AddCompositeCreateEndpoint.cs)
(the `CreateOrder` endpoint — apply it *after* the base CRUD API from § 3 exists,
since it adds one action onto an already-created `IRestService`).

## 4. Worked examples

- [`examples/AddRestApiForEntity.cs`](examples/AddRestApiForEntity.cs) — the exact
  pattern that produced a clean save (no `type: "error"` validation messages) on
  the second attempt, after fixing the Start/End and leading-slash issues from
  § 1 on the first. Builds all five plain-CRUD endpoints for one entity in a
  single `applyModelApiCode` call, creating the `IRestService` itself.
- [`examples/AddCompositeCreateEndpoint.cs`](examples/AddCompositeCreateEndpoint.cs) —
  the § 3.1 pattern: one composite "create header + lines" REST action added
  onto an *existing* `IRestService` (looked up via
  `eSpace.Integrations.OfType<IRestService>().Named(...)` rather than created
  fresh). First attempt hit gotcha #6 (two `Body` inputs); second attempt, with
  the request wrapped in a structure, saved clean.

## 5. Recovery

If a call comes back with `validationMessages` containing a `type: "error"`
entry (saved-but-invalid — see `servicestudio-mcp-oml`'s lambda-contract
reference for the general outcome table), don't try to patch forward from the
invalid state. Call `omlReset` (just `eSpaceName`) to discard the chain back to
the last good merged module, fix the code, and re-run `applyModelApiCode`. This
is the exact path that resolved both the Start/End trap and the URLPath trap in
one round-trip each — cheaper than chasing a saved-but-invalid model forward.

Once a run comes back clean (no `type: "error"` entries), finish with `omlMerge`
(`eSpaceName` + the `mutatedOmlPath` from that clean run) as usual.
