// Generator for docs/OutSystems.Model.UI.Web.Generated.cs and
// docs/OutSystems.Model.UI.Web.Widgets.Generated.cs
//
// HOW TO RUN: paste the body below (everything between the BEGIN/END markers) into the
// `code` argument of applyModelApiCode, wrapped in a full `eSpace => { ... }` lambda (no
// eSpace.Save), with any OutSystems module open in Service Studio. The body never touches
// `eSpace` (it only reflects the loaded Model assembly and writes files), so the run is a
// deliberate no-op against the OML — do NOT omlMerge it.
//
// See README.md in this folder for the method and rationale.

// ===== BEGIN generator body (wrap in eSpace => { ... }) =====
var asm = typeof(IWebScreen).Assembly;
var dir = @"<path-to-this-skill>\docs";   // set to this skill's docs/ folder before running
string currentNs = "";

var summaries = new Dictionary<string,string> {
  ["IPreparation"] = "Traditional data-fetch flow that runs before a Web screen or block renders. Build its contents with CreateNode<...>() (Start / Aggregate / End nodes); this is structurally different from Mobile's screen.CreateScreenAggregate(...).",
  ["IWebScreen.JavaScript"] = "Client-side JavaScript injected verbatim into the rendered Traditional page. NOT auto-escaped: interpolating user-controlled data here yields stored/DOM XSS in the compiled app. Escape with EncodeJavaScript(). See reference/traditional-ui-creation.md.",
  ["IWebScreen.IsFrequentDestination"] = "When true, this screen is a Frequent Destination, which silences the 'Unexpected Link' warning for links that target it. Set it on the destination screen; there is no API to register a flow connector from the link side.",
  ["IExternalSite.IsFrequentDestination"] = "When true, this external site is a Frequent Destination, silencing the 'Unexpected Link' warning for links that target it.",
  ["IWebWidget"] = "Base interface for every Traditional Web widget. CreateWidget<T>(...) requires T : IWebWidget; passing the Reactive ServiceStudio.Plugin.NRWidgets.IContainer fails with CS0311.",
  ["IContainerWidget"] = "Traditional Web container widget. Holds a Widgets sequence and exposes CreateWidget<T> to build the child tree (Text, Expression, Link, Input, ...).",
  ["IListRecordsWidget"] = "Traditional Web ListRecords widget. Bind its data with SetSourceRecordList(...) (an expression string, e.g. \"GetFeedPosts.List\") and provide an empty message via SetEmptyMessage(...)."
};

string Short(string full) {
    switch (full) {
        case "System.Int32": return "int";
        case "System.String": return "string";
        case "System.Boolean": return "bool";
        case "System.Object": return "object";
        case "System.Void": return "void";
        case "System.Double": return "double";
        case "System.Single": return "float";
        case "System.Int64": return "long";
        case "System.Decimal": return "decimal";
        case "System.Char": return "char";
    }
    if (full.StartsWith("System.Collections.Generic.")) return full.Substring("System.Collections.Generic.".Length);
    if (full.StartsWith("System.")) { var rest = full.Substring("System.".Length); if (!rest.Contains(".")) return rest; }
    if (currentNs.Length > 0 && full.StartsWith(currentNs + ".")) { var rest = full.Substring(currentNs.Length + 1); if (!rest.Contains(".")) return rest; }
    return full;
}
string FmtType(Type t) {
    if (t.IsGenericParameter) return t.Name;
    if (t.IsArray) return FmtType(t.GetElementType()) + "[]";
    if (t.IsGenericType) {
        var gtd = t.GetGenericTypeDefinition();
        var def = gtd.FullName ?? gtd.Name;
        var tick = def.IndexOf('`'); if (tick >= 0) def = def.Substring(0, tick);
        def = Short(def);
        return def + "<" + string.Join(", ", t.GetGenericArguments().Select(FmtType)) + ">";
    }
    return Short(t.FullName ?? t.Name);
}
string ParamSig(System.Reflection.MethodInfo m) => string.Join(",", m.GetParameters().Select(p => FmtType(p.ParameterType)));
string FmtMethod(System.Reflection.MethodInfo m, bool isNew) {
    var gen = m.IsGenericMethodDefinition ? "<" + string.Join(", ", m.GetGenericArguments().Select(a => a.Name)) + ">" : "";
    var ps = m.GetParameters().Select(p => {
        var def = "";
        if (p.IsOptional) def = " = " + (p.DefaultValue == null ? "null" : (p.ParameterType == typeof(string) ? "\"" + p.DefaultValue + "\"" : p.DefaultValue.ToString().ToLowerInvariant()));
        return FmtType(p.ParameterType) + " " + p.Name + def;
    });
    var cons = "";
    if (m.IsGenericMethodDefinition) foreach (var ga in m.GetGenericArguments()) { var cs = ga.GetGenericParameterConstraints(); if (cs.Length > 0) cons += " where " + ga.Name + ": " + string.Join(", ", cs.Select(FmtType)); }
    return (isNew ? "new " : "") + FmtType(m.ReturnType) + " " + m.Name + gen + "(" + string.Join(", ", ps) + ")" + cons + ";";
}
void EmitSummary(System.Text.StringBuilder sb, string key) {
    if (summaries.TryGetValue(key, out var s)) { sb.AppendLine("    /// <summary>"); sb.AppendLine("    /// " + s); sb.AppendLine("    /// </summary>"); }
}

Type[] all;
try { all = asm.GetTypes(); } catch (System.Reflection.ReflectionTypeLoadException ex) { all = ex.Types.Where(x => x != null).ToArray(); }
var flags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly;

var specs = new[] {
    new { Ns = "OutSystems.Model.UI.Web", Header = "// Traditional Web UI flows, screens, blocks, themes, emails, and preparation constructs", File = "OutSystems.Model.UI.Web.Generated.cs" },
    new { Ns = "OutSystems.Model.UI.Web.Widgets", Header = "// Traditional Web widget and control definitions (server-rendered, View State based)", File = "OutSystems.Model.UI.Web.Widgets.Generated.cs" }
};

foreach (var spec in specs) {
    currentNs = spec.Ns;
    var sb = new System.Text.StringBuilder();
    sb.AppendLine(spec.Header);
    sb.AppendLine("using System;");
    sb.AppendLine("using System.Collections.Generic;");
    sb.AppendLine();
    sb.AppendLine("namespace " + spec.Ns + ";");
    sb.AppendLine();
    var types = all.Where(t => t != null && t.IsInterface && t.IsPublic && t.Namespace == spec.Ns && !t.Name.EndsWith("Descriptor")).OrderBy(t => t.Name).ToList();
    foreach (var t in types) {
        var bases = t.GetInterfaces();
        var basePropNames = new HashSet<string>(bases.SelectMany(b => b.GetProperties(flags)).Select(p => p.Name));
        var baseMethodSigs = new HashSet<string>(bases.SelectMany(b => b.GetMethods(flags)).Where(x => !x.IsSpecialName).Select(x => x.Name + "(" + ParamSig(x) + ")"));
        var inherited = new HashSet<Type>(bases.SelectMany(i => i.GetInterfaces()));
        var direct = bases.Where(i => !inherited.Contains(i)).Select(FmtType).OrderBy(x => x).ToList();
        var head = t.Name;
        if (t.IsGenericTypeDefinition) head += "<" + string.Join(", ", t.GetGenericArguments().Select(a => a.Name)) + ">";
        if (summaries.ContainsKey(t.Name)) { sb.AppendLine("/// <summary>"); sb.AppendLine("/// " + summaries[t.Name]); sb.AppendLine("/// </summary>"); }
        sb.AppendLine("public interface " + head + (direct.Count > 0 ? " : " + string.Join(", ", direct) : "") + " {");
        foreach (var p in t.GetProperties(flags).Where(p => p.Name != "Descriptor").OrderBy(x => x.Name)) {
            EmitSummary(sb, t.Name + "." + p.Name);
            var isNew = basePropNames.Contains(p.Name);
            var acc = (p.CanRead ? "get; " : "") + (p.CanWrite ? "set; " : "");
            sb.AppendLine("    " + (isNew ? "new " : "") + FmtType(p.PropertyType) + " " + p.Name + " { " + acc + "}");
        }
        foreach (var m in t.GetMethods(flags).Where(x => !x.IsSpecialName).OrderBy(x => x.Name)) {
            EmitSummary(sb, t.Name + "." + m.Name);
            var isNew = baseMethodSigs.Contains(m.Name + "(" + ParamSig(m) + ")");
            sb.AppendLine("    " + FmtMethod(m, isNew));
        }
        sb.AppendLine("}");
        sb.AppendLine();
    }
    System.IO.File.WriteAllText(System.IO.Path.Combine(dir, spec.File), sb.ToString());
    Console.WriteLine(types.Count + " interfaces, " + sb.Length + " chars -> " + spec.File);
}
// ===== END generator body =====
