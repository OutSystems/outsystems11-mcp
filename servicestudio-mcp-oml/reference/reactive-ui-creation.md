# Building a Reactive screen from scratch

How to create a Reactive Web screen and its widget tree via `applyModelApiCode`.
Companion to [`traditional-ui-creation.md`](traditional-ui-creation.md) (which is for
Traditional Web only). Reactive/Mobile screens and widgets use the `IMobileScreen` /
`IMobileWidget` Model API names **even for non-mobile Reactive Web screens** — this is a
naming quirk, not a signal that you're building a phone app.

> **Read the two live-verified references first**, then use this file for the end-to-end
> screen-build walkthrough:
> - [`osui-verified-blocks.md`](osui-verified-blocks.md) — the API-correct OutSystemsUI
>   **block catalog**, per-archetype composition, and exact design tokens. Prefer a
>   composite block (`Counter`, `CardSectioned`, `Gallery`, chart blocks…) when the module
>   consumes it; the `Card`-based tile and CSS-bar recipes below are the **fallback** for
>   when it doesn't (or when no chart dependency is present).
> - [`reactive-widget-api.md`](reactive-widget-api.md) — verified widget-creation API and
>   the **runner-crash traps** (`Func<…>` object creation, `*Attribute`/`ClientActions`,
>   `MarginTop`, unqualified `IExpression`, `AggregationType.Average`, aggregate
>   auto-rename). Read the crash list before writing widget code.
>
> What's unique to this file: the screen-build sequence, and three things not covered by
> those two — the grouped-aggregate **reference-by-alias** rule, **data-bound inline styles**
> via `CreateExtendedProperty`, and the **real-chart-vs-CSS-fallback** decision.

Most facts below were verified live building a KPI + forecast-chart dashboard screen
(2026-07-20). Block and placeholder **names** (`LayoutTopHeader` vs `LayoutTopMenu`,
`MainContent`, `Title`, `Header`, …) vary by app template — **discover them** by reading an
existing screen in the same module with `getScreen` and pattern-matching, rather than
assuming.

## 1. Screen scaffold

```csharp
var flow = eSpace.MobileFlows.Named("MainFlow");           // or a feature flow
var scr  = flow.CreateScreen("OrderForecast");
scr.Widgets.ToList().ForEach(w => w.Delete());             // a fresh screen has default widgets — clear them
scr.Description = "…";
scr.SetTitle("\"Order Management - Forecast\"");           // title is an EXPRESSION → keep the inner quotes
scr.HorizontalPosition = 685; scr.VerticalPosition = 950;  // SET THESE or screens stack at one spot
scr.Public = false;
// A new screen already lists Registered + EVERY app role. Gate it by REMOVING what must not see it:
scr.Roles.Remove(eSpace.RegisteredRole);                    // otherwise any signed-in user gets in
foreach (var r in scr.Roles.ToList())
    if (r.Name != "OrderRepresentative" && r.Name != "OrderManager") scr.Roles.Remove(r);
// unauthorized users now hit InvalidPermissions
```

For a **public** screen add the Anonymous system role: `scr.Roles.Add(eSpace.AnonymousRole);`.
Don't use `scr.AnonymousAccess` — `omlMerge` drops it, so the live screen stays
authenticated-only (verified 2026-09-30). Details:
[`patterns-by-element.md`](patterns-by-element.md) § "Screen access", example
[`../examples/SetScreenAccess.cs`](../examples/SetScreenAccess.cs).

Screen aggregates: `scr.CreateScreenAggregate(false, "GetX")` — first bool is
`isClientSide` (`true` only for Local Storage / client entities; `false` for database
entities). Aggregates with `Fetch = AtStart` (the default) load automatically, so a
read-only dashboard needs **no** `OnInitialize` logic. For group-by / join / aggregated
attributes and the **reference-by-alias** rule, see
[`patterns-by-element.md`](patterns-by-element.md) § "Grouped aggregate".

**Custom aggregate names (`GetForecastTotals`, not the default `Get<Plural>`) are immune to
the auto-rename trap** — so you can safely build the aggregates *and* the widgets that
reference them in **one** lambda, with no mid-build read-back pause.

## 2. Widget types (the Reactive set)

Fully-qualify these — the `ServiceStudio.Plugin.NRWidgets` namespace is not in the host
default imports. Adding it to `imports` covers most of them, but **`IExpression` must be
written out in full regardless**: the sidecar pre-imports `OutSystems.Model.Expressions`,
which declares its own `IExpression`, so `CreateWidget<IExpression>()` silently binds the
wrong type and hard-crashes the runner (no compile error). Qualifying every widget type is
the safe habit.

| Widget | Type |
|---|---|
| Container (div) | `ServiceStudio.Plugin.NRWidgets.IContainer` |
| Expression (bound value) | `ServiceStudio.Plugin.NRWidgets.IExpression` |
| List (repeater) | `ServiceStudio.Plugin.NRWidgets.IList` |
| Dropdown / Input / Link | `ServiceStudio.Plugin.NRWidgets.IDropdown` / `IInput` / `ILink` |
| Static text | `OutSystems.Model.UI.Mobile.Widgets.ITextWidget` |
| If (conditional) | `OutSystems.Model.UI.Mobile.Widgets.IIfWidget` |
| Block instance | `OutSystems.Model.UI.Mobile.Widgets.IMobileBlockInstanceWidget` |

- `container.CreateWidget<T>()` builds children; `IExpression.SetValue("GetX.List.Current.Y")`
  binds a value; `ITextWidget.Text = "…"` is literal text.
- `IIfWidget`: `.SetCondition("GetX.List.Empty")`, then `.TrueBranch.CreateWidget<T>()` /
  `.FalseBranch.CreateWidget<T>()` — gate a `List` against an empty-state message.
- `IList`: `list.SetSource("GetX.List")`, then add the row template with
  `list.CreateWidget<IContainer>()`; inside, bind `GetX.List.Current.<attr>`.
- **Reactive `IExpression` has NO `EscapeContent` property** (that is Web-only — see
  [`traditional-ui-creation.md`](traditional-ui-creation.md)). You **cannot** inject raw
  HTML through a Reactive Expression. For anything HTML/style-shaped, use an extended
  property (§4).

## 3. Layout scaffold + block instances

Instantiate a block: `inst = parent.CreateWidget<IMobileBlockInstanceWidget>();
inst.SourceBlock = <IMobileBlock or IMobileBlockSignature>;`. Fill a placeholder by name and
set block inputs by name:

```csharp
var common = eSpace.MobileFlows.Named("Common");
var layoutBlk = common.Nodes.OfType<OutSystems.Model.UI.Mobile.IMobileBlock>().Named("LayoutTopHeader");
var layout = scr.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.IMobileBlockInstanceWidget>();
layout.SourceBlock = layoutBlk;
layout.SetArgumentValue(layoutBlk.InputParameters.Named("HasFixedHeader"), "True");
var phMain = layout.PlaceholdersContent.FirstOrDefault(p => p.Placeholder?.Name == "MainContent");
var main   = phMain.CreateWidget<ServiceStudio.Plugin.NRWidgets.IContainer>();
```

To find an OSUI block regardless of which OSUI sub-flow holds it, search across flows
instead of hard-coding the flow name:

```csharp
var osui = eSpace.References.Named("OutSystemsUI");
var card = osui.MobileFlows.SelectMany(f => f.Nodes)
               .OfType<OutSystems.Model.UI.Mobile.IMobileBlockSignature>().Named("Card");
```

**Check the block is actually consumed first.** `AddDependency` is **not supported over
MCP** (SKILL.md §8), so you can only instantiate blocks the module already references. Probe
by **enumerating the reference directly**: `References.Named("OutSystemsUI").MobileFlows.SelectMany(f => f.Nodes).OfType<IMobileBlockSignature>()`. (`searchModel` no longer
exists in the MCP host — see SKILL.md §8 — so it isn't an option here either.) A common
consumed set is `Card`, `BlankSlate`, `Tag`, `Columns2/3`, `ColumnsMediumRight`; canonical
KPI blocks like `Counter` / `CardSectioned` are frequently **not** consumed — build tiles
from `Card` instead (§4).

## 4. KPI tiles and data-bound inline styles (CSS-bar "chart")

**KPI tile from a `Card`** (no `Counter` block needed):

```csharp
var cardInst = wrap.CreateWidget<IMobileBlockInstanceWidget>();
cardInst.SourceBlock = card;
cardInst.SetArgumentValue(card.InputParameters.Named("UsePadding"), "True");
var cc = cardInst.PlaceholdersContent.FirstOrDefault(p => p.Placeholder?.Name == "Content");
var labWrap = cc.CreateWidget<IContainer>();
labWrap.CustomStyle = "font-size:12px; text-transform:uppercase; color:var(--color-neutral-7);";
labWrap.CreateWidget<ITextWidget>().Text = "Weighted Forecast";
var valWrap = cc.CreateWidget<IContainer>();
valWrap.CustomStyle = "font-size:26px; font-weight:700; color:var(--color-neutral-10);";
valWrap.CreateWidget<ServiceStudio.Plugin.NRWidgets.IExpression>()                  // MUST be qualified
       .SetValue("GetForecastTotals.List.Current.WeightedForecast");
```
Lay four out side-by-side: a flex row (`CustomStyle="display:flex; flex-wrap:wrap; gap:16px;"`)
with each tile in a `flex:1 1 180px` wrapper. Design tokens available as CSS vars:
`--color-neutral-0..10`, `--color-primary`, `--space-*`, `--border-radius-*`.

**Data-bound inline style via `CreateExtendedProperty()`.** `CustomStyle` is a *static*
string; to make a style value depend on row data (a bar width, a conditional color) add an
HTML attribute whose value is an **expression**:

```csharp
var track = row.CreateWidget<IContainer>();
track.CustomStyle = "background:var(--color-neutral-3); border-radius:6px; height:12px; overflow:hidden;";
var bar = track.CreateWidget<IContainer>();
var ep = bar.CreateExtendedProperty();
ep.Property = "style";                                     // the HTML attribute name
ep.SetValue("\"height:12px; border-radius:6px; background:var(--color-primary); width:\" "
          + "+ If(GetForecastTotals.List.Current.WeightedForecast = 0, 0.0, "
          + "GetForecastByStage.List.Current.StageForecast / GetForecastTotals.List.Current.WeightedForecast * 100) "
          + "+ \"%\"");
```
Two expression-language points that bite here:
- OutSystems **auto-converts number → text** in `+` concatenation, so `"width:" + 42 + "%"`
  is valid (no `NumberToText` needed).
- **Both branches of `If(...)` must be the same type** — use `0.0` (Decimal), not `0`
  (Integer), when the other branch is a Decimal, or you get a type-mismatch validation error.

## 5. Charts: real chart vs CSS fallback

- **Real chart (Donut/Line/Column):** requires the `OutSystemsCharts` reference. If it's
  already resident, follow [`../examples/AddDashboardScreenWithDonutChart.cs`](../examples/AddDashboardScreenWithDonutChart.cs)
  — instantiate the chart block and bind `DataPointList` with an
  `ExpressionDefinition.Parse("GetX.List mapTo { Value: Count, Label: Label }")` (chart field ← aggregate **alias**)
  **Do NOT use `new ExpressionDefinition.TypeConversion(...)` — it compiles but hard-crashes the sidecar (verified 2026-07-28); `Parse(... mapTo ...)` is the working path over MCP.**
- **If `OutSystemsCharts` is NOT referenced**, you're stuck — `AddDependency` can't pull it
  in over MCP. Either ask the user to add it in Service Studio (Manage Dependencies) first,
  **or** render a CSS "chart" with no dependency: a `List` over a grouped aggregate, each
  row = a label + value + a track/bar pair with the data-bound width from §4. This is fully
  native and needs only `IContainer` / `IExpression` / `IList`.

### OutSystemsCharts details (verified 2026-10-01)

- **Default add-ons.** A newly created chart instance (Bar/Column/Line/…) already holds
  these block instances in its single placeholder, `AddOns_Placeholder`:
  - `ChartXAxis`
  - `ChartYAxis`
  - `ChartLegend`
  - one `ChartSeriesStyling` with an empty `SeriesName`

  Delete the blank series styling before adding your own.
- **Series colours.** `DataPoint.Color` colours the bars or points, but the legend keeps
  the default palette, so the two don't match. To colour a series:
  1. Add one `ChartSeriesStyling` per series in `AddOns_Placeholder`, with `SeriesName`
     equal to the `DataPoint.SeriesName`.
  2. Set its `Styling` to a screen variable of type `SeriesStyling` (from OutSystemsCharts).
  3. Assign `Var.FillColor` and `Var.LineColor` in OnInitialize. A block argument can't
     take a structure literal.
- **`StackingType` records:** `Stacked`, `NoStacking`, `Stacked100Percent`. There is no `Normal`.
- **Long category labels** (unit names, for example) are cut off on a ColumnChart's x-axis.
  A stacked `BarChart` (horizontal) shows them in full and takes the same inputs.
- **Chart data from a structure-list data action.** Build `LS:DataPoint` in the data
  action's `OnAfterFetch` client action: `ListClear`, then a For Each with an Assign to a
  `DataPoint` variable followed by `ListAppend`.

### OutSystems UI block gotchas (verified 2026-10-01)

- **`Color` static entity.** Records: `Neutral0`..`Neutral10`, `Primary`, `Secondary`,
  `Blue`, `Green`, `Orange`, `Red`, `Violet`, `Teal`, `Yellow`, `Lime`, `Pink`, `Grape`,
  `Indigo`, `Cyan`, `Transparent`. There is **no** `Neutral`.
- **`Alert` type** is `Entities.Alert.Warning` / `.Error` / ….
- **`Counter`** inputs: `BackgroundColor`, `IsVertical`, `Height`, `ExtendedClass`.
  - Two content containers in its `Content` placeholder sit side by side, so long labels
    wrap badly. Set `IsVertical = True`.
  - For a light KPI card: `BackgroundColor = Entities.Color.Neutral0` plus an `ExtendedClass`
    styled by CSS in the theme stylesheet (see SKILL.md § 3.1 and § 7 below).
- **`Section` and `Card`** take `UsePadding` and `ExtendedClass`. The default Section title is
  large and sits close to the preceding card; give it an `ExtendedClass` with
  `margin-top` / `font-size` rules.
- **`Pagination`** inputs: `StartIndex`, `MaxRecords`, `TotalCount`.
  - Pass `TotalCount` as `LongIntegerToInteger(Agg.Count)`.
  - Wire the handler with
    `pg.EventHandlers.First(h => h.Event.Name == "OnNavigate").Handler = screenAction`.
    The screen action takes `NewStartIndex` (Integer).
- **`Tabs`** ships with 3 fixed header/content items. Adding more by API was not verified;
  a button bar with one If panel per tab works.
- **Upload widget.** It contains a default `Image` placeholder that renders as a large grey
  picture. Delete `upload.Widgets` and put a small text or If in its place.
  `IUpload` has no `Variable` getter; read the bound variable from the inner image's
  `ImageContent.ToString()`.
- **Dropdowns.** Over a **structure list** (data-action output), `SetValues` / `SetLabels`
  take **bare attribute names** (`"UserId"`). Over an **aggregate** they take `Entity.Attr`.
- **Block inputs** are set with `inst.SetArgumentValue(sig.InputParameters.First(p => p.Name == "X"), expr)`.
  **Navigation and event arguments** are set with
  `link.OnClick.Arguments.First(a => a.Parameter.Name == "X").SetValue(expr)`.
- **Local variable defaults must be literals.** Anything computed (`CurrDate()`,
  `AddDays(...)`, an input parameter) goes in an OnInitialize screen action, wired with
  `screen.OnInitialize.Destination = action`.
- **Enum names:**
  - Data-action fetch: `DataSourceFetch.OnDemand`.
  - Upload accept: `Accept.Image`.
- **`IMobileWidget` has no `Name`.** Cast to the concrete widget type to read it.

### Aggregates with chained joins

For 3+ sources, the join's `LeftSource` must be **the source named on the left of the
condition**, not `MasterSource`. Otherwise TrueChange reports join errors such as
`Asset~Inspection.AssetId…` joined against the wrong side:
```csharp
var src = d.CreateSource(En("Unit")); var j = d.CreateJoin();
var leftName = "Asset.UnitId = Unit.Id".Split('.')[0].Trim();          // "Asset"
j.LeftSource = d.Sources.FirstOrDefault(s => s.Name == leftName && s != src) ?? d.MasterSource;
j.RightSource = src; j.JoinType = JoinType.Inner; j.SetCondition("Asset.UnitId = Unit.Id");
```

## 6. Verify

After the UI mutation, run `getValidationMessages` with `filter: ["error"]` **once** — it
catches every bad expression path (wrong aggregate alias, unknown attribute, type mismatch)
in a single call, which is far cheaper than eyeballing the widget tree. Then `omlMerge` the
returned `mutatedOmlPath`.

Related: [`patterns-by-element.md`](patterns-by-element.md) (aggregates, grouped-alias rule,
screen scaffold), [`model-api-tips.md`](model-api-tips.md) (naming gotchas),
[`../examples/`](../examples/) (`Add*Screen*.cs`, `AddDashboardScreenWithDonutChart.cs`).

## 7. Styling and visual verification (verified 2026-10-02)

- **Stylesheets:** put app-wide CSS in the **theme stylesheet**. Edits there published 3 times
  out of 3, including repeated edits, as long as `IconLibrary` is left null and the sidecar's
  `InvalidIconLibrary` error is ignored (SKILL.md § 3.1). Block and screen stylesheets were
  less reliable: a write to an empty one deployed every time we tried, but edits to a
  non-empty one often didn't (SKILL.md § 8). Whatever you edit, confirm it deployed:
  `curl https://<host>/<App>/moduleservices/moduleinfo` lists `css/<App>.<Flow>.<Element>.css?<hash>`.
- **Inline styles:** `widget.CustomStyle` does not render on buttons. An extended property
  `style` is compiled, but any `!important` inside it is dropped when the page renders.
  Use a CSS class instead.
- **OSUI login screen:** `.login-screen` is the full-screen wrapper; the card is `form.login-form`.
  **Side menu:** `.app-menu-content` is already `position: fixed; top: 0; bottom: 0`, so don't add
  `min-height` to `.aside-navigation`. That wrapper spans the full width.
- **Screenshot loop:** use `puppeteer-core` with the locally installed Chrome. Log in by clicking
  the Login button; pressing Enter doesn't submit the OSUI login form. Pass credentials through
  environment variables. The app scrolls inside a container rather than the window, so scroll that
  element; `fullPage` alone won't capture the whole page. Tile shots into contact sheets so many
  screens can be reviewed in one image.
