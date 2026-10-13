---
name: servicestudio-mcp-rest-api
description: >-
  Expedites exposing a full REST API (an Exposed REST API integration, O11's
  "Integrations > REST" feature) over an existing O11 server entity via the
  Service Studio MCP `applyModelApiCode` tool — List/Get/Create/Update/Delete
  wired straight to the entity's own auto-generated CRUD actions, plus
  composite "create a header with detail lines in one call" endpoints for
  master-detail entities (ForEach + accumulator pattern). Captures gotchas
  discovered the hard way (the `CreateIntegration<IRestService>` entry point,
  the "More than one Start" / "End must have at least one incoming
  connector" node trap, the leading-slash `URLPath` requirement, the disallowed
  `imports` namespace, the one-Body-input-per-method limit, the "don't splice a
  node into an already-merged flow" trap, and the real parameter names of an
  entity's Get/Create/Update/Delete actions) so a REST API gets built in one or
  two clean applies instead of three or four. Verified end to end against a
  published module (HTTP calls, composite create, identifier URL parameters on
  the wire, error status codes). Use
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
  origin: distilled from a live session building ShippingCompanyAPI over a ShippingCompany entity in module "aademo" (2026-09-11) — three applyModelApiCode attempts before it saved cleanly; extended the same session building OrderAPI's composite CreateOrder endpoint (Order + OrderLine header/detail) — one more gotcha (single Body input per method) found and folded in. Re-verified live 2026-10-13 on Service Studio 11.55.92 against module "aademo": the CRUD recipe and the composite create were published and exercised over HTTP; findings in section 6.
---

# Expose an O11 entity as a REST API

This is a companion to **`servicestudio-mcp-oml`** — read that skill first for the
full-lambda contract (`eSpace => { ... }`, no `eSpace.Save`), the tool catalogue
(`applyModelApiCode`, `getDataModel`, `runQuery`, `omlReset`, `omlMerge`), and the
session-pointer/silent-no-op traps. Everything below assumes you already know that
contract. `servicestudio-mcp-oml` covers the basics of **Exposed REST APIs**
(`ServiceStudio.Plugin.RESTService`) in
[`reference/logic-and-integrations.md`](../servicestudio-mcp-oml/reference/logic-and-integrations.md)
("Exposed REST APIs") and `examples/AddExposedRestApi.cs` (one GET method).
This skill goes further: a full CRUD + List recipe, composite master-detail
create, and the gotchas you only hit once you build several methods. Items
marked *(also in OML)* repeat OML on purpose so this skill stands alone.

## Contents

- [§ 1 — The things that will bite you](#1-the-things-that-will-bite-you)
- [§ 2 — Before you write code: confirm the entity's action signatures](#2-before-you-write-code-confirm-the-entitys-action-signatures)
- [§ 3 — The recipe](#3-the-recipe)
- [§ 3.1 — Composite create for a master-detail entity pair](#31-composite-create-for-a-master-detail-entity-pair)
- [§ 4 — Worked examples](#4-worked-examples)
- [§ 5 — Recovery](#5-recovery)
- [§ 6 — Verified live behaviour](#6-verified-live-behaviour)
- [§ 7 — Advice before you ship](#7-advice-before-you-ship)

## 1. The things that will bite you

Each of these cost a failed `applyModelApiCode` call the first time. Read them
before you write code, not after the error comes back.

1. **The entry point is the generic integration factory** *(also in OML)*. There is no
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

2. **You cannot import that namespace** *(also in OML)*. `applyModelApiCode`'s `imports` array
   only accepts `OutSystems.Model` or its sub-namespaces —
   `ServiceStudio.Plugin.RESTService` is rejected outright:
   > `Import 'ServiceStudio.Plugin.RESTService' is not allowed. Only
   > 'OutSystems.Model' (or one of its sub-namespaces) may be imported.`
   Pass `imports: []` and fully-qualify every `ServiceStudio.Plugin.RESTService.*`
   type inline in the code string instead. This is more verbose but it's the
   only path that compiles.

3. **`api.CreateAction("Name")` already has a Start node and an End node** *(also in OML, which deletes the defaults; this skill reuses them)*.
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
   expect** *(output name also noted in OML)*. `Get<Entity>`'s output parameter is named **`Record`**, not the
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
  `Authentication` (the default `None` exposes the API unauthenticated and
  TrueChange does not flag it — see § 7 before publishing anywhere shared),
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
- [`examples/AddRestApiWithNotFound.cs`](examples/AddRestApiWithNotFound.cs) — the
  same CRUD API with a real `404` on missing ids for Get / Update / Delete via
  `HTTPRequestHandler.SetStatusCode`. Requires that Forge component as a
  reference (§ 7.2).
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

## 6. Verified live behaviour

Everything below was observed on 2026-10-13 against Service Studio 11.55.92:
the § 3 CRUD recipe (adapted to a `Product` entity) and the § 3.1 composite
create (against a throwaway `TestOrder`/`TestOrderLine` pair) were applied,
merged (`omlMerge`), published (`omlPublish`) and called with `curl`.

**Model level**

- Both recipes saved clean on the **first** apply — no `type: "error"`
  entries. The only messages were unrelated `AnonymousAccess_NoPermissionsNoRoles`
  warnings already in the module.
- `{Id}` URL inputs default to `IsMandatory = true`. You do **not** need to set
  it; OML's note that a placeholder input must be mandatory only bites if
  someone sets it to false.
- Use `entity.IdentifierType` for `{Id}` inputs (as the examples do). A Long
  Integer input converted with `LongIntegerToIdentifier(Id)` validates and
  behaves **identically** on the wire (below), so the conversion is optional.
- A Get action's `Record` output is an **anonymous structure** that contains an
  attribute named after the entity. Both `GetEntity.Record` (what the examples
  assign) and `GetEntity.Record.<Entity>` validate and assign to an
  entity-typed output.
- The validator **does** check assignment expressions and types, so a typo is
  caught at apply time, not publish time: unknown path →
  `ExpressionUnknownObject: Can't identify 'X' element in expression`; wrong
  type → `ExpressionInvalidExpressionType: 'Product' data type required instead
  of 'Text'`. Read `validationMessages` after every apply.
- Re-confirmed on the live model: `URLPath` without a leading `/` →
  `'URL Path' property must start with '/'`; two `Body` inputs →
  `Method '<Name>' can have only one input parameter with 'Receive In'
  property set to 'Body'` (reported on **each** Body input, so you see it
  twice).

**Wire level** (`https://<host>/<module>/rest/<Api>/<URLPath>`)

| Call | Result |
|---|---|
| `GET /Products` | `200`, bare JSON array of records |
| `GET /Products/1` | `200`, record object; identical for Identifier-typed and Long Integer-typed `{Id}` |
| `GET /Products/abc` | `400` `{"Errors":["Failed to parse. 'LongInteger' compatible data type is required for parameter 'Id'."],"StatusCode":400}` — same for both id types |
| `GET /Products/999999` (no such row, plain recipe) | **`500`** `{"Errors":["Error executing query."],"StatusCode":500}` — use the § 7.2 guard for a real `404` |
| `GET` / `PUT` / `DELETE /Products/999999` with the § 7.2 guard | **`404`** (Get body: default record; Put/Delete: empty) |
| `POST /TestOrders` (composite) | `200`, body is the bare id, e.g. `1` |
| `GET <base URL>` | `301` to the generated documentation page when `ShowDocumentation = Yes` |

- A single `SendIn = Body` output is sent **unwrapped**: a list output is a
  bare JSON array, an identifier output is a bare value (`1`), a record output
  is the record object. There is no `{"Id": 1}` envelope.
- Request JSON field names are the structure attribute names. The composite
  endpoint accepted
  `{"TestOrder":{...},"TestOrderLines":[{...},{...}]}` (the names come from the
  `CreateTestOrderRequest` structure's attributes).
- **Composite create verified end to end**: one POST with two lines
  (`2 × 10` and `3 × 5`) produced a header with `TotalAmount = 35`, two lines
  with `TestOrderId` stamped to the new header and `LineTotal` computed, so the
  ForEach, FK stamp, accumulator and Update-back-onto-header all work as
  written.
- Merge and publish path: `omlMerge` (`mutatedOmlPath` from the last clean
  apply) then `omlPublish` — both returned success without further prompting.
  Allow for publish to the shared dev environment.

**MCP session notes for this build**

- Service Studio 11.55.92 requires `createSessionToken` first and the token on
  every call (`sessionToken` at top level, everything else under `input`).
  `runQuery`'s `query` goes **inside** `input` next to `eSpaceName`.
- `IsPublic` / `IsIdentifier` are not queryable `runQuery` fields; use the
  § 2 query shape, or `getDataModel` and read `Public = true` / `IdentifierAttribute`.

## 7. Advice before you ship

1. **Set `Authentication` deliberately.** The default is `None`. The recipes
   leave it at `None` so the first publish works, and on the live test an
   unauthenticated `POST` created data. TrueChange does not flag it. Switch to
   `Basic` or `Custom` (and implement the `OnAuthentication` callback) before
   the API is published anywhere other people can reach.
2. **A real HTTP 404 needs the HTTPRequestHandler component.** `Get<Entity>`
   raises when the row does not exist, so the plain recipe answers `500` with
   `Error executing query.`. The Model API itself cannot fix that: `(System)`
   has no status-setting action, a REST output's `SendIn` is only `Body` or
   `Header`, and the built-in exceptions (`AllExceptions`, `Database`,
   `Security`, `NotRegistered`, `Communication`) include nothing for "not
   found". What works (verified 2026-10-13): add the Forge component
   **HTTPRequestHandler** as a reference of the module (a Service Studio step
   — Manage Dependencies), then in each id-based method look the row up with
   an aggregate (`FindX`, filter `X.Id = Id`), branch on `FindX.List.Empty`,
   and on the true branch call `HTTPRequestHandler.SetStatusCode` with
   `StatusCode = 404` and end on its **own** End node. Apply the same guard
   to Update and Delete — without it a missing id is also a 500. Full lambda:
   [`examples/AddRestApiWithNotFound.cs`](examples/AddRestApiWithNotFound.cs).
   Details observed: `SetStatusCode` takes one input, `StatusCode`; the 404
   body is still the output record with default values (`{"Id":0,"Name":""…}`),
   Update/Delete 404s have an empty body; a non-numeric id is still `400`.
3. **Don't expose the entity record as the contract unless you mean to.** The
   examples return and accept the entity as-is, so every attribute (including
   internal ones) is part of the public API. For anything beyond a demo,
   expose a structure with just the fields callers need and map in the flow.
4. **Rehearse without merging.** `applyModelApiCode` only changes the
   in-memory chain; read it back (`getIntegrations`, `getValidationMessages`)
   and finish with `omlReset` to leave the module untouched. Only `omlMerge`
   and `omlPublish` touch the real module and the server.
5. **Check the node flow after the apply.** A clean `validationMessages` array
   does not prove the flow shape. Read it back with `getIntegrations` and
   confirm each action has exactly one Start and one End and the verb / path
   you expected (this is how the § 1 trap was caught the first time).
6. **Test on the wire, then clean up.** Publishing a test entity creates a real
   table and `curl` calls create real rows. Name throwaway objects obviously
   (`Test…`) so they can be deleted, and remember deleting the entity drops
   the table on the next publish.
7. **Order matters for the composite endpoint.** It looks the API up by name,
   so create the base `IRestService` first (or `CreateIntegration` it in the
   same lambda). The header and detail entities must be `Public` and have
   their identifier set before their `CreateAction` / `UpdateAction` exist.
