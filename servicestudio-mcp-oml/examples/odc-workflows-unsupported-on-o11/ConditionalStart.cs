// Start a new execution branch in MyProcess workflow when MyEvent is raised and the following condition is true: 1 = 2
// Context: MyProcess workflow consists of a Start node connected to an End node. The module has a dependency to the MyEvent event declared in the Producer module

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var myProcess = eSpace.BusinessProcesses.Named("MyProcess");
    var conditionalStart = myProcess.CreateNode<OutSystems.Model.BusinessProcesses.Nodes.IConditionalStartNode>("ConditionalStart");
    conditionalStart.Label = "Conditional Start";
    var producer = eSpace.References.Named("Producer");
    var myEvent = producer.Events.Named("MyEvent");
    conditionalStart.StartFlowOn = myEvent;

    var eventCondition = conditionalStart.CreateCondition("1 = 2");

    var endNode = myProcess.CreateNode<OutSystems.Model.BusinessProcesses.Nodes.IEndNode>();
    endNode.Label = "End";
    conditionalStart.Target = endNode;
}
