// Change the value of the SomeValue argument in server action B to 456 in all existing calls.
// Context: Module contains two server actions: A and B. Action B has a single parameter named SomeValue. Action A calls action B.

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var a = eSpace.ServerActions.Named("A");
    var b = a.Nodes.OfType<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>().Named("B");
    var b2 = eSpace.ServerActions.Named("B");
    var someValue = b2.InputParameters.Named("SomeValue");
    b.SetArgumentValue(someValue, "456");
}
