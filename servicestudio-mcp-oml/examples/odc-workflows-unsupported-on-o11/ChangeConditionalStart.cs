// Change the ConditionalStart condition of a workflow to 10 = 20. Add an additional condition: 3 = 4
// Context: MyProcess workflow of a Start node connected to an End node, and a ConditionalStart node connected to its own End node.

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var myProcess = eSpace.BusinessProcesses.Named("MyProcess");
    var conditionalStart = myProcess.Nodes.OfType<OutSystems.Model.BusinessProcesses.Nodes.IConditionalStartNode>().Named("ConditionalStart");
    var eventCondition = conditionalStart.Conditions.ElementAt(0);
    eventCondition.SetValue("10 = 20");

    var eventCondition2 = conditionalStart.CreateCondition("3 = 4");
}
