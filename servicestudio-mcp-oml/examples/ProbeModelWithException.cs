// Read-only discovery probe: collect facts about the module into a string and return them through exceptionMessage (nothing is saved and the session pointer does not move)
// The MCP code sandbox rejects Console, System.*-qualified names and reflection, so throwing is the output channel. validationMessages still come back. Verified live 2026-09-30.

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var s = "";
    var screens = eSpace.MobileFlows.SelectMany(f => f.Nodes).OfType<IMobileScreen>().ToList();
    s += "screens=" + string.Join(",", screens.Select(x => x.Name + (x.Roles.Contains(eSpace.AnonymousRole) ? "(public)" : "[" + string.Join("|", x.Roles.Select(r => r.Name)) + "]")));
    var screen = screens.First();
    foreach (var layout in screen.Widgets.OfType<IMobileBlockInstanceWidget>())
        s += " ; layout " + layout.SourceBlock.Name + " placeholders=" + string.Join(",", layout.PlaceholdersContent.Select(p => p.Placeholder.Name));
    s += " ; entities=" + string.Join(",", eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Select(e => e.Name));
    s += " ; references=" + string.Join(",", eSpace.References.Select(r => r.Name));
    throw new Exception("PROBE:" + s);
}
