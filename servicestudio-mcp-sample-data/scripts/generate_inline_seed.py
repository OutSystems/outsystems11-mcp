#!/usr/bin/env python3
"""Generate an applyModelApiCode lambda that seeds server entities on publish.

Usage:  generate_inline_seed.py spec.json > seed.cs

The output is a full `eSpace => { ... }` lambda (no eSpace.Save) that creates
  * a server action <action> with a "tables are empty" guard, optional lookup
    aggregates (with a minimum-rows check), then Assign + Create calls for every
    record and its children (foreign key stamped from the parent's new id), and
  * a timer <timer> scheduled "WhenPublished" that runs the action.
Pass it as `code` with `imports: []`.

Spec (JSON):
{
  "action": "BootstrapOrders", "timer": "BootstrapOrders",
  "description": "...",                       # optional
  "guard":   {"entity": "Order"},             # seed only while this table is empty
  "lookups": [{"aggregate": "GetProducts", "entity": "Product", "minRows": 5}],
  "records": [
    {"entity": "Order",
     "set": {"OrderNumber": "\\"SO-1001\\"", "Status": "\\"Open\\""},   # attr -> OutSystems expression
     "children": [
       {"entity": "OrderLine", "fk": "OrderId",
        "set": {"ProductId": "GetProducts.List[0].Product.Id", "Quantity": "2"}}
     ]}
  ]
}
Attribute values are OutSystems EXPRESSIONS: strings need their own quotes,
list items are indexed with List[i] (not List.Get(i)). Set every attribute you
care about on every record: the local record variable is reused, so an attribute
you skip keeps the value of the previous record of the same entity.
"""
import json
import re
import sys

N = "OutSystems.Model.Logic.Nodes."


def q(s):
    return json.dumps(s)  # valid C# string literal for the expressions used here


def ident(s):
    return re.sub(r"\W", "", s)


def gen(spec):
    out = []
    a = out.append
    entities = []

    def want(e):
        if e not in entities:
            entities.append(e)

    want(spec["guard"]["entity"])
    for lk in spec.get("lookups", []):
        want(lk["entity"])
    parents = set()
    for r in spec["records"]:
        want(r["entity"])
        if r.get("children"):
            parents.add(r["entity"])
        for c in r.get("children", []):
            want(c["entity"])

    a("eSpace => {")
    for e in entities:
        a(f'    var ent_{ident(e)} = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named({q(e)});')
    a(f'    var act = eSpace.CreateServerAction({q(spec["action"])});')
    if spec.get("description"):
        a(f'    act.Description = {q(spec["description"])};')
    used = [e for e in entities if e != spec["guard"]["entity"] or e in parents or True]
    rec_entities = []
    for r in spec["records"]:
        for e in [r["entity"]] + [c["entity"] for c in r.get("children", [])]:
            if e not in rec_entities:
                rec_entities.append(e)
    for e in rec_entities:
        a(f'    var rec_{ident(e)} = act.CreateLocalVariable("{ident(e)}Record"); rec_{ident(e)}.DataType = ent_{ident(e)};')
    for e in sorted(parents):
        a(f'    var id_{ident(e)} = act.CreateLocalVariable("New{ident(e)}Id"); id_{ident(e)}.DataType = ent_{ident(e)}.IdentifierType;')

    a(f"    var start = act.CreateNode<{N}IStartNode>();")
    g = spec["guard"]["entity"]
    gagg = f"Existing{ident(g)}"
    a(f'    var guardAgg = act.CreateNode<{N}IAggregateNode>("{gagg}").ConnectedBelow(start);')
    a(f"    guardAgg.AsDatabaseAggregate.CreateSource(ent_{ident(g)});")
    a(f"    var guardIf = act.CreateNode<{N}IIfNode>().ConnectedBelow(guardAgg);")
    a(f'    guardIf.SetCondition("{gagg}.List.Empty"); guardIf.Label = "Table empty?";')
    a(f"    var guardEnd = act.CreateNode<{N}IEndNode>().ToTheRightOf(guardIf);")
    a("    guardIf.FalseTarget = guardEnd;")

    state = {"prev": "guardIf", "branch": "guardIf"}

    def place(var, create):
        # first node after an If hangs off its TrueTarget; later nodes chain ConnectedBelow
        if state["prev"] in ("guardIf",) or state["prev"].startswith("lkIf"):
            a(f"    var {var} = {create}.Below({state['prev']});")
            a(f"    {state['prev']}.TrueTarget = {var};")
        else:
            a(f"    var {var} = {create}.ConnectedBelow({state['prev']});")
        state["prev"] = var

    for i, lk in enumerate(spec.get("lookups", []), 1):
        agg = ident(lk["aggregate"])
        place(f"lkAgg{i}", f'act.CreateNode<{N}IAggregateNode>("{agg}")')
        a(f"    lkAgg{i}.AsDatabaseAggregate.CreateSource(ent_{ident(lk['entity'])});")
        a(f"    var lkIf{i} = act.CreateNode<{N}IIfNode>().ConnectedBelow(lkAgg{i});")
        a(f'    lkIf{i}.SetCondition("{agg}.List.Length >= {int(lk.get("minRows", 1))}"); lkIf{i}.Label = "Enough {agg} rows?";')
        a(f"    var lkEnd{i} = act.CreateNode<{N}IEndNode>().ToTheRightOf(lkIf{i});")
        a(f"    lkIf{i}.FalseTarget = lkEnd{i};")
        state["prev"] = f"lkIf{i}"

    def create_row(prefix, entity, sets, label):
        e = ident(entity)
        place(f"{prefix}Set", f'act.CreateNode<{N}IAssignNode>("Prepare{label}")')
        for attr, expr in sets.items():
            a(f'    {prefix}Set.CreateAssignment({q(e + "Record." + attr)}, {q(expr)});')
        place(f"{prefix}Create", f'act.CreateNode<{N}IExecuteServerActionNode>("Create{label}")')
        a(f"    {prefix}Create.Action = ent_{e}.CreateAction;")
        a(f'    {prefix}Create.SetArgumentValue(ent_{e}.CreateAction.InputParameters.Named("Source"), "{e}Record");')
        return f"Create{label}"

    for ri, r in enumerate(spec["records"], 1):
        e = ident(r["entity"])
        exec_name = create_row(f"r{ri}", r["entity"], r["set"], f"{e}{ri}")
        kids = r.get("children", [])
        if kids:
            place(f"r{ri}Id", f'act.CreateNode<{N}IAssignNode>("StoreId{e}{ri}")')
            a(f'    r{ri}Id.CreateAssignment("New{e}Id", "{exec_name}.Id");')
        for ci, c in enumerate(kids, 1):
            sets = {c["fk"]: f"New{e}Id"}
            sets.update(c["set"])
            create_row(f"r{ri}c{ci}", c["entity"], sets, f"{ident(c['entity'])}{ri}_{ci}")

    if state["prev"] in ("guardIf",) or state["prev"].startswith("lkIf"):
        a(f"    var okEnd = act.CreateNode<{N}IEndNode>().Below({state['prev']});")
        a(f"    {state['prev']}.TrueTarget = okEnd;")
    else:
        a(f"    var okEnd = act.CreateNode<{N}IEndNode>().ConnectedBelow({state['prev']});")

    a("")
    a(f'    var timer = eSpace.CreateTimer({q(spec["timer"])});')
    a("    timer.Action = act;")
    a('    timer.Schedule = "WhenPublished";')
    a("    timer.TimeoutInMinutes = 20;")
    a(f'    timer.Description = {q(spec.get("description", "Seeds sample data on publish when the table is empty"))};')
    a("}")
    return "\n".join(out) + "\n"


if __name__ == "__main__":
    if len(sys.argv) != 2:
        sys.exit(__doc__)
    sys.stdout.write(gen(json.load(open(sys.argv[1]))))
