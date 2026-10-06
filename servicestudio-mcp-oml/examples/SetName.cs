// Rename server action A to MyAction
// Context: Module has a single server action named A

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var a = eSpace.ServerActions.Named("A");
    a.Name = "MyAction";
}
