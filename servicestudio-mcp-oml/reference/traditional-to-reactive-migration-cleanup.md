# Traditional → Reactive migration cleanup (verified recipe)

**Status**: Live-verified cleaning up a real module produced by the OutSystems
Traditional→Reactive migration tool (`PVO_Tasks` → `PVO_Tasks_Reactive`,
2026-07). This file captures the facts that turn that job from an
explore-and-crash slog into a lookup. Read it **before** touching a migrated
module — it will save you a dozen round-trips.

**Companion**: [`reactive-widget-api.md`](reactive-widget-api.md) (the general
runner-crash traps), [`patterns-by-element.md`](patterns-by-element.md)
(aggregate/node basics).

## What a migrated module looks like

The migration tool reproduces Traditional page-load semantics literally, which
is exactly what it's asked to review/fix:

- Each content screen gets a **dummy `OnStart` Data Action** that only sets a
  `Ready = False` flag, with `OnStart.OnAfterFetch → Preparation`.
- The real fetch lives in a **`Preparation` *client* action** that calls a
  `…Srv` server action **synchronously** (an `ExecuteServerAction` node inside a
  client action) and copies the result into a **local variable** shaped like an
  anonymous structure `{ Count, List, StartIndex }`. Widgets bind to that local
  var (`GetX.List`, `GetX.Count`, `GetX.StartIndex`).
- Pagination / sort / search actions repeat the same inline
  `ExecuteServerAction` + assign.
- This is what raises **`ConsecutiveServerInteractions`** ("Preparation calls
  several Server Actions") and **`Block_ClientActionWithServerRequestsOnLifecycleEvent`**
  ("Preparation contains server accesses, delays render — use Aggregates or Data
  Actions instead") warnings.
- Common leftovers that are **validation errors**, not warnings: RichWidgets
  artifacts (e.g. a `List_BulkSelect*` `ExecuteServerAction` node with
  `Action == null`), Traditional-only functions surviving in expressions (e.g.
  `GetUser(GetUserId()).User.Name` → *Unknown function 'GetUser'*), and mandatory
  action input params passed `null` from a migrated event handler.

Start every migrated-module task with
`getValidationMessages({filter:["error"]})` — the errors ARE the "pending items"
to fix; the warnings drive the data-access review.

## The platform fact that reshapes the plan

**Reactive screen/block Data Actions have NO input parameters.** Calling
`dataAction.CreateInputParameter(...)` **hard-crashes the runner** (empty
`stdoutOutput` + empty `mutatedOmlPath`, no exception). A Data Action instead
**reads screen scope directly** — screen input parameters, **local variables,
and client variables are all in scope inside the data action's flow** and are
sent to the server on each fetch/refresh. Verified: an `ExecuteServerAction`
node inside a Data Action whose arguments reference `GetX.StartIndex` (local
var), a sort local var, and a `Client.*` client variable validates cleanly.

This is why you do **not** need — and cannot build — a parameterized data
action for server-side pagination. Use screen scope + `Refresh Data`.

## The conversion recipe (minimal-rebind, verified)

Goal: make the fetch asynchronous via a Data Action **without** rebinding the
deeply-nested table widgets (which is fragile — see crash traps). Keep the
migration's local variable as the binding surface.

Per data source on a screen:

1. **Repurpose the dummy `OnStart` Data Action to do the fetch.** Add output
   params matching the local var's `List` / `Count` types (rebuild the anon
   structure with `GetOrCreateListType(GetOrCreateAnonymousStructure(Tuple.Create<string,ITypeSignature>("Task", task), …))`
   — same component order as the local var so `GetOrCreate` returns the *same*
   type instance and the copy assignment type-checks). Insert an
   `IExecuteServerActionNode` calling the existing `…Srv` action, with arguments
   referencing screen scope (`GetX.StartIndex`, `"50"`, sort var, `Client.*`),
   then an assign node setting the outputs. Wire it into the existing flow with
   `ConnectedBelow`.
2. **Rewire `Preparation` (the `OnAfterFetch` handler) to copy, not fetch.**
   Delete the `ExecuteServerAction` node and its result-assign; add one assign
   `GetX.List = OnStart.<ListOut>; GetX.Count = OnStart.<CountOut>`. Keep any
   other assigns the migration put there (e.g. `TaskForm_Record.Record =
   GetX.List.Current`, `OnStart.Ready = True`).
3. **Convert refresh/sort/search/postback to `Refresh Data`.** Replace each
   inline `ExecuteServerAction` + assign with an
   `IRefreshDataNode` whose `.DataSource = <the data action>`. The pagination
   pre-assign (`GetX.StartIndex = List_Navigation_GetStartIndex(...)`) stays
   *before* the refresh; the data action's `OnAfterFetch` does the copy. A
   `PostbackHandler` that used to call `Preparation` should call `Refresh Data`
   instead.
4. **Split independent datasets into separate Data Actions.** If one screen
   loads several things (e.g. a task record **and** its paginated detail list),
   give each its own Data Action + its own `OnAfterFetch` copy handler.
   Otherwise a details-table pagination `Refresh Data(OnStart)` re-fetches the
   record too and **resets an edited form** — a real, user-visible regression.
   Create the extra data action with `screen.CreateDataAction("LoadDetails")`,
   delete its default output param, add outputs, build Start→call→assign→End
   yourself (CreateDataAction does **not** auto-create flow nodes), and set
   `ld.OnAfterFetch.Destination = <new screen action>`.

**Fixing the `GetUser` expression error the idiomatic way**: add a Data Action
with an aggregate over `(System).User` filtered `User.Id = GetUserId()`, output
a `UserName` Text (`assign UserName = <aggName>.List.Current.User.Name`), then
rebind the expression to `<DataAction>.UserName`. Resolve User via
`eSpace.References.Named("(System)").Entities.OfType<IServerEntitySignature>().Named("User")`
and pass it to `agg.AsDatabaseAggregate.CreateSource(user)`. Name the aggregate
something non-default (e.g. `GetCurrentUserRecord`) to dodge the auto-rename
trap.

## Crash traps hit specifically during migration cleanup

All of these produce the uncatchable-crash signature (empty `stdoutOutput` AND
empty `mutatedOmlPath`, no exception). **Nothing you did before a crash reaches
you** (and `Console` is rejected by the sandbox — use the throw-probe from
[`lambda-contract.md`](lambda-contract.md) § "Sandbox" for everything else);
bisect by removing statements instead. Treat introspection as
guilty-until-proven-safe:

For **walking flow nodes**, use the typed `node.Connectors` / `*Target` members
described in [`model-api-tips.md`](model-api-tips.md) § "Walking a flow" —
reflection is rejected by the sandbox.

- **`screen.ScreenDataSets` enumeration → crash.** (Use `screen.DataActions`,
  `screen.DataSets` cautiously; prefer `.DataActions.Named(...)`.)
- **`dataAction.CreateInputParameter(...)` → crash** (data actions have no
  inputs — see above).
- Don't try to discover members by reflection (it is sandboxed out, and on
  concrete types it also hid explicit-interface members). Look the interface up
  in [`../docs/`](../docs/) instead.

### Safe accessors (verified working on migrated screens/actions)

- `eSpace.MobileFlows.Named("MainFlow").Nodes.OfType<OutSystems.Model.UI.Mobile.IMobileScreen>().Named("X")`
- `screen.ScreenActions.Named(...)`, `screen.DataActions.Named(...)`,
  `screen.LocalVariables`, `screen.ScreenAggregates` (per `reactive-widget-api.md`:
  `screen.ClientActions` **crashes** — use `ScreenActions`).
- `action.Nodes.OfType<IStartNode|IEndNode|IAssignNode|IExecuteServerActionNode|IExecuteClientActionNode|IIfNode|IRefreshDataNode>()`,
  then `.Name`, `.Label`, `.Target`, `.Action`, `.DataSource`.
- `dataAction.OutputParameters`, `dataAction.OnAfterFetch.Destination`,
  `dataAction.Fetch`.
- Descending a widget tree ONE known-container level at a time is safe
  (`.Widgets.OfType<IContainer>()`, `IIfWidget.TrueBranch/.FalseBranch`,
  `blockInstance.PlaceholdersContent.First(p => p.Placeholder != null && p.Placeholder.Name == "Content")`);
  **recursively** walking `.Widgets`, or `.Widgets` on a leaf, crashes.
- Editing an existing expression widget: navigate to it and call `.SetValue("…")`.
  You generally **cannot** edit an individual `IAssignNode` assignment's RHS in
  place — delete the assign node and recreate it with the correct
  `CreateAssignment(target, value)` calls.

## Workflow that keeps it fast

1. `getValidationMessages({filter:["error"]})` first — that's the work list.
2. Read each target screen **once** (large screens exceed the token cap and get
   written to a file — slice it by character range with one `python read()[A:B]`
   pass, not many).
3. Fix the unambiguous logic-flow errors first (dead nodes, null mandatory args)
   — no widget-tree navigation, lowest risk.
4. For data-access: apply the repurpose recipe above, **one atomic mutation per
   step**, and run `getValidationMessages` after each (catches saved-but-invalid
   immediately). Fix forward — don't `omlReset` (it nukes the whole unmerged
   chain, including earlier good fixes).
5. `omlMerge` the final `mutatedOmlPath` once at the end.

## Layout / CSS tweaks — hand these to the user unless you can see the result

Visual layout fixes are **unreliable blind through the Model API** (no rendered
preview, can't iterate visually). Default to letting the user do them in Service
Studio, and if you do attempt one, keep it minimal and verify with them. If you have
a screenshot loop (see [`reactive-ui-creation.md`](reactive-ui-creation.md) § 7), put
the CSS in the **theme stylesheet** (SKILL.md § 3.1) and confirm the deployed CSS
changed before trusting a screenshot.

Two concrete lessons from a filter-row "input and buttons misaligned" fix
(verified 2026-07):

- **The cause was a height mismatch**, not alignment plumbing: the migrated
  `Input` (legacy `form-control ThemeGrid_Width4`) rendered a different height
  than the reactive `Button`s (~40px). The correct minimal fix was
  `input.CustomStyle = "height: 40px;"` to match the button height.
- **Do NOT override the wrapper with inline `display:flex`** — the migrated
  filter container carries an OutSystems UI theme class (e.g. `Filters_Wrapper`)
  that already lays the row out. An inline `display:flex; align-items:center; …`
  on that container *fights* the theme class and made it look worse; the user
  reverted it to `CustomStyle = null` and just matched the input height. When a
  widget already has a theme style class, adjust the mismatched child property,
  don't re-layout the parent inline.

## Scope note

"Review the data access in the **screens**" means the content screens. The
shared `Common` menu-layout blocks (`LayoutTopMenu` / `LayoutSideMenu` /
`Layout_Website`) also fetch on their `OnInitialize` and raise the same
performance warning, but they're shared UI and riskier to touch — call them out
as an optional follow-up rather than converting them unprompted.
