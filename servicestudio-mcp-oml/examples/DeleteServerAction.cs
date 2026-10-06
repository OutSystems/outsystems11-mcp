// Delete server action B
// Context: Module has two server actions, A and B

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    eSpace.ServerActions.Named("B").Delete();
}
