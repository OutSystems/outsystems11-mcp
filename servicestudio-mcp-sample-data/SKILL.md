---
name: servicestudio-mcp-sample-data
description: >-
  Seeds an O11 module with realistic sample rows via the Service Studio MCP
  `applyModelApiCode` tool: a `Bootstrap<X>` server action guarded by "the
  table is still empty", plus a timer scheduled `WhenPublished`, so a fresh
  publish fills an empty database and later publishes leave existing data
  alone. Handles parent/child rows (foreign key stamped from the parent's new
  id) and real foreign keys into existing entities (looked up by index, not
  hard-coded ids), and ships a generator script that turns a small JSON spec
  into the full lambda. Captures the gotchas hit building it (`List[i]` not
  `List.Get(i)`, the guard branch needing its own End node, the timer running
  server-side so you must verify through a read endpoint). Use whenever the
  user asks to "add sample data", "seed the app", "add demo data", "populate
  the database", "create test orders", or similar, in an O11 module open in
  Service Studio. O11 counterpart of the ODC skill `outsystems-sample-data-json`
  (which drives Mentor and JSON Resources and does not apply here). Companion to
  `servicestudio-mcp-oml` — depends on it for the full-lambda contract, tool
  names and session-pointer mechanics; load that skill first if you haven't
  already.
license: MIT
compatibility: Requires the `servicestudio` MCP server connected to a running Service Studio build with an O11 module open, and the `servicestudio-mcp-oml` skill (same `applyModelApiCode` contract, same `eSpaceName`/`sessionToken` conventions — this skill does not restate them). The generator needs Python 3 and nothing else.
metadata:
  author: arjanwaard
  origin: distilled from a live session on Service Studio 11.55.92 (2026-10-13) seeding module "aademo" with three orders and six order lines that reference existing products. The hand-written equivalent of examples/AddSeedOrders.cs was merged, published and read back over HTTP; the generator's output was applied and read back at model level.
---

# Seed an O11 module with sample data

Companion to **`servicestudio-mcp-oml`** — read it first for the full-lambda
contract (`eSpace => { ... }`, no `eSpace.Save`), the tool catalogue and the
silent-no-op traps. The timer example there (`examples/AddTimer.cs`) only wires
a timer to an action that already exists; this skill is the part it doesn't
cover: **the seed action itself, its guard, and the foreign-key handling.**

## Contents

- [§ 1 — The recipe](#1-the-recipe)
- [§ 2 — The generator](#2-the-generator)
- [§ 3 — Gotchas](#3-gotchas)
- [§ 4 — Verify it worked](#4-verify-it-worked)
- [§ 5 — Advice](#5-advice)

## 1. The recipe

One server action and one timer, both created in a single `applyModelApiCode`
call (`imports: []`):

```
Start → Aggregate Existing<Entity> → If <Existing>.List.Empty
          ├─ false → End                         (data already there: do nothing)
          └─ true  → [Aggregate Get<Lookup> → If .List.Length >= N  (else End)]*
                     → Assign <Entity>Record.<attr> := <expr> ...
                     → Execute <Entity>.CreateAction (Source = <Entity>Record)
                     → Assign New<Entity>Id := Create<Entity>.Id       (parents only)
                     → Assign <Child>Record.<fk> := New<Entity>Id, ...
                     → Execute <Child>.CreateAction                    (repeat per row)
                     → End
Timer <name>: Action = Bootstrap<X>, Schedule = "WhenPublished", TimeoutInMinutes = 20
```

- **Guard.** Look at one table (the parent) and run the seed only when it is
  empty. This is what makes re-publishing safe; it is the same pattern the
  Excel-based `BootstrapProducts` action in the starter template uses.
- **Foreign keys into existing data.** Add a lookup aggregate and reference rows
  by index, for example `GetProducts.List[0].Product.Id`. Don't hard-code ids:
  they differ between environments. Add a minimum-rows check so the action ends
  cleanly instead of failing when the lookup table is empty.
- **Parent/child.** Create the parent, store `Create<Parent>.Id` in a local
  identifier variable, and stamp it on every child's foreign key before its
  create. Create all of a parent's children before the next parent.
- **Computed fields.** Compute them in the assignment expression from the same
  source values (`LineTotal := 2 * GetProducts.List[0].Product.Price`, and the
  parent total as the matching sum) so rollups agree.

Full lambda: [`examples/AddSeedOrders.cs`](examples/AddSeedOrders.cs), generated
from [`examples/orders.spec.json`](examples/orders.spec.json).

## 2. The generator

A seed with a few parents and children is already 100+ lines of C#. Generate it:

```bash
python3 scripts/generate_seed.py my.spec.json > seed.cs
```

Spec fields: `action`, `timer`, `description`; `guard.entity`; `lookups[]`
(`aggregate`, `entity`, `minRows`); `records[]` (`entity`, `set`, optional
`children[]` with `entity`, `fk`, `set`). Values in `set` are **OutSystems
expressions**: text needs its own quotes (`"\"SO-1001\""`), dates use
`CurrDateTime()`, list items use `List[i]`. Set every attribute you care about
on every record — the local record variable is reused, so a skipped attribute
keeps the previous record's value. Pass the generated text as `code`.

## 3. Gotchas

1. **`List[i]`, not `List.Get(i)`.** The latter saves but is invalid:
   `ParserUnexpectedElement: Syntax error caused by unexpected '(' element in
   expression`, once per assignment that uses it.
2. **A saved-but-invalid chain is not mergeable.** `validationMessages` with
   `type: "Error"` means call `omlReset` (just `eSpaceName`), fix, re-apply.
3. **The guard's false branch needs its own End node.** Same rule as exception
   handlers; the generator does this for you.
4. **Check the entity's action signatures before you wire them.** The recipe
   uses `entity.CreateAction` with its `Source` input and `Id` output, which is
   what the live run confirmed for module entities (the REST skill's § 2 query
   prints them for any entity).
5. **Create the parent before its children.** The child's foreign key needs the
   new parent id, so each child's Assign must come after the parent's create
   and the id store. (Behaviour for a mandatory FK created out of order was not
   tested.)
6. **Don't count on seeing the data in Service Studio.** The timer runs on the
   server after publish; the Model API can't read rows. See § 4.

## 4. Verify it worked

Apply, then read the chain back before merging:

1. `applyModelApiCode` with the generated lambda. Expect an empty
   `exceptionMessage` and no `type: "Error"` entries.
2. `getServerAction` for the new action and check the node count and the two
   guard Ifs. (For a rehearsal, stop here and `omlReset`: nothing touches the
   real module until `omlMerge`.)
3. `omlMerge` with the last clean `mutatedOmlPath`, then `omlPublish`. The
   `WhenPublished` timer runs as part of publishing.
4. Read the rows over HTTP. Add a **temporary** read-only REST API for the
   tables (the List recipe in `servicestudio-mcp-rest-api`), publish, `curl` it,
   then delete it and publish again so it doesn't stay exposed. In the live run
   this showed 3 orders and 6 lines with every order total equal to the sum of
   its line totals.
5. Republish and confirm the counts did not change (the guard). Note a publish
   with no model changes reports `Upload Skipped`; a changed model is a real
   re-run.

## 5. Advice

- **Make the seed deterministic and small.** A handful of rows with realistic
  names covers screens and lists; anything bigger belongs in a data import.
- **Keep demo data identifiable** (`SO-1001`, a demo customer) so it is easy to
  delete or filter out later.
- **Guard on the table you create into, not on a flag.** There is nothing to
  keep in sync, and an emptied table reseeds on the next publish — which is
  usually what you want, and worth knowing before you publish to a shared
  environment.
- **Not for production.** A `WhenPublished` seed runs on every environment the
  module is published to; remove or disable the timer before promoting.
- **Other seed sources are untested here.** JSON resources and Excel imports
  (the ODC skill's approach, and the template's `BootstrapProducts`) were not
  attempted through the Model API; this skill covers inline rows only.
