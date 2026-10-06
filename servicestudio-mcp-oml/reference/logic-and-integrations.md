# Logic and integrations beyond basic flows — verified recipes

**Status:** every recipe below was run live through `applyModelApiCode` against a
Reactive O11 module on 2026-09-30 and validated with TrueChange. Several facts were
first reported by the internal `claude-oml-tool` project (a headless builder) and are
kept here only where they reproduced in-process.
**Companions:** [`patterns-by-element.md`](patterns-by-element.md) (entities, actions,
silent renames, screen access), [`model-api-tips.md`](model-api-tips.md) (flow layout).

## Contents

- [SQL (advanced query) nodes](#sql-advanced-query-nodes)
- [For Each](#for-each)
- [Exception handlers](#exception-handlers)
- [Exposed REST APIs](#exposed-rest-apis)
- [Consuming REST APIs](#consuming-rest-apis)
- [Sending email](#sending-email)
- [Timers](#timers)
- [Site properties](#site-properties)
- [Locales and translations](#locales-and-translations)
- [BPT processes (O11)](#bpt-processes-o11)
- [What TrueChange catches (and what it doesn't)](#what-truechange-catches-and-what-it-doesnt)

## SQL (advanced query) nodes

Fully typed — no reflection. Example: [`../examples/AddServerActionWithAdvancedQuery.cs`](../examples/AddServerActionWithAdvancedQuery.cs).

```csharp
var sql = act.CreateNode<OutSystems.Model.Logic.Nodes.ISQLNode>("GetBigOrders").ConnectedBelow(start);
var p = sql.CreateInputParameter("MinAmount");  p.DataType = eSpace.DecimalType;
sql.SetArgumentValue(p, "MinAmount");                 // the argument slot exists — set it, don't add one
sql.CreateOutput(order);                              // entity or structure; one per output record
sql.Statement = "SELECT {Order}.* FROM {Order} WHERE {Order}.[Amount] >= @MinAmount";
sql.SetMaxRecords(100);                               // optional; an expression, not an int property
```

- Output rows are addressed as `<Node>.List.Current.<Output>.<Attr>` (e.g. `GetBigOrders.List.Current.Order.Amount`).
- `ISQLInputParameter.ExpandInline` exists for inline (unescaped) parameters — avoid it unless you
  sanitise with `EncodeSql(...)`; it is an SQL-injection vector.
- O11 targets SQL Server/Oracle syntax, not PostgreSQL — don't port ODC SQL snippets verbatim.
- Prefer an aggregate for simple reads; use SQL for things aggregates can't express.

## For Each

```csharp
var loop = act.CreateNode<OutSystems.Model.Logic.Nodes.IForEachNode>().ConnectedBelow(prev);
loop.SetRecordList("GetBigOrders.List");
var body = act.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>().ToTheRightOf(loop);
body.CreateAssignment("Total", "Total + GetBigOrders.List.Current.Order.Amount");
loop.CycleTarget = body;        // first node of the body
body.Target = loop;             // last node of the body loops back
loop.Target = end;              // exit path
```

- Address the current row through the list: `<Source>.List.Current…`.
- Optional: `SetStartIndex(...)`, `SetMaximumIterations(...)`.
- In the layout pass, the body goes one column right on the For Each's row ([`model-api-tips.md`](model-api-tips.md) § "The validated BFS layout pass").

## Exception handlers

```csharp
var h = act.CreateNode<OutSystems.Model.Logic.Nodes.IExceptionHandlerNode>("AllExceptions");
h.Exception = eSpace.AllExceptions;       // or a user / role exception
h.AbortTransaction = false;               // default true: the caller's transaction is rolled back
h.LogError = true;                        // default
var fix = act.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>().ConnectedBelow(h);
act.CreateNode<OutSystems.Model.Logic.Nodes.IEndNode>().ConnectedBelow(fix);   // its OWN End
```

- **The handler's path must end on its own End node.** Pointing it at the main End is a TrueChange
  error: `InvalidFlow_BadErrHandlerPath` ("Flow path of exception handler … can't cross main path")
  plus `InvalidFlow_MalformedAction` ("Ambiguous paths to End").
- Read the message as `<HandlerName>.ExceptionMessage`.
- `AbortTransaction = true` (the default) rolls back **everything** the request wrote so far, even
  though the flow continues — set it to `false` when the handler is meant to recover.
- Handlers aren't reachable from Start, so the layout pass has to place them separately.

## Exposed REST APIs

Typed through the REST-service plugin interfaces (write them fully qualified; `ServiceStudio.Plugin.*`
can't go in `imports`). Example: [`../examples/AddExposedRestApi.cs`](../examples/AddExposedRestApi.cs).

```csharp
var api = eSpace.CreateIntegration<ServiceStudio.Plugin.RESTService.IRestService>("OrdersAPI");
var m = api.CreateAction("GetOrderById");
m.Nodes.ToList().ForEach(x => x.Delete());   // REST methods are created WITH Start + End
m.HTTPMethod = ServiceStudio.Plugin.RESTService.Enumerations.HTTPMethod.GET;   // GET | PUT | POST | DELETE | PATCH
var id = m.CreateInputParameter("Id");       // ReceiveIn defaults to URL (also Header, Body)
id.DataType = eSpace.LongIntegerType;
m.CreateOutputParameter("Order").DataType = order;
// … build the flow from a new Start …
```

- **A new REST method already has a Start and an End.** Adding another Start fails TrueChange with
  `InvalidFlow_TooManyNodes: More than one Start found`. Delete the defaults, or build from the existing Start.
- Service defaults (read back): `BaseURL = "/rest/<Api>"`, `Authentication = None`,
  `HttpSecurity = SSL`, `InternalAccess = No`, `ShowDocumentation = Yes`, `CrossSiteRequests = Allow`.
  **Set `Authentication` deliberately** — the default exposes the API unauthenticated.
- Callbacks: `api.CreateOnRequestCallback()` / `CreateOnResponseCallback()`; with Basic/Custom
  authentication an `OnAuthentication` flow is available.
- Take identifiers as Long Integer inputs and convert with `LongIntegerToIdentifier(...)`.
- **URL path placeholders** (`/PurchaseOrders/{OrderRef}`) need the matching input
  parameter to be **mandatory**, otherwise TrueChange reports an error. A simple fix is
  to make the parameter an optional **query** parameter instead (verified 2026-10-01).

## Consuming REST APIs

See [`../examples/AddRestClient.cs`](../examples/AddRestClient.cs) (`CreateIntegration<IRestClient>`).
Verified 2026-10-01:
- The base URL property is `BaseURL`. For a base URL that varies by environment, set it in an
  `OnBeforeRequest` callback from a site property (no hard-coded endpoints).
- Structure-typed request/response parameters need **DTO structures owned by the
  client**. Passing structures from elsewhere in the module fails validation, so create
  dedicated structures and map them.

## Sending email

Example: [`../examples/AddServerActionSendEmail.cs`](../examples/AddServerActionSendEmail.cs).

- Create the email on a UI flow: `flow.CreateEmail("OrderShipped")`, `SetSubject("\"…\"")`, inputs, body widgets.
- Send it from a **server action** with the Reactive node
  `OutSystems.Model.Logic.Mobile.Nodes.ISendEmailNode` (`Email = …`, `SetTo`, `SetFrom`, `SetCc`, `SetBcc`,
  `SetArgumentValue(emailInput, "expr")`). The `OutSystems.Model.Logic.Nodes.ISendEmailNode` is the
  legacy Traditional node.
- TrueChange warns `FlowThemeTooBigForEmails` when the email's flow uses a theme over 14 KB — keep emails
  in a dedicated flow with a light theme.
- If the send can fail, put it in its own action with an exception handler (`AbortTransaction = false`)
  so a mail error doesn't roll back the business transaction.

## Timers

Example: [`../examples/AddTimer.cs`](../examples/AddTimer.cs).

- `eSpace.CreateTimer("X")`, `.Action = serverAction`, `.Schedule` is a **string**:
  `"WhenPublished"` runs on each publish; an **empty schedule means "not scheduled"** (only runs when woken).
  Strings such as `"Every 5 minutes"` are stored as written — check the schedule in Service Studio's
  editor rather than trusting the text.
- `TimeoutInMinutes` defaults to **20**.
- Cron-style **step syntax is rejected**: `*/15 * * * *` fails. Write the minutes out
  in full: `0,15,30,45 * * * *` (verified 2026-10-01).
- If the action has mandatory inputs the timer must set them — TrueChange error
  `ArgumentRequiredExpression: A valid expression must be set for parameter '…'`. Prefer
  input-less timer actions.

## Site properties

- `eSpace.CreateSiteProperty(isReadOnly: false, name: "ApiKey")`, `.DataType = eSpace.TextType`,
  `.IsSecret = true` for credentials, `SetDefaultValue(...)` for the default.
- A site property with no default is **not** flagged by TrueChange — readers get the type's zero value.
  Always set a meaningful default (except for secrets, which are set per environment).

## Locales and translations

- `eSpace.GetOrCreateLocale(OutSystems.Model.Enumerations.Culture.PT_PT)` creates a locale (verified).
- `eSpace.ApplyToTextResources((obj, name, type, defaultValue) => …)` walks every translatable text
  resource (178 on a near-empty Reactive module, starting with the built-in validation messages) —
  read-only.
- **There is no typed API to write a translated value**, and `LoadTextResources(path)` needs file
  access, which the sandbox forbids. Translation work stays in Service Studio.

## BPT processes (O11)

O11 Business Process Technology is authored through `applyModelApiCode` with an API that the generated
[`../docs/`](../docs/) **don't list** (and can't be regenerated, because the generator relies on reflection,
which the sandbox now blocks). Everything below was mapped with compile probes and verified live on
2026-09-30: a Launch-on-Create process with a human activity, an automatic activity and a Yes/No decision
saved with **zero TrueChange errors**. Full example:
[`../examples/AddProcessWithHumanActivity.cs`](../examples/AddProcessWithHumanActivity.cs).

> **Not `eSpace.CreateBusinessProcess`.** That is the ODC Workflows API (`OutSystems.Model.BusinessProcesses.*`)
> and fails on an O11 module with `Process objects can't be children of Module objects`.

### The process

```csharp
var p = eSpace.CreateProcess("ApproveOrder");            // OutSystems.Model.Processes.IProcess; eSpace.Processes lists them
p.LaunchOn.Entity = order;                                // IProcessTrigger
p.LaunchOn.Action = OutSystems.Model.Processes.EntityActionKind.Create;   // None | Create | Update
p.SetDetail("\"Order \" + OrderId");                   // Process Detail (Taskbox); missing = warning ProcessMissingInboxDetail
```

- Members that exist: `Name`, `Label`, `Description`, `Folder`, `Icon`, `Detail` / `SetDetail(expr)`,
  `InputParameters` / `CreateInputParameter(name)`, `LaunchOn`, `OnActivityClose` (an `ICallback` flow),
  `Nodes`, `CreateNode<T>()`.
- Setting a Create/Update trigger **auto-creates the `<Entity>Id` process input** (`OrderId`, mandatory) and
  names the Start node after it ("New Order").
- The model generates **`Launch<Process>`** (output `ProcessId`) on save, and one **`Close<Activity>`**
  action per human activity (input `ActivityId`, output `NextHumanActivityId`). Call them from **server**
  actions. Per claude-oml-tool (runtime behaviour, not re-verified here): *Launch On* **Update** never starts
  an instance — for "on update" call `Launch<Process>` from the updating server action; `Close<Activity>`
  checks no role, so gate the server action that calls it.

### Nodes and wiring

Node types live in **`OutSystems.Model.Processes.Nodes`** (write them fully qualified):
`IStartNode`, `IEndNode`, `IHumanActivityNode`, `IAutomaticActivityNode`, `IDecisionNode`, `IWaitNode`,
`IConditionalStartNode`, `IExecuteProcessNode`, `ICommentNode`, `ISendEmailNode` (base `IProcessNode`).

- **Wire with `node.Targets.Add(next)`** (`ICollection<IProcessNode>`). The `Connected*` helpers **throw**
  (`Node doesn't have a target property`); position with `Below` / `ToTheRightOf` instead. Missing wiring is
  a TrueChange error (`RequiredConnector_In` / `RequiredConnector_Out`).
- Outgoing connectors: `node.Connectors` (`IProcessConnector`: `Source`, `Target`).

| Node | Verified members | Notes |
|---|---|---|
| `IHumanActivityNode` | `Name`, `Label`, `Description`, `Destination` (`IScreenSignature`), `Roles`, `Instructions` (string), `SetUser(expr)` / `User`, `SetDueDate(expr)` / `DueDate`, `Arguments`, `OnReady` / `OnClose` + `CreateOnReady()` / `CreateOnClose()`, `CreateOutputParameter`, `OutputParameters`, `CloseOn`, `Targets` | **Roles default to `Registered`** — add the real role and `Roles.Remove(eSpace.RegisteredRole)`. **Screen inputs:** setting `Destination` creates one argument per screen input; set it with `ha.Arguments.First(a => a.Parameter.Name == "OrderId").SetValue("OrderId")`. (`SetScreenArgumentValue` does **not** work here — it only accepts the ODC `BusinessProcesses` type, `CS1929`.) **`OnReady` / `OnClose` are `null` until created**; `CreateOnReady()` returns an empty flow — add Start and End or TrueChange reports `InvalidFlow_NoStart` |
| `IAutomaticActivityNode` | `Nodes`, `CreateNode<Logic.Nodes.*>()`, `LocalVariables`, `CreateLocalVariable`, `OutputParameters`, `Targets` | **It is a logic flow and ships with its own Start** — reuse it (adding one = `InvalidFlow_TooManyNodes`) |
| `IDecisionNode` | `CreateConnector(name, target)`, `Connectors`, `Nodes` | Creating named connectors **auto-builds the decision flow** (`Start → If → one Outcome per connector`); set the `If` condition (`dec.Nodes.OfType<Logic.Nodes.IIfNode>().Single().SetCondition(…)`). No `Targets` |
| `IWaitNode` | `SetTimeout(expr)`, `Timeout`, `CloseOn`, `Targets` | |
| `IConditionalStartNode` | `StartOn`, `Targets` | |
| `IExecuteProcessNode` | `Process`, `Arguments`, `Targets` | |
| `IStartNode` | `Targets`, `Connectors` | no `Name` member — it is named from the trigger |

- `ICallback` (human-activity `OnReady` / `OnClose`, process `OnActivityClose`) is an action flow:
  `Nodes`, `CreateNode<Logic.Nodes.*>()`, `LocalVariables`, `CreateLocalVariable`. It doesn't exist
  until created (`ha.CreateOnReady()`), and it is created empty — unlike an automatic activity, which
  ships with its own Start.
- Inside process flows the **process inputs are in scope** (e.g. `OrderId` in an automatic activity's
  `SetArgumentValue` or a decision condition).

### Reading processes

- `getSerializedObjectByNameAndType` with `objectType: "OutSystems.Model.Processes.IProcess"` returns the
  process as JSON (nodes, activity flows, decision connectors, generated actions) — the best read tool.
- `runQuery`: `Root { Processes { Name Nodes { … } } }` works; not every node type has `Name`, so select
  fields carefully.
- There is no dedicated `getProcess` code-returning tool.

### Taskbox recipe (from claude-oml-tool, not re-verified in-process)

"My tasks" = `(System).Activity` filtered by `Activity.User_Id = GetUserId() and Activity.Closed = NullDate()`;
role-queued tasks have `User_Id = NullIdentifier()`. Give the task screen an optional `ActivityId` input and
reference `(System)` `Activity` through `eSpace.References.Named("(System)")`.

## What TrueChange catches (and what it doesn't)

In-process TrueChange (the `validationMessages` array and `getValidationMessages`) is the primary
check, and it covers far more than a headless tool gets. Observed 2026-09-30:

| Problem | TrueChange | id |
|---|---|---|
| Action with no Start | Error | `InvalidFlow_NoStart` |
| Two Starts (e.g. REST method defaults + your own) | Error | `InvalidFlow_TooManyNodes` |
| Exception handler path reaches the main End | Error | `InvalidFlow_BadErrHandlerPath`, `InvalidFlow_MalformedAction` |
| Input with no Variable | Error | `RequiredPropertyValue_Property` ("Variable must be set") |
| Input bound to an Identifier / non-basic type | Error | `ExpressionInvalidVariableType` |
| Input type doesn't match the variable (Number on Text) | Error | `CustomObjectError` |
| Button / Link with no On Click | Error | `RequiredPropertyValue_Property` |
| Table Records with no Source | Error | `RequiredPropertyValue_Property` |
| Popup with no Show Popup | Error | `RequiredPropertyValue_Property` |
| Second Binary Data attribute | Error | `InvalidEntity_OneBin` |
| Timer action input not set | Error | `ArgumentRequiredExpression` |
| Process node without incoming / outgoing connector | Error | `RequiredConnector_In` / `RequiredConnector_Out` |
| Process without Detail | Warning | `ProcessMissingInboxDetail` |
| Aggregate with two sources and no join | **Warning only** | `CrossJoinDataSet` |
| Screen only for authenticated users but module has no roles | Warning | `AnonymousAccess_NoPermissionsNoRoles` |
| Email flow theme > 14 KB | Warning | `FlowThemeTooBigForEmails` |
| Silently renamed element (`GetOrder2`, truncated attribute) | **not flagged** — read `.Name` back | — |
| Parameter named like an entity | not flagged | — |
| Site property without default | not flagged | — |
| Nodes stacked on one coordinate | not flagged — run the layout pass | — |
| Exposed REST API left with `Authentication = None` | not flagged | — |

Treat the **warnings** above as defects too: a cross join silently multiplies rows, and an agent that only
filters on `Error` will miss it.

### Problems that pass TrueChange but fail at publish or merge (verified 2026-10-01)

- **Multiple cascade paths.** O11 implements the `Delete` delete rule as
  `ON DELETE CASCADE`. Two FK chains with `Delete` that reach the same table cause
  publish to fail with an "Upgrade Error" during the database upgrade. Example:
  `Part → PartInventory → ReorderAlert` plus a direct `Part → ReorderAlert`.
  TrueChange shows 0 errors. Fix: set one of the paths to `Ignore`.
- **`Record List To Excel` output** can't be referenced (SKILL.md § 8). Export CSV instead.
- **`Theme.IconLibrary`** is accepted by the sidecar, but older Service Studio builds
  reject the merge (SKILL.md § 3.1). Don't set it. Theme stylesheet edits merge and
  publish fine without it; the sidecar's `InvalidIconLibrary` error can be ignored.
- **Expression-type traps that TrueChange does flag, but are easy to hit:**
  - Comparing `Date` with `DateTime`: wrap with `DateTimeToDate(...)`.
  - Assigning `LongInteger` to `Integer`: use `LongIntegerToInteger(...)`.
  - `FormatPercent` takes a different number of arguments than `FormatDecimal`; use
    `FormatDecimal(x, 1, ".", "") + "%"`.
