// Create a client action function ProjectCanRead that returns a boolean CanRead output parameter, assigned to (CheckAdminRole() or CheckManagerRole())
// Context: The module has a folder named Project

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var projectCanRead = eSpace.CreateClientAction("ProjectCanRead");
    projectCanRead.Folder = eSpace.Folders.Named("Project");
    projectCanRead.Function = true;

    var startNode = projectCanRead.CreateNode<OutSystems.Model.Logic.Nodes.IStartNode>();

    var assignNode = projectCanRead.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>().ConnectedBelow(startNode);
    assignNode.CreateAssignment("CanRead", "(CheckAdminRole() or CheckManagerRole())");

    var endNode = projectCanRead.CreateNode<OutSystems.Model.Logic.Nodes.IEndNode>().ConnectedBelow(assignNode);

    var canRead = projectCanRead.CreateOutputParameter("CanRead");
    canRead.DataType = eSpace.BooleanType;
}
