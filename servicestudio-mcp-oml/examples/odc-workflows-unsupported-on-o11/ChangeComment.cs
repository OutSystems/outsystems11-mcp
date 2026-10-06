// Change the comment's text in a Comment node of a workflow to be: Hello, world
// Context: MyProcess workflow consists of a Start node connected to an End node, and a Comment node linked to the Start node

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var myProcess = eSpace.BusinessProcesses.Named("MyProcess");
    var commentNode = myProcess.Nodes.OfType<OutSystems.Model.BusinessProcesses.Nodes.ICommentNode>().Single();
    commentNode.Comment = "Hello, world!";
    var endNode = myProcess.Nodes.OfType<OutSystems.Model.BusinessProcesses.Nodes.IEndNode>().Single();
    var commentConnector = commentNode.Connectors.OfType<OutSystems.Model.BusinessProcesses.Nodes.ICommentConnector>().Single();
    commentConnector.Target = endNode;
}
