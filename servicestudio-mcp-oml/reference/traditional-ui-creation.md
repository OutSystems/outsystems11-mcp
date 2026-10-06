# Traditional Web — creating UI (screens, blocks, widgets, data fetch)

Companion to [`traditional-patterns.md`](traditional-patterns.md) (which covers *reading* and
*mutating existing* Traditional Web UI) and to [`../SKILL.md`](../SKILL.md) § 4.1. This file is
the **recipe book for *creating* Traditional Web UI** through `applyModelApiCode`: web
screens, web blocks, widget trees, the Preparation + Aggregate data-fetch flow, ListRecords
binding, and the client-side `JavaScript` hook.

Interface signatures for every type used here live in
[`../docs/OutSystems.Model.UI.Web.Generated.cs`](../docs/OutSystems.Model.UI.Web.Generated.cs)
and
[`../docs/OutSystems.Model.UI.Web.Widgets.Generated.cs`](../docs/OutSystems.Model.UI.Web.Widgets.Generated.cs).
For a full worked screen, see
[`../examples/AddWebScreenWithListRecords.cs`](../examples/AddWebScreenWithListRecords.cs).

> **Re-verified 2026-10-03** on a live Traditional module, under the sandboxed contract (throw-probe,
> nothing saved): `CreateScreen`, `SetTitle`, container / text / expression / link widgets,
> `CreatePreparation` + aggregate, `IListRecordsWidget.SetSourceRecordList` / `SetEmptyMessage`,
> `OnClick.Destination` and `IsFrequentDestination` (which clears the `DisconnectedDestination`
> warning) all work as written. An unqualified `CreateWidget<ITextWidget>()` bound to the
> Reactive/Mobile type and failed with **CS0311**, so fully qualify Web widget types.

> **Applies to Traditional only.** Call `listApps` first; these recipes apply when `moduleType` is
> `"Traditional"` or `"TraditionalLibrary"`. For Reactive / Mobile, use
> [`patterns-by-element.md`](patterns-by-element.md) (`eSpace.MobileFlows`, `IMobileScreen`,
> `screen.CreateScreenAggregate(...)`, `ServiceStudio.Plugin.NRWidgets.*`).

> **All snippets below are the statements that go inside the lambda.** Wrap them in
> `eSpace => { ... }` when you send them as the `code` arg, with no `eSpace.Save(...)` (the host
> appends Save for you). See [`lambda-contract.md`](lambda-contract.md).

> **Security (SKILL.md § 9):** `applyModelApiCode` runs with the host's
> privileges. Use these recipes only against modules you own.

## Read this first — the two compile errors that block Traditional widget creation

### CS0104 — `ITextWidget` is ambiguous (Mobile vs Web). Always fully-qualify widget types. {#widget-namespace-collisions}

Traditional widgets live in `OutSystems.Model.UI.Web.Widgets`, but the host's default imports
(SKILL.md § 4) also bring `OutSystems.Model.UI.Mobile` into scope, and **several widget names
exist in *both* namespaces**: `ITextWidget`, `IContent`, `IIfWidget`, `IIfBranchWidget`,
`IPlaceholderWidget`, `IPlaceholderContentWidget`. A bare `ITextWidget` therefore fails:

```
CS0104: 'ITextWidget' is ambiguous between
'OutSystems.Model.UI.Mobile.Widgets.ITextWidget' and 'OutSystems.Model.UI.Web.Widgets.ITextWidget'
```

**Prophylaxis — do both:**

1. **Always write the fully-qualified Web type** in `CreateWidget<T>`, e.g.
   `CreateWidget<OutSystems.Model.UI.Web.Widgets.ITextWidget>()`. The uniquely-named widgets
   (`IContainerWidget`, `IExpressionWidget`, `ILinkWidget`, `IListRecordsWidget`, …) are not
   ambiguous, but qualifying *all* of them is the safe, copy-pasteable habit.
2. **Do NOT add `using OutSystems.Model.UI.Mobile.Widgets;`** (or pass it in `imports`) inside a
   Web body — it makes the collision worse, not better.

This is the same class of dual-interface trap as `IExternalSite` (which exists in both Mobile and
Web) — see the existing note in
[`traditional-patterns.md`](traditional-patterns.md#externalsite--the-special-case-dual-interface-gotcha).
Logic-node types have a sibling clash (`IStartNode`/`IEndNode`); keep them fully-qualified too —
see [`model-api-tips.md`](model-api-tips.md).

### CS0311 — `CreateWidget<T>` needs `T : IWebWidget`, not the Reactive widget

`IWebScreen` / `IWebBlock` / `IContainerWidget` all expose
`CreateWidget<T>() where T : OutSystems.Model.UI.Web.Widgets.IWebWidget`. Passing the Reactive
container interface fails:

```
CS0311: 'ServiceStudio.Plugin.NRWidgets.IContainer' cannot be used as type parameter 'T' …
no implicit reference conversion to 'OutSystems.Model.UI.Web.Widgets.IWebWidget'
```

Use the Traditional widget interfaces from
[`../docs/OutSystems.Model.UI.Web.Widgets.Generated.cs`](../docs/OutSystems.Model.UI.Web.Widgets.Generated.cs).

### Don't emit Reactive-only constructs on Traditional

Traditional Web has **no** "On Scroll Ending" event, no OutSystems UI patterns, no client
actions, and no client-side JavaScript *logic node*. Emitting them does not error loudly — the
request **silently degrades**. Equivalents on Traditional:

| Reactive intent | Traditional equivalent |
|---|---|
| `List` + On Scroll Ending infinite scroll | `IListRecordsWidget`/`ITableRecordsWidget` + classic paging (RichWidgets `List_Navigation` block driven by the aggregate's `StartIndex`/`MaxRecords`) |
| JavaScript node in a client action | `IWebScreen.JavaScript` string (recipe 6), or an `IExpressionWidget` with `EscapeContent = false` |
| `screen.CreateScreenAggregate(...)` | screen/block **Preparation** flow with an aggregate node (recipe 4) |

## Recipe 1 — Create a Web screen

`eSpace.WebFlows.Named(...)` returns `IWebFlow`, whose `CreateScreen` returns `IWebScreen`
directly (no cast). `SetTitle` takes an **OutSystems expression string** — a literal title needs
inner quotes.

```csharp
var plansFlow = eSpace.WebFlows.Named("PlansFlow");
var screen = plansFlow.CreateScreen("PostFeed");
screen.SetTitle("\"News Feed\"");   // expression string: literal text is double-quoted
```

> If you obtained the flow as `IUIFlow` (e.g. via a broader query), `CreateScreen` returns the
> base `IScreen` — cast it: `var screen = (IWebScreen)flow.CreateScreen("PostFeed");`.

## Recipe 2 — Create a Web block

Same shape via `IWebFlow.CreateBlock` → `IWebBlock`. Unlike Reactive blocks, **Traditional blocks
support a Preparation** (recipe 4), so block-level data fetch works.

```csharp
var plansFlow = eSpace.WebFlows.Named("PlansFlow");
var block = plansFlow.CreateBlock("NotificationsPanel");
block.Description = "Dropdown panel listing unread notifications.";
```

## Recipe 3 — Build a widget tree (Container › Text / Expression / Link)

`CreateWidget<T>` is available on screens, blocks, and any container-like widget
(`IContainerWidget`, `ICellWidget`, `ILinkWidget`, `IListRecordsWidget`, `IContent`, …). Build
parent-first, then descend. **Note the fully-qualified widget types** (CS0104 prophylaxis above).

```csharp
var container = screen.CreateWidget<OutSystems.Model.UI.Web.Widgets.IContainerWidget>();

// Text: a plain settable string (NOT an expression)
var heading = container.CreateWidget<OutSystems.Model.UI.Web.Widgets.ITextWidget>();
heading.Text = "Latest posts";

// Expression: SetValue takes an expression string. EscapeContent defaults to true (HTML-safe).
var expr = container.CreateWidget<OutSystems.Model.UI.Web.Widgets.IExpressionWidget>();
expr.SetValue("GetPosts.List.Current.Post.Content");

// Link with a child label; OnClick is auto-created (non-null) and exposes Destination.
var link = container.CreateWidget<OutSystems.Model.UI.Web.Widgets.ILinkWidget>();
var label = link.CreateWidget<OutSystems.Model.UI.Web.Widgets.ITextWidget>();
label.Text = "Open dashboard";
```

## Recipe 4 — Preparation + Aggregate (the Traditional data-fetch flow)

This is the biggest structural difference from Mobile/Reactive. A Traditional screen or block
fetches data through a **Preparation flow that contains an Aggregate node** — *not*
`screen.CreateScreenAggregate(...)` (that is the Mobile API and does not exist on `IWebScreen`).
`IWebScreen.CreatePreparation()` / `IWebBlock.CreatePreparation()` returns
`OutSystems.Model.UI.Web.IPreparation`, whose `CreateNode<T>()` builds Start / Aggregate / End
nodes (keep logic-node types fully-qualified — see CS0104 note). Connect nodes with
`ConnectedBelow(...)`.

```csharp
var post = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Post");

var prep  = screen.CreatePreparation();
var start = prep.CreateNode<OutSystems.Model.Logic.Nodes.IStartNode>();
var getPosts = prep
    .CreateNode<OutSystems.Model.Logic.Nodes.IAggregateNode>("GetPosts")
    .ConnectedBelow(start);
getPosts.SetMaxRecords("50");                       // expression string
getPosts.AsDatabaseAggregate.CreateSource(post);    // FROM Post
getPosts.AsDatabaseAggregate.CreateFilter("Post.IsDeleted = False");   // optional WHERE
prep.CreateNode<OutSystems.Model.Logic.Nodes.IEndNode>().ConnectedBelow(getPosts);
```

The aggregate's output is referenced elsewhere on the screen as `GetPosts.List` (and per-row as
`GetPosts.List.Current.Post.<Attribute>`). **Bind the aggregate to a widget** (recipe 5) — an
aggregate that nothing consumes raises a benign `Unused Aggregate` validation warning.

## Recipe 5 — ListRecords binding

`IListRecordsWidget.SetSourceRecordList(...)` takes the aggregate's list as an **expression
string** (e.g. `"GetPosts.List"`), *not* a typed list reference. Add child widgets to the
ListRecords to define the row template; bind row expressions to `…List.Current.<Entity>.<Attr>`.

```csharp
var list = container.CreateWidget<OutSystems.Model.UI.Web.Widgets.IListRecordsWidget>();
list.SetSourceRecordList("GetPosts.List");          // <- aggregate output, as an expression
list.SetEmptyMessage("\"No posts yet.\"");          // expression string (literal is quoted)

var row = list.CreateWidget<OutSystems.Model.UI.Web.Widgets.IContainerWidget>();
var content = row.CreateWidget<OutSystems.Model.UI.Web.Widgets.IExpressionWidget>();
content.SetValue("GetPosts.List.Current.Post.Content");
```

`ITableRecordsWidget` follows the same `SetSourceRecordList` pattern for a tabular layout.

## Recipe 6 — Client-side JavaScript via `IWebScreen.JavaScript`

Traditional screens carry a `JavaScript` string property that is emitted verbatim into the
rendered page (there is no client-action JS node).

```csharp
screen.JavaScript = "console.log('PostFeed loaded');";
```

> ### ⚠ XSS caveat — the `JavaScript` hook is NOT auto-escaped
> Data interpolated into the `JavaScript` string is rendered verbatim and runs in the end-user's
> browser. Interpolating unescaped screen/widget/input data produces stored/DOM XSS **in the
> compiled app**. Escape user-controlled values with OutSystems expression escaping
> (`EncodeJavaScript(...)`) before embedding them. Note the contrast: `IExpressionWidget.SetValue`
> defaults to escaping (`EscapeContent = true`), but the `JavaScript` hook does **not** escape
> anything. (This mirrors the platform's own `HTML Injection` TrueChange warning on unescaped
> inline expressions.)

## The "Unexpected Link" warning — benign; set `IsFrequentDestination`

Pointing a link at a screen with `link.OnClick.Destination = someScreen` saves correctly but
raises a TrueChange **warning**:

```
(Warning) Unexpected Link … '<screen>' should be either a Frequent Destination
or the target of a connector from '<block>'.
```

The warning is **benign** (the link works). There is **no Model-API helper to register a flow
connector / "frequent destination" from the link side** — that surface is not API-discoverable
today. The honest, verified mitigation is to mark the **target** screen as a frequent
destination, which silences the warning:

```csharp
var dashboard = plansFlow.Nodes.OfType<IWebScreen>().First(s => s.Name == "Dashboard");
link.OnClick.Destination = dashboard;
dashboard.IsFrequentDestination = true;   // silences the 'Unexpected Link' warning for this target
```

(`IExternalSite.IsFrequentDestination` does the same for external-site link targets.) If you can't
set it on the target (e.g. a referenced screen you don't own), treat the warning as expected
noise.

## Cross-references

- [`traditional-patterns.md`](traditional-patterns.md) — reading + mutating *existing* Traditional
  Web UI; the `IExternalSite` CS0104 sibling gotcha; per-style verb selection.
- [`../docs/OutSystems.Model.UI.Web.Generated.cs`](../docs/OutSystems.Model.UI.Web.Generated.cs),
  [`../docs/OutSystems.Model.UI.Web.Widgets.Generated.cs`](../docs/OutSystems.Model.UI.Web.Widgets.Generated.cs)
  — interface/member signatures. (Widget event interfaces — `OnClick`, `OnChange` — live in
  `OutSystems.Model.UI.Web.Events`, not separately documented.)
- [`../examples/AddWebScreenWithListRecords.cs`](../examples/AddWebScreenWithListRecords.cs) — a
  full worked screen (Preparation + aggregate + ListRecords + widget tree) in full-lambda form.
- [`../SKILL.md`](../SKILL.md) § 3 (recoverable errors — CS0104 / CS0311), § 9 (security).
