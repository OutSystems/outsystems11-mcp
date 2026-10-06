// Add a comment linked to the start node of MyProcess workflow with the following text: Hello
// Context: MyProcess workflow consists of a Start node connected to an End node

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var myProcess = eSpace.BusinessProcesses.Named("MyProcess");
    var commentNode = myProcess.CreateNode<OutSystems.Model.BusinessProcesses.Nodes.ICommentNode>();
    commentNode.Comment = "Hello";
    var start = myProcess.Nodes.OfType<OutSystems.Model.BusinessProcesses.Nodes.IStartNode>().Named("Start");
    commentNode.CreateConnector(start);
}
