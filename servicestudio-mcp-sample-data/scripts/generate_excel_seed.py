#!/usr/bin/env python3
"""Generate the workbook AND the applyModelApiCode lambda that seeds server
entities from an .xlsx resource on publish (ExcelToRecordList).

  generate_excel_seed.py spec.json --xlsx SampleData.xlsx > seed.cs   # workbook + full lambda
  generate_excel_seed.py spec.json --xlsx SampleData.xlsx --replace-resource-only > swap.cs

Pure standard library (zipfile): no openpyxl needed. The lambda is a full
`eSpace => { ... }` (no eSpace.Save); pass it as `code` with `imports: []`.

It creates: the resource (workbook bytes embedded as base64), one Excel
structure per sheet, a server action with a guard, one ExcelToRecordList node
per sheet, one For Each per sheet (in sheet order: parents first), and a timer
scheduled WhenPublished.

Spec (JSON):
{
  "action": "BootstrapOrdersFromExcel", "timer": "BootstrapOrdersFromExcel",
  "resource": "SampleOrders.xlsx",
  "description": "...",                                   # optional
  "guard": {"entity": "Order", "filter": "Order.OrderNumber like \\"XL-%\\""},  # filter optional: default = table empty
  "xlsx": "my-edited.xlsx",                               # optional: embed this file instead of generating from "rows"
  "sheets": [
    {"name": "Order", "entity": "Order", "structure": "Excel_Orders",
     "columns": [
        {"name": "OrderNumber", "type": "text", "length": 20},        # -> attribute OrderNumber (same name)
        {"name": "TotalAmount", "type": "decimal"}
     ],
     "rows": [["XL-1001", 300]],
     "set": {"OrderDate": "CurrDateTime()"}},                       # extra attribute := expression
    {"name": "OrderLine", "entity": "OrderLine", "structure": "Excel_OrderLines",
     "columns": [
        {"name": "OrderNumber", "type": "text", "length": 20, "lookup": {"entity": "Order", "by": "OrderNumber", "into": "OrderId"}},
        {"name": "ProductCode", "type": "text", "length": 50, "lookup": {"entity": "Product", "by": "ProductCode", "into": "ProductId"}},
        {"name": "Quantity", "type": "integer"}, {"name": "UnitPrice", "type": "decimal"}],
     "rows": [["XL-1001", "CD628248719581", 3, 60]],
     "set": {"LineTotal": "{row}.Quantity * {row}.UnitPrice"}}
  ]
}
Column rules: a column with "lookup" is NOT copied; it finds the referenced row
by business key and stores its Id in attribute `into`. Every other column is
copied to the attribute of the same name (or "attribute": "Other"). `{row}` in a
"set" expression is the current sheet row (ConvertX.Current.<Structure>).
Types: text (needs length), integer, longinteger, decimal, boolean.
"""
import base64
import io
import json
import os
import re
import sys
import zipfile

N = "OutSystems.Model.Logic.Nodes."
TYPES = {"text": "TextType", "integer": "IntegerType", "longinteger": "LongIntegerType",
         "decimal": "DecimalType", "boolean": "BooleanType"}


def q(s):
    return json.dumps(s)


def ident(s):
    return re.sub(r"\W", "", s)


_XML_ILLEGAL = re.compile("[\x00-\x08\x0b\x0c\x0e-\x1f\ufffe\uffff]")
NAME_RE = re.compile(r"^[A-Za-z_][A-Za-z0-9_]*$")
SHEET_BAD = re.compile(r"[\[\]:*?/\\]")
MAX_XLSX_BYTES = 20 * 1024 * 1024


def escape(s):
    """XML-escape text and attribute values (quotes included); drop characters
    that are not legal in XML 1.0 so a bad cell can never corrupt the package."""
    s = _XML_ILLEGAL.sub("", str(s))
    return (s.replace("&", "&amp;").replace("<", "&lt;").replace(">", "&gt;")
             .replace('"', "&quot;"))


def fail(msg):
    sys.exit("error: " + msg)


def validate(spec):
    """Reject anything that would be interpolated into the generated lambda or
    workbook in a way the generator does not escape."""
    for key in ("action", "timer", "resource", "guard", "sheets"):
        if key not in spec:
            fail(f'spec is missing "{key}"')

    def name(v, what):
        if not isinstance(v, str) or not NAME_RE.match(v):
            fail(f"{what} must be an identifier (letters, digits, underscore): {v!r}")

    name(spec["action"], "action")
    name(spec["timer"], "timer")
    if not re.match(r"^[A-Za-z0-9_. -]{1,80}\.xlsx$", str(spec["resource"])):
        fail('resource must be a plain file name ending in ".xlsx"')
    name(spec["guard"].get("entity"), "guard.entity")
    if not spec["sheets"]:
        fail("sheets must not be empty")
    for s in spec["sheets"]:
        n = s.get("name", "")
        if not isinstance(n, str) or not 1 <= len(n) <= 31 or SHEET_BAD.search(n):
            fail(f"sheet name must be 1-31 characters without [ ] : * ? / \\ : {n!r}")
        name(s.get("entity"), "sheet.entity")
        name(s.get("structure"), "sheet.structure")
        if not s.get("columns"):
            fail(f"sheet {n!r} has no columns")
        for c in s["columns"]:
            name(c.get("name"), "column.name")
            if c.get("type") not in TYPES:
                fail(f"column {c.get('name')!r}: type must be one of {sorted(TYPES)}")
            if c["type"] == "text":
                ln = c.get("length")
                if not isinstance(ln, int) or isinstance(ln, bool) or not 1 <= ln <= 2000:
                    fail(f"column {c['name']!r}: text needs an integer length 1-2000")
            if "attribute" in c:
                name(c["attribute"], "column.attribute")
            lk = c.get("lookup")
            if lk:
                for k in ("entity", "by", "into"):
                    name(lk.get(k), "lookup." + k)
        for row in s.get("rows", []):
            if len(row) != len(s["columns"]):
                fail(f"sheet {n!r}: a row has {len(row)} values for {len(s['columns'])} columns")
        for attr in s.get("set", {}):
            name(attr, "set attribute")


def read_xlsx(path, base_dir):
    """Read a workbook to embed. It must be a .xlsx file inside the spec's own
    directory (no ../ escapes, no symlink out) and at most MAX_XLSX_BYTES."""
    full = os.path.realpath(os.path.join(base_dir, path))
    root = os.path.realpath(base_dir)
    if os.path.commonpath([full, root]) != root:
        fail(f"xlsx must live inside the spec directory: {path}")
    if not full.lower().endswith(".xlsx") or not os.path.isfile(full):
        fail(f"xlsx must be an existing .xlsx file: {path}")
    if os.path.getsize(full) > MAX_XLSX_BYTES:
        fail(f"xlsx is larger than {MAX_XLSX_BYTES // (1024 * 1024)} MB: {path}")
    with open(full, "rb") as fh:
        return fh.read()


# ---------------------------------------------------------------- xlsx writer
def col_letter(i):
    s = ""
    i += 1
    while i:
        i, r = divmod(i - 1, 26)
        s = chr(65 + r) + s
    return s


def make_xlsx(sheets):
    """sheets: list of (name, header[list], rows[list of list]) -> bytes"""
    strings, index = [], {}

    def sst(v):
        if v not in index:
            index[v] = len(strings)
            strings.append(v)
        return index[v]

    sheet_xml = []
    for name, header, rows in sheets:
        out = ['<?xml version="1.0" encoding="UTF-8" standalone="yes"?>',
               '<worksheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main"><sheetData>']
        for r, row in enumerate([header] + rows, 1):
            out.append(f'<row r="{r}">')
            for c, v in enumerate(row):
                ref = f"{col_letter(c)}{r}"
                if isinstance(v, bool):
                    out.append(f'<c r="{ref}" t="b"><v>{int(v)}</v></c>')
                elif isinstance(v, (int, float)):
                    out.append(f'<c r="{ref}"><v>{v}</v></c>')
                elif v is None:
                    continue
                else:
                    out.append(f'<c r="{ref}" t="s"><v>{sst(str(v))}</v></c>')
            out.append("</row>")
        out.append("</sheetData></worksheet>")
        sheet_xml.append("".join(out))

    n = len(sheets)
    ct = ['<?xml version="1.0" encoding="UTF-8" standalone="yes"?>',
          '<Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types">',
          '<Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/>',
          '<Default Extension="xml" ContentType="application/xml"/>',
          '<Override PartName="/xl/workbook.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml"/>']
    for i in range(1, n + 1):
        ct.append(f'<Override PartName="/xl/worksheets/sheet{i}.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml"/>')
    ct += ['<Override PartName="/xl/sharedStrings.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.sharedStrings+xml"/>',
           '<Override PartName="/xl/styles.xml" ContentType="application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml"/>',
           '</Types>']
    rels = ('<?xml version="1.0" encoding="UTF-8" standalone="yes"?>'
            '<Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">'
            '<Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument" Target="xl/workbook.xml"/>'
            '</Relationships>')
    wb = ['<?xml version="1.0" encoding="UTF-8" standalone="yes"?>',
          '<workbook xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main" '
          'xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships"><sheets>']
    wbrels = ['<?xml version="1.0" encoding="UTF-8" standalone="yes"?>',
              '<Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">']
    for i, (name, _, _) in enumerate(sheets, 1):
        wb.append(f'<sheet name="{escape(name)}" sheetId="{i}" r:id="rId{i}"/>')
        wbrels.append(f'<Relationship Id="rId{i}" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet" Target="worksheets/sheet{i}.xml"/>')
    wb.append("</sheets></workbook>")
    wbrels.append(f'<Relationship Id="rId{n+1}" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/sharedStrings" Target="sharedStrings.xml"/>')
    wbrels.append(f'<Relationship Id="rId{n+2}" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles" Target="styles.xml"/>')
    wbrels.append("</Relationships>")
    sst_xml = ('<?xml version="1.0" encoding="UTF-8" standalone="yes"?>'
               f'<sst xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main" count="{len(strings)}" uniqueCount="{len(strings)}">'
               + "".join(f'<si><t xml:space="preserve">{escape(s)}</t></si>' for s in strings) + "</sst>")
    styles = ('<?xml version="1.0" encoding="UTF-8" standalone="yes"?>'
              '<styleSheet xmlns="http://schemas.openxmlformats.org/spreadsheetml/2006/main">'
              '<fonts count="1"><font><sz val="11"/><name val="Calibri"/></font></fonts>'
              '<fills count="2"><fill><patternFill patternType="none"/></fill><fill><patternFill patternType="gray125"/></fill></fills>'
              '<borders count="1"><border><left/><right/><top/><bottom/><diagonal/></border></borders>'
              '<cellStyleXfs count="1"><xf numFmtId="0" fontId="0" fillId="0" borderId="0"/></cellStyleXfs>'
              '<cellXfs count="1"><xf numFmtId="0" fontId="0" fillId="0" borderId="0" xfId="0"/></cellXfs>'
              '<cellStyles count="1"><cellStyle name="Normal" xfId="0" builtinId="0"/></cellStyles>'
              '</styleSheet>')
    buf = io.BytesIO()
    def put(z, name, data):
        # fixed timestamp: the same spec always produces byte-identical output
        zi = zipfile.ZipInfo(name, date_time=(2026, 1, 1, 0, 0, 0))
        zi.compress_type = zipfile.ZIP_DEFLATED
        z.writestr(zi, data)

    with zipfile.ZipFile(buf, "w") as z:
        put(z, "[Content_Types].xml", "".join(ct))
        put(z, "_rels/.rels", rels)
        put(z, "xl/workbook.xml", "".join(wb))
        put(z, "xl/_rels/workbook.xml.rels", "".join(wbrels))
        for i, x in enumerate(sheet_xml, 1):
            put(z, f"xl/worksheets/sheet{i}.xml", x)
        put(z, "xl/sharedStrings.xml", sst_xml)
        put(z, "xl/styles.xml", styles)
    return buf.getvalue()


# ------------------------------------------------------------------ lambda
def gen(spec, xlsx_bytes, replace_only=False):
    res = spec["resource"]
    b64 = base64.b64encode(xlsx_bytes).decode()
    if replace_only:
        return ("eSpace => {\n"
                f'    eSpace.Resources.Named({q(res)}).Delete();\n'
                f'    eSpace.CreateResource(Convert.FromBase64String("{b64}"), {q(res)});\n'
                "}\n")
    res_expr = "Resources." + re.sub(r"\W", "_", res) + ".Content"
    o = []
    a = o.append
    a("eSpace => {")
    entities = []
    for s in spec["sheets"]:
        for e in [s["entity"]] + [c["lookup"]["entity"] for c in s["columns"] if c.get("lookup")]:
            if e not in entities:
                entities.append(e)
    if spec["guard"]["entity"] not in entities:
        entities.append(spec["guard"]["entity"])
    for e in entities:
        a(f'    var ent_{ident(e)} = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named({q(e)});')
    a(f'    eSpace.CreateResource(Convert.FromBase64String("{b64}"), {q(res)});')
    for s in spec["sheets"]:
        v = f'st_{ident(s["structure"])}'
        a(f'    var {v} = eSpace.CreateStructure({q(s["structure"])});')
        for c in s["columns"]:
            a(f'    var {v}_{ident(c["name"])} = {v}.CreateAttribute({q(c["name"])}); {v}_{ident(c["name"])}.DataType = eSpace.{TYPES[c["type"]]};'
              + (f' {v}_{ident(c["name"])}.Length = {int(c["length"])};' if c["type"] == "text" else ""))
    a(f'    var act = eSpace.CreateServerAction({q(spec["action"])});')
    if spec.get("description"):
        a(f'    act.Description = {q(spec["description"])};')
    for s in spec["sheets"]:
        e = ident(s["entity"])
        a(f'    var rec_{e} = act.CreateLocalVariable("{e}Record"); rec_{e}.DataType = ent_{e};')
    a(f"    var start = act.CreateNode<{N}IStartNode>();")
    g = spec["guard"]
    gname = f"Existing{ident(g['entity'])}"
    a(f'    var guardAgg = act.CreateNode<{N}IAggregateNode>("{gname}").ConnectedBelow(start);')
    a(f"    guardAgg.AsDatabaseAggregate.CreateSource(ent_{ident(g['entity'])});")
    if g.get("filter"):
        a(f'    guardAgg.AsDatabaseAggregate.CreateFilter({q(g["filter"])});')
    a(f"    var guardIf = act.CreateNode<{N}IIfNode>().ConnectedBelow(guardAgg);")
    a(f'    guardIf.SetCondition("{gname}.List.Empty"); guardIf.Label = "Not seeded yet?";')
    a(f"    var guardEnd = act.CreateNode<{N}IEndNode>().ToTheRightOf(guardIf);")
    a("    guardIf.FalseTarget = guardEnd;")
    prev = "guardIf"
    for i, s in enumerate(spec["sheets"], 1):
        cv = f"conv{i}"
        st = ident(s["structure"])
        nm = "Convert" + ident(s["name"])
        if i == 1:
            a(f'    var {cv} = act.CreateNode<{N}IExcelToRecordListNode>("{nm}").Below(guardIf);')
            a(f"    guardIf.TrueTarget = {cv};")
        else:
            a(f'    var {cv} = act.CreateNode<{N}IExcelToRecordListNode>("{nm}").ConnectedBelow({prev});')
        a(f'    {cv}.DataType = eSpace.GetOrCreateListType(eSpace.GetOrCreateAnonymousStructure(Tuple.Create<string, OutSystems.Model.Types.ITypeSignature>({q(s["structure"])}, st_{st})));')
        a(f'    {cv}.SetFileContent({q(res_expr)});')
        a(f'    {cv}.SetSheetName({q(chr(34) + s["name"] + chr(34))});')
        prev = cv
    prevfe = None
    for i, s in enumerate(spec["sheets"], 1):
        e = ident(s["entity"])
        st = ident(s["structure"])
        cvname = f'Convert{ident(s["name"])}'
        fe = f"fe{i}"
        if prevfe is None:
            a(f"    var {fe} = act.CreateNode<{N}IForEachNode>().ConnectedBelow({prev});")
        else:
            a(f"    var {fe} = act.CreateNode<{N}IForEachNode>().Below({prevfe});")
            a(f"    {prevfe}.Target = {fe};")
        a(f'    {fe}.SetRecordList("{cvname}");')
        row = f"{cvname}.Current.{st}"
        first = None
        last = None

        def chain(var, create):
            nonlocal first, last
            if last is None:
                a(f"    var {var} = {create}.ToTheRightOf({fe});")
                first = var
            else:
                a(f"    var {var} = {create}.ConnectedBelow({last});")
            last = var

        for c in s["columns"]:
            lk = c.get("lookup")
            if not lk:
                continue
            le = ident(lk["entity"])
            var = f"find{i}_{ident(c['name'])}"
            chain(var, f'act.CreateNode<{N}IAggregateNode>("Find{le}For{ident(s["name"])}{ident(c["name"])}")')
            a(f"    {var}.AsDatabaseAggregate.CreateSource(ent_{le});")
            flt = lk["entity"] + "." + lk["by"] + " = " + row + "." + c["name"]
            a("    " + var + ".AsDatabaseAggregate.CreateFilter(" + q(flt) + ");")
        asg = f"asg{i}"
        chain(asg, f'act.CreateNode<{N}IAssignNode>("Prepare{ident(s["name"])}")')
        for c in s["columns"]:
            lk = c.get("lookup")
            if lk:
                le = ident(lk["entity"])
                target = e + "Record." + lk["into"]
                src = "Find" + le + "For" + ident(s["name"]) + ident(c["name"]) + ".List.Current." + lk["entity"] + ".Id"
            else:
                target = e + "Record." + c.get("attribute", c["name"])
                src = row + "." + c["name"]
            a("    " + asg + ".CreateAssignment(" + q(target) + ", " + q(src) + ");")
        for attr, expr in s.get("set", {}).items():
            a("    " + asg + ".CreateAssignment(" + q(e + "Record." + attr) + ", " + q(expr.replace("{row}", row)) + ");")
        ex = f"ex{i}"
        chain(ex, f'act.CreateNode<{N}IExecuteServerActionNode>("Create{e}")')
        a(f"    {ex}.Action = ent_{e}.CreateAction;")
        a(f'    {ex}.SetArgumentValue(ent_{e}.CreateAction.InputParameters.Named("Source"), "{e}Record");')
        a(f"    {fe}.CycleTarget = {first};")
        a(f"    {ex}.Target = {fe};")
        prevfe = fe
    a(f"    var okEnd = act.CreateNode<{N}IEndNode>().Below({prevfe});")
    a(f"    {prevfe}.Target = okEnd;")
    a("")
    a(f'    var timer = eSpace.CreateTimer({q(spec["timer"])});')
    a("    timer.Action = act;")
    a('    timer.Schedule = "WhenPublished";')
    a("    timer.TimeoutInMinutes = 20;")
    a(f'    timer.Description = {q(spec.get("description", "Seeds sample data from " + res + " on publish"))};')
    a("}")
    return "\n".join(o) + "\n"


def build_xlsx(spec, base_dir):
    if spec.get("xlsx"):
        return read_xlsx(spec["xlsx"], base_dir)
    sheets = [(s["name"], [c["name"] for c in s["columns"]], s["rows"]) for s in spec["sheets"]]
    return make_xlsx(sheets)


if __name__ == "__main__":
    args = sys.argv[1:]
    if not args or args[0].startswith("-"):
        sys.exit(__doc__)
    spec_path = args[0]
    out_xlsx = None
    replace_only = "--replace-resource-only" in args
    if "--xlsx" in args:
        k = args.index("--xlsx") + 1
        if k >= len(args):
            fail("--xlsx needs a file name")
        out_xlsx = args[k]
        if not out_xlsx.lower().endswith(".xlsx"):
            fail("--xlsx output must end in .xlsx")
    with open(spec_path, encoding="utf-8") as fh:
        spec = json.load(fh)
    validate(spec)
    data = build_xlsx(spec, os.path.dirname(os.path.abspath(spec_path)))
    if out_xlsx:
        with open(out_xlsx, "wb") as fh:
            fh.write(data)
    sys.stdout.write(gen(spec, data, replace_only))
