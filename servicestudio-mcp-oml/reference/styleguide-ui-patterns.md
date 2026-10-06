# StyleGuidePreview UI Patterns Reference

**Source**: StyleGuidePreview (Reactive Web v1.3.2) — 26 screens, 50+ patterns
**Status**: Production-ready, validated patterns from official OutSystems reference implementation
**When to use**: Any time you're building OutSystems screens — cite these patterns for consistency

> **DESIGN-INTENT REFERENCE** — This is a catalog of *what UI patterns to use and when* (layout, forms, data/state, tables, motion, mobile, accessibility). The bullets name real OutSystemsUI blocks/properties; the "Implementation Recipes" below are **natural-language pseudo-steps, not literal Model API code**. For the exact, API-correct code — widget interfaces, block placeholders/inputs, composition recipes, runner-crash traps, and design-token values — always defer to **[osui-verified-blocks.md](osui-verified-blocks.md)** + **[reactive-widget-api.md](reactive-widget-api.md)** (live-verified). A few specifics here are unverified (e.g. exact `AnimationType` record names, `SetSortAttribute`) — confirm against the verified files or a live probe.

## Quick Pattern Lookup

### Layout Patterns
- **Responsive Grid** — Use `Columns3` / `Columns5` blocks with `PhoneBehavior=All`, `TabletBehavior=Middle` for automatic breakpoint handling
- **Sidebar Layout** — Use `ColumnsSmallLeft` for main + sidebar composition with responsive stacking on mobile
- **Section Grouping** — Nest `LayoutBase` → `LayoutBaseSection` → `Columns` → `Cards` for semantic hierarchy
- **Fixed Header** — Use `LayoutTopMenu` block with `HasFixedHeader=True` for sticky navigation

### Form Patterns
- **Form with Validation** — Use built-in `Form.Valid` property; trigger validation with `ValidateAndContinue` button behavior
- **Label-Input Binding** — Set `label.TargetWidget = input` and `input.SetVariable(aggregateSource)` for accessibility
- **Animated Labels** — Use `AnimatedLabel` block to float labels above inputs on focus
- **Multi-Column Forms** — Use `Columns2` inside forms for responsive side-by-side input pairs

### Data & State Patterns
- **Filtered List** — Aggregate with `CreateFilter()` conditions + screen variables; refresh on filter change
- **Pagination** — Use `Pagination` block from Navigation; wire `OnNavigate` event to update `StartIndex`
- **Bulk Selection** — Track state with `SelectAllRows` (Boolean) + `SelectedRowsCount` (Integer) variables; intermediate checkbox state shows partial selection
- **Infinite Scroll** — Use `OnScrollEnding` event to increment `MaxRecords`; show loading state during fetch

### Table Patterns
- **Sortable Table** — Add `SetSortAttribute()` to header cells; toggle direction with conditional logic on sort column
- **Row Actions** — Use `SetSelectedTableRow` client action from OutSystemsUI to highlight rows
- **Table with Checkboxes** — Header checkbox (with intermediate state styling) controls row selection state
- **Bulk Operations** — ForEach loop over selected items with conditional logic for add/remove/update actions

### Animation Patterns
- **Cascading Entrance** — Use `Animate` block with `Delay = "GetProductsById.List.CurrentRowNumber * 300"` for staggered effect
- **Fade Transitions** — Use `AnimationType.FadeIn` for smooth enter/exit effects
- **Loading Spinners** — Use `Animate` with `AnimationType.Spinner` during data fetch
- **Stateview Transitions** — Conditional rendering with If nodes for different UI states

### Interactive Patterns
- **Map Integration** — Use Map widget from OutSystemsMaps; set center position + respond to `Map_Initialized` event
- **Floating Content** — Use `FloatingContent` block with responsive positioning: `If(IsPhone(), Position.Bottom, Position.TopRight)`
- **Tabs for Breakpoints** — Use `Tabs` block with `TabsHeaderItem` + `TabsContentItem` to show different content per breakpoint
- **Rating Display** — Use `Rating` block; bind value to aggregate field (e.g., `RatingAvg`)

### Mobile Patterns
- **Responsive Orientation** — Use `ScrollableArea.Orientation = If(IsPhone(), Horizontal, Vertical)`
- **Touch-Friendly Buttons** — Minimum 44×44px target; use `SetStyle()` for padding
- **Mobile Navigation** — Use `Menu` block (reference) in Header; menu auto-adapts to mobile
- **Bottom Sheets** — Use `FloatingContent` with `Position.Bottom` for mobile-first modals

### State Management Patterns
- **Loading State** — Use `IsExecuting` variable with `ButtonLoading` wrapper; show/hide spinners conditionally
- **Error Handling** — Create `AllExceptions` node in action; assign error state variables + display error message
- **Hover Effects** — Track `CurrentHoveredItem` variable; update on `onmouseover` / `onmouseout` custom events
- **Selection State** — Use screen variables for `CurrentSelectedItem`, `SelectedRowsCount`; bind UI visibility to these

## Implementation Recipes

### Recipe 1: Simple Filtered List
```
1. Create aggregates: GetCategories (AtStart, cached), GetItems (OnDemand, filtered)
2. Create screen variable: SelectedCategoryId
3. Add dropdown: bind to GetCategories, SetVariable=SelectedCategoryId
4. Add list: bind source to GetItems.List
5. Aggregate filter: If(SelectedCategoryId > 0, Category.Id = SelectedCategoryId, True)
6. Dropdown OnChange: RefreshData on GetItems
```

### Recipe 2: Table with Bulk Select
```
1. Create locals: SelectAllRows (Boolean), SelectedRowsCount (Integer)
2. Add table with checkboxes:
   - Header checkbox: SetVariable=SelectAllRows, style conditional for intermediate state
   - Row checkbox: SetVariable=GetData.List.Current.IsSelected
3. Create OnChangeBulkHeader action:
   - If SelectAllRows + some selected + not all selected: show "intermediate"
   - ForEach loop: update each row's IsSelected state
4. Add action button: enabled only if SelectedRowsCount > 0
```

### Recipe 3: Paginated List with Sort
```
1. Create locals: StartIndex=0, MaxRecords=20, TableSort=""
2. Aggregate: CreateFilter(TableSort), SetStartIndex(StartIndex), SetMaxRecords(MaxRecords)
3. Add Pagination block: wire OnNavigate → update StartIndex → RefreshData
4. Table OnSort: 
   - If TableSort = ClickedColumn: append " DESC"
   - Else: set TableSort = ClickedColumn (ascending)
   - Reset StartIndex = 0, RefreshData
```

### Recipe 4: Map with Floating Filter
```
1. Create screen aggregate: GetLocations with lat/lon fields
2. Add Map widget: SetArgumentValue(MapAddress, "lat,lon")
3. Add FloatingContent block:
   - Position = If(IsPhone(), Bottom, TopRight)
   - Inside: Filter UI (dropdowns, search input)
4. Filter OnChange: RefreshData on GetLocations
5. Map OnInitialized: optionally update center position
```

### Recipe 5: Responsive Form with Animated Labels
```
1. Use LayoutTopMenu (fixed header)
2. Add Form container
3. For each field:
   - Use AnimatedLabel block (floats on focus)
   - Delete default label/input placeholders
   - Add Input widget inside placeholder
   - Set Input.SetVariable(aggregate field)
4. Add Columns2 for larger screens (side-by-side fields)
5. Add ButtonLoading wrapper around Submit button
6. Form OnSubmit: validate with Form.Valid, execute server action, show result
```

## Common Mistakes to Avoid

❌ **Don't**: Use raw aggregates without pagination (causes performance issues on large datasets)
✓ **Do**: Always set MaxRecords on aggregates; use Pagination block for large lists

❌ **Don't**: Bind directly to aggregate fields without intermediate variables (makes state tracking complex)
✓ **Do**: Create screen variables for filter state; refresh aggregates on variable change

❌ **Don't**: Build complex conditionals in binding expressions
✓ **Do**: Create local variables for computed values; bind to those instead

❌ **Don't**: Skip accessibility patterns (labels, alt text, aria-labels)
✓ **Do**: Always use semantic HTML; associate labels with inputs; add alt text to images

❌ **Don't**: Use custom CSS for responsive layouts
✓ **Do**: Use OutSystemsUI Columns blocks; they handle all breakpoint logic

## When Patterns Don't Fit

- **Complex SQL queries** → Use DataAction with SQL node instead of aggregate
- **Offline data** → Use Local entities with client-side aggregates (CreateScreenAggregate(true, ...))
- **Multi-source workflows** → Combine multiple aggregates + screen variables for state orchestration
- **Custom animations** → Extend Animate block with custom CSS classes via SetStyle()

## References (API-verified — use for actual code)

- **[osui-verified-blocks.md](osui-verified-blocks.md)** — API-correct OutSystemsUI block catalog, per-archetype composition recipes, and design tokens with exact values.
- **[reactive-widget-api.md](reactive-widget-api.md)** — verified widget-creation API + runner-crash traps + joins / M2M / role-check idioms.
- **[patterns-by-element.md](patterns-by-element.md)** — entity / action / node creation basics.

---

**Updated**: 2026-07-07 | **Coverage**: 26 screens analyzed, 50+ patterns extracted | **Status**: Production-ready
