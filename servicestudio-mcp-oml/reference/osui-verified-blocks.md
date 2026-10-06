# OutSystemsUI Blocks & Design Tokens — API-Verified

**Status**: Live-verified against the real StyleGuidePreview app (all 18 app templates probed via `applyModelApiCode` widget-tree traversal) + OutSystemsUI stylesheet. Use these exact block/widget names and tokens when composing Reactive screens.
**Verified**: OutSystemsUI **2.27.0** (design-token values) / StyleGuidePreview 1.2.18. Version-sensitive bits are flagged — re-verify with a live probe if the platform version differs.
**Companion**: [`reactive-widget-api.md`](reactive-widget-api.md) (widget-creation mechanics + runner-crash traps). This file + that one are the **API source of truth**; [`styleguide-ui-patterns.md`](styleguide-ui-patterns.md) is a design-intent/UX catalog only (defer to here for actual code). *(A former `styleguide-recipes.md` containing inaccurate Model API code was removed 2026-07-08.)*

> **Why this exists:** the older styleguide references named methods that don't exist in the live Model API (`IMobileListWidget`, `GetPlaceholder`, `.Aggregate=`, `SetSourceRecordList`). Everything below was confirmed by reading real StyleGuidePreview widget trees.

## Use composite OSUI blocks by DEFAULT (first pass — not a later "polish" step)
When building Reactive UI, reach for the OutSystemsUI composite blocks **first**. Do **not**
default to plain styled `IContainer`s and only upgrade to blocks when asked — plain containers +
design tokens are a **fallback** for when no block fits. The blocks in the contract table below are
live-verified and safe to build directly (no per-build probing needed). A screen that shows KPIs as
styled `<div>`s, statuses as plain text, empty lists as a bare table, or catalog items as table rows
is **under-built** — use `Counter`, `Tag`, `BlankSlate`, `CardSectioned` respectively.

| Need | Use this block (NOT a plain container/text) |
|---|---|
| KPI / stat tile | `Counter` |
| Status / category chip | `Tag` (color-coded via the `Color` input) |
| Empty list / no-data state | `BlankSlate` |
| Record card / catalog item | `CardSectioned` (or `Card`) |
| Proportional / breakdown data | `DonutChart` / `ColumnChart` / `PieChart` / `LineChart` |

## Verified block contracts (live-probed 2026-07 · OutSystemsUI 2.27 / OutSystemsCharts)
Placeholders + inputs below were read from **real instances** — build straight from them, no exploration required.

| Block (flow) | Placeholders | Key inputs |
|---|---|---|
| `Numbers/Counter` | `Content` | BackgroundColor (Color id), IsVertical, Height, ExtendedClass |
| `Content/Tag` | `Tag` | **Color (Color id)**, Size, Shape, IsLight, ExtendedClass |
| `Content/BlankSlate` | `Icon`, `Content`, `Actions` | FullHeight, ExtendedClass |
| `Content/Card` | `Content` | (padding/style inputs) |
| `Content/CardSectioned` | `Image`, `Title`, `Content`, `Footer` | UsePadding, IsVertical, ImagePadding, ExtendedClass |
| `Charts/DonutChart` | `AddOns_Placeholder` | **DataPointList** (DataPoint List), Height, InnerSize, OptionalConfigs |
| `Charts/ColumnChart` | `AddOns_Placeholder` | **DataPointList**, StackingType, ValuesType, OptionalConfigs |

Resolve + instantiate a block:
```csharp
var blk = eSpace.References.Named("OutSystemsUI").MobileFlows.Named("Content")
    .Nodes.OfType<OutSystems.Model.UI.Mobile.IMobileBlockSignature>().Named("Tag"); // charts: ref "OutSystemsCharts", flow "Charts"
var inst = parent.CreateWidget<OutSystems.Model.UI.Mobile.Widgets.IMobileBlockInstanceWidget>();
inst.SourceBlock = blk;                                                             // then:
inst.SetArgumentValue(blk.InputParameters.Named("Color"), "<expr>");               // set an input
var ph = inst.PlaceholdersContent.FirstOrDefault(p => p.Placeholder?.Name == "Tag"); // fill a placeholder
ph.CreateWidget<ServiceStudio.Plugin.NRWidgets.IExpression>().SetValue("...");
```
`inst.CustomStyle` is safe for per-instance spacing (do **not** use `.MarginTop`/`.MarginBottom` — those setters crash).

**`Tag`/`Counter` `Color` = an OutSystemsUI `Color` identifier.** Reference records in an expression as
`Entities.Color.<Record>`; **valid records include `Green, Red, Orange, Blue`** (more exist) — `Success`/`Error`
are **not** records. For per-row color, pass an `If(...)` to the `Color` arg, e.g.
`If(GetX.List.Current.Loan.LoanStatusId = Entities.LoanStatus.Overdue, Entities.Color.Red, Entities.Color.Green)`.
(You may reference `Entities.Color.X` in expressions even though *enumerating* `Color.Records` crashes the runner.)

**`BlankSlate` empty-state pattern:** `IIfWidget` cond `GetX.IsDataFetched and GetX.List.Empty` →
TrueBranch = BlankSlate (message in its `Content` placeholder), FalseBranch = the table/list.

### Chart data recipe (`DataPointList`)
The list element is the `OutSystemsCharts` structure **`DataPoint { Value, Label, SeriesName, Color, Tooltip }`**.
Aggregates can't live in client/screen actions, so build the list in a **server** action, then load it in the
screen's `OnInitialize` into a local var the chart binds to:
```csharp
var dpStruct  = eSpace.References.Named("OutSystemsCharts").Structures
    .OfType<OutSystems.Model.Data.IStructureSignature>().First(s => s.Name == "DataPoint");
var listType  = eSpace.GetOrCreateListType(dpStruct);                         // "DataPoint List"
var listAppend = eSpace.References.Named("(System)").ServerActions.Named("ListAppend"); // inputs: List, Element
// server action: out Points(listType) + local Dp(dpStruct); per slice:
//   Assign Dp.Label="Available", Dp.Value = GetAvailAgg.Count
//   ExecuteServerAction ListAppend  (List=Points, Element=Dp)   // records copy by value → reuse one Dp
// screen: local ChartData(listType); OnInitialize action → call server action → Assign ChartData = Call.Points
//   chartInst.SetArgumentValue(donut.InputParameters.Named("DataPointList"), "ChartData")
```

### Probe technique (only for blocks NOT in the table above)
Instantiate the block on a throwaway screen, print `blk.InputParameters.Select(p => p.Name)` and
`inst.PlaceholdersContent.Select(p => p.Placeholder?.Name)`, then `scratch.Delete()`. **Never** recursively
walk `.Widgets` to introspect — accessing `.Widgets` on a leaf widget hard-crashes the runner; read one level
with typed `as`/`is` checks only.

## Runtime widget type names (what `GetType().Name` returns) — create via the `I*` interface
`List`(IList), `ListItem`, `Dropdown`(IDropdown), `TableRecords`(ITableRecords), `TextArea`, `Upload`, `Popup`, `Button`, `ButtonGroup`, `Checkbox`, `Image`, `Icon`, `Expression`, `AdvancedHtml`, `Text`(ITextWidget), `Label`, `Link`, `If`(IIfWidget), `WebBlockInstance`(IMobileBlockInstanceWidget — note it's "Web" even in Reactive).

## OutSystemsUI block catalog actually used (block name → placeholders / purpose)
**Layouts** (screen root block): `LayoutTopMenu` {Header(→Menu), Breadcrumbs, Title, Actions, MainContent, Footer} — default for app screens. `LayoutBase` + `LayoutBaseSection` {BackgroundImage, Content} — marketing/landing. `LayoutSideMenu` also exists. Page title = `AdvancedHtml 'Screen_Title'` in Title; action buttons go in **Actions** (not inside the Form). `Menu` block sits in Header.
**Columns/structure:** `Columns2/3/4/5/6`; weighted `ColumnsSmallLeft/SmallRight`, `ColumnsMediumLeft/MediumRight`; `Section` {Title,Content}; `Separator`; `AlignCenter` {Content}; `CenterContent` {Center}; `ScrollableArea` {Content}; `FloatingContent` {Content} (overlay, e.g. over a Map); `DisplayOnDevice` {OnPhone,OnTablet,OnDesktop} (responsive show/hide).
**Cards:** `Card` {Content}; `CardSectioned` {Image,Title,Content,Footer} (rich image card); `CardBackground` {Content,BackgroundImage}.
**Data display:** `Gallery` {Content} (responsive card GRID — put a `List` inside, one card per row); `List`+`ListItem`/`ListItemContent` {Left,Title,Content}; `TableRecords`; `Pagination` {Previous,Next}; `Accordion`+`AccordionItem` {Title,Content}; `Tabs`+`TabsHeaderItem` {Title}+`TabsContentItem` {Content}; `MasterDetail` {LeftContent,RightContent} (list+detail).
**Inputs/forms:** `Form` wraps fields; `Search` {Input}; `DatePicker` {Datepicker→InputWithIcon}; `InputWithIcon` {Icon,Input}; `RangeSliderInterval` (numeric range filter); Label+Input / Label+Dropdown / Label+TextArea / Upload / Checkbox(in AlignCenter).
**Feedback/state:** `BlankSlate` {Icon,Content,Actions} (empty); `Counter` {Content: number Expression + label + Icon} (KPI stat tile — use for dashboard tiles); `ProgressBar`; `Tag` {Tag} (status/filter chip); `Badge`; `Rating` {FilledState,HalfState,EmptyState}; `Animate` {Content: AnimationType, Delay, Speed}.
**Charts** (OutSystemsCharts ref): `LineChart_v1`, `DonutChart_v1` (+ Column/Bar/etc.) — wrap in a card + `If HasData → chart / else BlankSlate`.
**Media/misc:** `Map` {LicenseWarning} (OutSystemsMaps); `LightboxImage` {Thumbnail}; `InlineSVG`.
**AVOID (deprecated in the library):** `DEPRECATED_Carousel`, `DEPRECATED_HorizontalScroll`.

## Verified composition recipes by archetype
- **Gallery + filters** (FourColumnGallery): MainContent→`ColumnsSmallRight`→Col1: `Gallery`→`List`(src=Get.List)→`CardSectioned` + `Pagination`; Col2: `Search` + `CardSectioned`(category `List` + `RangeSliderInterval`).
- **Catalog w/ list⇄grid toggle** (ProductCatalog): Search + sort Dropdown + view-toggle Buttons; advanced filters in an `Animate` reveal; `If ShowGallery` → `Gallery`+`CardSectioned` / else `List`+`ListItemContent`; loading/empty container; `Pagination`. List items wrapped in `Animate`.
- **Form** (ProductForm/RequestCreation): buttons in `[Actions]`; `Form`→`ColumnsMediumLeft`(Image+`Upload` | fields)→nested Columns rows; `Label`+`Input`/`Dropdown`/`TextArea`/`DatePicker`; checkbox in `AlignCenter`; `Popup` for delete-confirm. Input `var = GetById.List.Current.<Entity>.<Attr>`; Dropdown `Values`/`Labels` use **entity-name scope** (`Sample_ProductCategory.Id`, NOT `GetX.List.Current…`). Title uses edit/new `If Id <> NullIdentifier()`.
- **List + detail** (Employeeslistanddetail/directory): `MasterDetail` {LeftContent:`List`; RightContent:`If Selected<>NullIdentifier()` → detail(`Section`/Columns) / else `BlankSlate`}. Often inside `Tabs`; filter via `ButtonGroup`+`Search`. Simpler variant: `Accordion`+`AccordionItem`.
- **Dashboard** (Dashboard/AdminDashboard/TransactionsDashboard): KPI row = `Columns3` of `Counter`; charts = `Columns2` of `CardSectioned`→`If HasData`→`Chart`/`BlankSlate`; data = `CardSectioned`→`TableRecords` + loading/empty container.
- **Management** (RequestManagement): `Counter`+`ProgressBar` header; `ButtonGroup` status filter; mini-stat `Card`s + `LineChart`; `Tag` filter chips; `Search`; `TableRecords`+`Pagination`.
- **Rich detail** (ProductFeature/LocationDetail): `Columns2`(media | info); `Rating`; action Buttons; `Card`+`Columns6` spec grid / `Columns4` feature icons; related items `Gallery`; `LightboxImage` galleries; `Map`+`FloatingContent`+`ScrollableArea` (BranchLocator); reviews in `Section`.
- **Onboarding** (OnboardingWithAnimation): `CardBackground` split bg + `CenterContent` + `FloatingContent` action button. **Landing** (Homepage): `LayoutBase`→`LayoutBaseSection` hero (BackgroundImage+AdvancedHtml) + feature `Columns3` of `Card` + `Columns5` grid.

## Key idioms
- **Empty/loading:** `If GetX.IsDataFetched and GetX.List.Empty` (empty) / `If not GetX.IsDataFetched` (loading) / `If not GetX.List.Empty` (has data). `BlankSlate` for empty.
- **Data:** cached lookup aggregate (Fetch AtStart) + main aggregate (filtered/paginated); reset StartIndex=0 on filter change, then a RefreshData node.
- **Responsive:** `DisplayOnDevice`, weighted Columns, `Entities.BreakColumns.All/Middle` on Columns `PhoneBehavior`/`TabletBehavior`.
- **Stagger animation:** wrap the list item in `Animate`, `Delay = GetX.List.CurrentRowNumber * ~60`, `AnimationType = Entities.AnimationType.FadeIn` (**`FadeIn` is valid; `FadeInUp` is not** — set a value and let validation confirm; enumerating `AnimationType.Records` on the referenced entity crashes the runner).

## Design tokens (OutSystemsUI CSS custom properties — EXACT values, v2.27.0)
Use in `CustomStyle` / `StyleClasses` via `var(--…)`. (Base themes return only a dependency stub via `getTheme`; the authoritative stylesheet is fetchable at `/<Module>/css/OutSystemsUI.OutSystemsUI.css`.)
- **Neutral scale:** `--color-neutral-0`=#ffffff, -1=#f8f9fa, -2=#f1f3f5, -3=#e9ecef, -4=#dee2e6, -5=#ced4da, -6=#adb5bd, -7=#6a7178, -8=#4f575e, -9=#272b30, -10=#101213.
- **Brand:** `--color-primary`=#1068eb, `--color-primary-hover`=#295fd6, `--color-secondary`=#303d60, `--color-focus-outer`=#ffd337.
- **Semantic:** `--color-error`=#dc2020 (+`-light` #fceaea), `--color-warning`=#e9a100 (+`-light` #fdf6e5), `--color-success`=#29823b (+`-light` #eaf3eb), `--color-info`=#017aad (+`-light` #e5f5fc).
- **Extended palette:** 12 hues `red/orange/yellow/lime/green/teal/cyan/blue/indigo/violet/grape/pink`, each ×7 steps `-lightest/-lighter/-light/(base)/-dark/-darker/-darkest` (e.g. `--color-blue`=#1a79cb, `--color-violet`=#7048e8).
- **Spacing:** `--space-none`=0, `-xs`=4px, `-s`=8px, `-base`=16px, `-m`=24px, `-l`=32px, `-xl`=40px, `-xxl`=48px.
- **Border radius:** `-none`=0, `-soft`=4px, `-rounded`=100px, `-circle`=100%. **Border size:** `-none`=0, `-s`=1px, `-m`=2px, `-l`=3px.
- **Shadow:** `--shadow-none/xs/s/m/l/xl` (xs=`0 1px 2px rgba(0,0,0,.1)` → xl=`0 8px 10px …`).
- **Typography size (px):** `--font-size-h1`=32, h2=28, h3=26, h4=22, h5=20, h6=18, `-display`=36, `-base`=16, `-s`=14, `-xs`=12, `-label`=11. **Weight:** `--font-light`=300, `--font-regular`=400, `--font-semi-bold`=600, `--font-bold`=700.
- **App/layout:** `--color-background-body`=#f3f6f8, `--header-size`=56px, `--header-size-content`=48px, `--side-menu-size`=300px, `--bottom-bar-size`=56px, `--overlay-background`=rgba(0,0,0,.25). Plus a `--layer-*` z-index system and `--os-safe-area-*` iOS insets.

Prefer tokens over hardcoded px/hex: `border:var(--border-size-s) solid var(--color-neutral-4); padding:var(--space-m); border-radius:var(--border-radius-soft); box-shadow:var(--shadow-s)`.

## How to extend this catalog (probe technique)
To capture a screen's real composition without dumping thousands of `getScreen` lines: run `applyModelApiCode` with a depth-limited `Action<…>` recursion over `screen.Widgets` that collects `GetType().Name` + Name + key bindings (List `.Source`, Dropdown `.Values`, Input `.Variable`, If `.Condition` via `.DisplayName`), recursing block instances via `.PlaceholdersContent[].Widgets`, `If` via `TrueBranch`/`FalseBranch`, and containers/forms/lists/links via `.Widgets` — using typed `is` checks (crash-safe), then returns the text via `throw new Exception("PROBE:" + s)` (the sandbox rejects `Console`). ~3 screens per call keeps output readable. For design tokens, WebFetch `/<Module>/css/OutSystemsUI.OutSystemsUI.css`.
