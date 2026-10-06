// Allow the List screen to be accessible also by people with the Administrator role
// Context: Module contains List screen in the Main ui flow, and two roles: Administrator and Manager. The List screen is only accessible by people with the Manager role.

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var main = eSpace.MobileFlows.Named("Main");
    var list = main.Nodes.OfType<OutSystems.Model.UI.Mobile.IMobileScreen>().Named("List");
    var administrator = eSpace.Roles.Named("Administrator");
    list.Roles.Add(administrator);
}
