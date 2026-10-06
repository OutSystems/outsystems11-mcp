// Add a new parameter named SomeValue in server action B. Use 123 as the value for its argument in existing calls.
// Context: Module contains two server actions: A and B. Action A calls action B.

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var b = eSpace.ServerActions.Named("B");
    var someValue = b.CreateInputParameter("SomeValue");
    someValue.DataType = eSpace.TextType;
    var a = eSpace.ServerActions.Named("A");
    var b2 = a.Nodes.OfType<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>().Named("B");
    b2.SetArgumentValue(someValue, "123");
}
