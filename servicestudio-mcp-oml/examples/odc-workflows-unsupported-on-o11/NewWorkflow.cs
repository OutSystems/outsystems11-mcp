// Create a new empty workflow named MyProcess. The workflow must start when MyEvent is raised.
// Context: Workflow module without any workflows. It has a dependency to the MyEvent event declared in the Producer module

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var myProcess = eSpace.CreateBusinessProcess("MyProcess");

    var start = myProcess.CreateNode<OutSystems.Model.BusinessProcesses.Nodes.IStartNode>("Start");
    start.Label = "Start";
    var producer = eSpace.References.Named("Producer");
    var myEvent = producer.Events.Named("MyEvent");
    start.StartProcessOn = myEvent;

    var endNode = myProcess.CreateNode<OutSystems.Model.BusinessProcesses.Nodes.IEndNode>().ConnectedBelow(start);
    endNode.Label = "End";
}
