// Add an automatic activity calling MyAction to a workflow. Use 123 as the argument for the MyAction action.
// Context: MyProcess workflow consists of a Start node connected to an End node. The module has a dependency to the MyEvent event and the MyAction service action declared in the Producer module. MyAction has a single parameter

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var myProcess = eSpace.BusinessProcesses.Named("MyProcess");
    var start = myProcess.Nodes.OfType<OutSystems.Model.BusinessProcesses.Nodes.IStartNode>().Named("Start");
    var automaticActivity = myProcess.CreateNode<OutSystems.Model.BusinessProcesses.Nodes.IAutomaticActivityNode>("AutomaticActivity").ConnectedBelow(start);
    var producer = eSpace.References.Named("Producer");
    var myAction = producer.ServiceActions.Named("MyAction");
    automaticActivity.ActionToTrigger = myAction;
    automaticActivity.Label = "Automatic Activity";
    var endNode = myProcess.Nodes.OfType<OutSystems.Model.BusinessProcesses.Nodes.IEndNode>().Single();
    automaticActivity.Target = endNode;
    var myParam = myAction.InputParameters.Named("MyParam");
    automaticActivity.SetArgumentValue(myParam, "123");
    endNode.VerticalPosition += 1759;
}
