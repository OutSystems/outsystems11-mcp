// Replace the End node in MyProcess workflow by a Terminate node
// Context: MyProcess workflow consists of a Start node connected to an End node

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var myProcess = eSpace.BusinessProcesses.Named("MyProcess");
    var start = myProcess.Nodes.OfType<OutSystems.Model.BusinessProcesses.Nodes.IStartNode>().Named("Start");
    var terminateNode = myProcess.CreateNode<OutSystems.Model.BusinessProcesses.Nodes.ITerminateNode>().ConnectedBelow(start);
    myProcess.Nodes.OfType<OutSystems.Model.BusinessProcesses.Nodes.IEndNode>().Single().Delete();
}
