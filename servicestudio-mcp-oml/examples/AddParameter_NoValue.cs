// Add a new non-mandatory parameter named SomeValue in server action B. Leave its corresponding arguments empty in existing calls.
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
    someValue.IsMandatory = false;
}
