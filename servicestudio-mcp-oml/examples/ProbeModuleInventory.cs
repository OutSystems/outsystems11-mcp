// Summarise a whole module in ONE read-only call: entities (with attribute types), structures, screens per UI flow (with roles), server actions (with folders), client actions, roles, integrations and references
// Context: works on Reactive and Traditional modules; nothing is saved (throw-probe). Verified live 2026-10-17: OrderManagement (17 entities, 15 screens, 43 server actions) came back as ~5.6k characters, against ~113k for getDataModel; the TrueChange messages come back in the same result. Drill into single elements with the get* tools afterwards.

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    // Type names print as "Text (BasicTypes.Text:key)"; keep the part before " (".
    string T(object t) { var s = "" + t; var i = s.IndexOf(" ("); return i > 0 ? s.Substring(0, i) : s; }
    string Roles(IEnumerable<OutSystems.Model.Logic.IRoleSignature> roles) => "{" + string.Join("/", roles.Select(r => r.Name)) + "}";

    var lines = new List<string>();
    lines.Add("ENTITIES: " + string.Join("; ", eSpace.Entities.Select(e =>
        e.Name + "(" + string.Join(",", e.Attributes.Select(a => a.Name + ":" + T(a.DataType))) + ")")));
    lines.Add("STRUCTURES: " + string.Join("; ", eSpace.Structures.Select(s =>
        s.Name + "(" + string.Join(",", s.Attributes.Select(a => a.Name + ":" + T(a.DataType))) + ")")));
    // Reactive / Mobile flows
    lines.Add("SCREENS (reactive): " + string.Join("; ", eSpace.MobileFlows.Select(f => f.Name + "=[" + string.Join(",",
        f.Nodes.OfType<OutSystems.Model.UI.Mobile.IMobileScreen>().Select(x => x.Name + Roles(x.Roles))) + "]")));
    // Traditional Web flows
    lines.Add("SCREENS (traditional): " + string.Join("; ", eSpace.WebFlows.Select(f => f.Name + "=[" + string.Join(",",
        f.Nodes.OfType<OutSystems.Model.UI.Web.IWebScreen>().Select(x => x.Name + Roles(x.Roles))) + "]")));
    lines.Add("SERVER ACTIONS: " + string.Join(", ", eSpace.ServerActions.Select(a => a.Name + (a.Folder == null ? "" : "@" + a.Folder.Name))));
    lines.Add("CLIENT ACTIONS: " + string.Join(", ", eSpace.ClientActions.Select(a => a.Name)));
    lines.Add("ROLES: " + string.Join(", ", eSpace.Roles.Select(r => r.Name)));
    lines.Add("INTEGRATIONS: " + string.Join(", ", eSpace.Integrations.Select(i => i.Name + ":" + i.GetType().Name)));
    lines.Add("REFERENCES: " + string.Join(", ", eSpace.References.Select(r => r.Name)));

    throw new Exception("PROBE:" + string.Join("\n", lines));
}
