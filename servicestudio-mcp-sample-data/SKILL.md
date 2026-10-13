---
name: servicestudio-mcp-sample-data
description: >-
  Seeds an O11 module with realistic sample rows via the Service Studio MCP
  `applyModelApiCode` tool. Primary mode: an .xlsx workbook embedded in the
  module as a resource and read with `ExcelToRecordList` by a `Bootstrap<X>`
  server action that is guarded ("not seeded yet") and run by a timer
  scheduled `WhenPublished`, so a fresh publish fills an empty database and
  later publishes leave existing data alone. Rows reference each other and
  existing entities by business key (order number, product code), not
  hard-coded ids. Ships a generator that writes the workbook (pure standard
  library) and the full lambda from one JSON spec, plus a second, inline mode
  (Assign + Create per row, no file) for data derived from existing rows.
  Captures the gotchas hit building it (resource created from base64 bytes,
  `Resources.<Name_xlsx>.Content`, parents-first sheet order, `List[i]` not
  `List.Get(i)`, the timer running server-side so you verify through a read
  endpoint). Use whenever the user asks to "add sample data", "seed the app",
  "add demo data", "populate the database", "load data from Excel", or
  similar, in an O11 module open in Service Studio. O11 counterpart of the
  ODC skill `outsystems-sample-data-json` (which drives Mentor and JSON
  Resources and does not apply here). Companion to `servicestudio-mcp-oml` —
  depends on it for the full-lambda contract, tool names and session-pointer
  mechanics; load that skill first if you haven't already.
license: MIT
compatibility: Requires the `servicestudio` MCP server connected to a running Service Studio build with an O11 module open, and the `servicestudio-mcp-oml` skill (same `applyModelApiCode` contract, same `eSpaceName`/`sessionToken` conventions — this skill does not restate them). The generators need Python 3 and nothing else.
metadata:
  author: arjanwaard
  origin: distilled from live sessions on Service Studio 11.55.92 (2026-10-13) seeding module "aademo" with orders and order lines that reference existing products. Excel mode: a workbook written with openpyxl and one written by scripts/generate_excel_seed.py were both merged, published and read back over HTTP. Inline mode: the hand-written equivalent was published and read back; the generator output was applied and read back at model level.
---

# Seed an O11 module with sample data

Companion to **`servicestudio-mcp-oml`** — read it first for the full-lambda
contract (`eSpace => { ... }`, no `eSpace.Save`), the tool catalogue and the
silent-no-op traps. Its `examples/AddTimer.cs` only wires a timer to an action
that already exists; this skill is the part it doesn't cover: **the seed action,
its guard, the data source, and the foreign-key handling.**

## Contents

- [§ 1 — Which mode](#1-which-mode)
- [§ 2 — Excel mode (primary)](#2-excel-mode-primary)
- [§ 3 — The Excel generator](#3-the-excel-generator)
- [§ 4 — Inline mode](#4-inline-mode)
- [§ 5 — Gotchas](#5-gotchas)
- [§ 6 — Verify it worked](#6-verify-it-worked)
- [§ 7 — Advice](#7-advice)

## 1. Which mode

| | Excel mode (default) | Inline mode |
|---|---|---|
| Data lives in | an `.xlsx` resource you can open and edit | assignments in the lambda |
| Cross-references | by business key (`OrderNumber`, `ProductCode`) via lookups | by list index of a lookup aggregate (`GetProducts.List[0]`) |
| Flow size | constant — independent of row count | grows with every row (153 lines for 3 orders) |
| Use when | you have or want a data file, more than a handful of rows, or rows that need to read well | data must be computed from rows already in the database and there is no natural business key |

Start with Excel mode. Fall back to inline only for the case in the last row.

## 2. Excel mode (primary)

One resource, one structure per sheet, one server action, one timer, all created
in a single `applyModelApiCode` call (`imports: []`):

```
Resource  <Name>.xlsx           eSpace.CreateResource(Convert.FromBase64String("<base64>"), "<Name>.xlsx")
Structure Excel_<Sheet>         attributes = the sheet's column headers (same names), typed
Action    Bootstrap<X>
  Start → Aggregate Existing<Entity> [filter] → If .List.Empty
            ├─ false → End                            (already seeded: do nothing)
            └─ true  → ExcelToRecordList Convert<Sheet1>  (FileContent = Resources.<Name_xlsx>.Content,
                         → ExcelToRecordList Convert<Sheet2>   SheetName = "<Sheet>", DataType = list of Excel_<Sheet>)
                         → For Each Convert<Sheet1>: Assign <Entity>Record from .Current.Excel_<Sheet1> → Create
                         → For Each Convert<Sheet2>: [Aggregate Find<Parent> by business key]*
                                                      → Assign … (FK := Find<Parent>.List.Current.<Parent>.Id) → Create
                         → End
Timer     Bootstrap<X>  Action, Schedule = "WhenPublished", TimeoutInMinutes = 20
```

- **Resource.** `CreateResource(byte[], name)` embeds the file in the module; the
  sandbox accepts `Convert.FromBase64String` (a 3–6 KB workbook is 4–8k base64
  characters). Reference it as `Resources.<name with . replaced by _>.Content`
  (`SampleOrders.xlsx` → `Resources.SampleOrders_xlsx.Content`).
- **Structures.** Attribute names must equal the header cells. Use the matching
  type (text with a length, integer, decimal, …); the node's `DataType` is
  `GetOrCreateListType(GetOrCreateAnonymousStructure(Tuple.Create<string,
  ITypeSignature>("Excel_<Sheet>", structure)))`, and rows are then read as
  `Convert<Sheet>.Current.Excel_<Sheet>.<Column>`.
- **Sheets are read one node per sheet.** Convert all sheets first, then loop.
  Process **parents before children** (sheet order = creation order).
- **Lookups by business key.** Inside the child loop, one aggregate per
  reference (`Order.OrderNumber = ConvertLines.Current.Excel_OrderLines.OrderNumber`)
  and assign `FindOrder.List.Current.Order.Id` to the foreign key. This is what
  makes the file readable and independent of generated ids.
- **Computed fields** go in the assignment (`LineTotal :=
  Quantity * UnitPrice`); keep any denormalised total in the file consistent
  with its lines.
- **Guard.** "Table empty" for a fresh table, or a filter on a marker (`Order.OrderNumber like
  "XL-%"`) so the seed coexists with other rows. Either way a re-publish is a
  no-op.

Full lambda: [`examples/AddExcelSeedOrders.cs`](examples/AddExcelSeedOrders.cs),
generated from [`examples/orders.excel.spec.json`](examples/orders.excel.spec.json);
the workbook it embeds is [`examples/SampleOrders.xlsx`](examples/SampleOrders.xlsx).

## 3. The Excel generator

```bash
python3 scripts/generate_excel_seed.py my.spec.json --xlsx SampleData.xlsx > seed.cs
```

It writes the workbook (pure standard library — no openpyxl) **and** prints the
lambda that embeds it. Spec: `action`, `timer`, `resource`, `description`,
`guard` (`entity`, optional `filter`), and `sheets[]` with `name`, `entity`,
`structure`, `columns[]` (`name`, `type`, `length`, optional `lookup` `{entity,
by, into}`, optional `attribute`), `rows[]`, and `set` (extra `attribute :=
expression`; `{row}` is the current sheet row). A column with `lookup` is not
copied: it finds the referenced row by business key and stores its `Id`
in `into`. Types: `text`, `integer`, `longinteger`, `decimal`, `boolean`.

To change the data later: edit the generated `.xlsx` in Excel, set `"xlsx":
"edited.xlsx"` in the spec (rows are then ignored), and run

```bash
python3 scripts/generate_excel_seed.py my.spec.json --replace-resource-only > swap.cs
```

which emits only `Resources.Named(...).Delete()` + `CreateResource(...)` — a
resource has no content setter, so replacing is delete-and-recreate under the
same name. (The action's `Resources.…Content` expression kept resolving in an
in-memory apply; it was not published.) A replaced file does **not** reseed on
its own: the guard still sees the old rows, so clear them or change the marker.

## 4. Inline mode

No file: one `Assign` per row plus an `Execute <Entity>.CreateAction`, guarded
the same way, with foreign keys into existing data taken by index from a lookup
aggregate (`GetProducts.List[0].Product.Id`) and a minimum-rows check so the
action ends cleanly when the lookup table is short.

```bash
python3 scripts/generate_inline_seed.py my.spec.json > seed.cs
```

Spec and a worked example: [`examples/orders.inline.spec.json`](examples/orders.inline.spec.json) →
[`examples/AddInlineSeedOrders.cs`](examples/AddInlineSeedOrders.cs). The script's
docstring lists the spec fields. Set every attribute on every record: the local
record variable is reused, so a skipped attribute keeps the previous record's
value.

## 5. Gotchas

1. **"Unused" warnings before the action exists are expected.** Applied in two
   steps, the resource and structures report `UnusedElement` / `UnusedResource`
   until the action referencing them is added. One call (the generator's
   output) is clean.
2. **`List[i]`, not `List.Get(i)`** (inline mode). The latter saves but is
   invalid: `ParserUnexpectedElement: Syntax error caused by unexpected '('
   element in expression`, once per assignment.
3. **A saved-but-invalid chain is not mergeable.** `validationMessages` with
   `type: "Error"` → `omlReset` (just `eSpaceName`), fix, re-apply.
4. **Guard and loop wiring.** The guard's false branch needs its own End node;
   a For Each needs `CycleTarget = <first body node>` and the last body node's
   `Target = <the For Each>`; the node after the loop hangs off the For Each's
   own `Target`. The generators do all of this.
5. **Column ↔ attribute names must match exactly** (case included), and sheet
   names are matched by `SetSheetName("\"Name\"")`.
6. **Create parents before children**, in sheet order. (Behaviour for a
   mandatory FK created out of order was not tested.)
7. **Dates and booleans in Excel cells were not tested.** Date columns are
   better filled by expression (`CurrDateTime()`, `AddDays(CurrDateTime(), -3)`
   in `set`), which is what the examples do.
8. **Don't count on seeing the data in Service Studio.** The timer runs on the
   server after publish and the Model API can't read rows. See § 6.

## 6. Verify it worked

1. Apply the generated lambda. Expect an empty `exceptionMessage` and no
   `type: "Error"` entries (the only messages should be your module's existing
   warnings). Read back with `getServerAction`. For a rehearsal stop here and
   `omlReset` — nothing touches the real module until `omlMerge`.
2. `omlMerge` with the last clean `mutatedOmlPath`, then `omlPublish`. The
   `WhenPublished` timer runs as part of publishing.
3. Read the rows over HTTP. Add a **temporary** read-only REST API for the
   tables (the List recipe in `servicestudio-mcp-rest-api`), publish, `curl` it.
   Check counts, foreign keys, and that parent totals equal the sum of their
   lines.
4. Force a **real** re-run and confirm the counts don't change: a publish with
   no model change reports `Upload Skipped`, so change something trivial (a timer
   description) first.
5. Delete the temporary API and publish again so it doesn't stay exposed. To
   remove test rows, give the temporary API a `DELETE` that calls the entity's
   `DeleteAction` (deleting a parent cascades to children with the `Delete`
   rule).

## 7. Advice

- **Make the seed deterministic and small.** A handful of rows with realistic
  names covers screens and lists; larger data belongs in a proper import.
- **Mark demo data** (`XL-1001`, a demo customer) so it can be found, filtered
  and deleted later, and so a marker guard can keep the seed re-runnable.
- **Keep the workbook next to the spec.** The embedded copy is the only copy in
  the module; the `.xlsx` you edit in Excel is the source of truth.
- **Not for production.** A `WhenPublished` seed runs in every environment the
  module is published to; remove or disable the timer before promoting.
- **Two seeds, two guards.** If a module already seeds a table (the template's
  `BootstrapProducts`, or an inline seed), a second seed on the same entity
  needs a marker filter, or both will insert.
