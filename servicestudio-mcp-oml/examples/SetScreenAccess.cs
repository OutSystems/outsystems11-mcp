// Create a Manager role, a screen ManagerDashboard visible only to Managers, and a public (anonymous) screen About
// Context: Reactive module with a MainFlow UI flow. Verified live 2026-09-30 on the LIVE module after omlMerge (public screen reads Anonymous|Registered|…, gated screen reads Manager).

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var manager = eSpace.CreateRole("Manager");
    var mainFlow = eSpace.MobileFlows.Named("MainFlow");

    // A NEW screen starts with Registered + EVERY app role in screen.Roles.
    // Role-gate it by removing Registered (and any other role that must not see it).
    var dashboard = mainFlow.CreateScreen("ManagerDashboard");
    dashboard.HorizontalPosition = 3400;
    dashboard.VerticalPosition = 700;
    dashboard.Roles.Remove(eSpace.RegisteredRole);
    if (!dashboard.Roles.Contains(manager)) dashboard.Roles.Add(manager);

    // Public screen: add the Anonymous system role (Service Studio's "Anonymous" checkbox).
    // Do NOT use about.AnonymousAccess = true: omlMerge drops it and the live screen stays
    // authenticated-only. Service Studio keeps Registered alongside Anonymous.
    var about = mainFlow.CreateScreen("About");
    about.HorizontalPosition = 3400;
    about.VerticalPosition = 2200;
    if (!about.Roles.Contains(eSpace.AnonymousRole)) about.Roles.Add(eSpace.AnonymousRole);
}
