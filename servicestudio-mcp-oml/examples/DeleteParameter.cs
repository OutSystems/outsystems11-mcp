// Delete parameter SomeValue from server action B
// Context: Module contains two server actions: A and B. Action B has a single parameter named SomeValue. Action A calls action B.

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var b = eSpace.ServerActions.Named("B");
    b.InputParameters.Named("SomeValue").Delete();
}
