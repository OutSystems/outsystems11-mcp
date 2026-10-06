// Create a server action ValidateCanEditProject that takes a Project Id as input, checks if the user can edit via ProjectCanEdit(), and raises a SecurityException if not
// Context: The module has a server entity named Project, a server action named ProjectCanEdit, a user exception named SecurityException, and a folder named Project

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var validateCanEditProject = eSpace.CreateServerAction("ValidateCanEditProject");
    validateCanEditProject.Folder = eSpace.Folders.Named("Project");

    var project = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Project");

    var id = validateCanEditProject.CreateInputParameter("Id");
    id.DataType = project.IdentifierType;
    id.IsMandatory = false;

    var canEdit = validateCanEditProject.CreateLocalVariable("CanEdit");
    canEdit.DataType = eSpace.BooleanType;

    var startNode = validateCanEditProject.CreateNode<OutSystems.Model.Logic.Nodes.IStartNode>();

    var assignNode = validateCanEditProject.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>().ConnectedBelow(startNode);
    assignNode.CreateAssignment("CanEdit", "ProjectCanEdit()");

    var ifNode = validateCanEditProject.CreateNode<OutSystems.Model.Logic.Nodes.IIfNode>().ConnectedBelow(assignNode);
    ifNode.SetCondition("CanEdit");

    var raiseExceptionNode = validateCanEditProject.CreateNode<OutSystems.Model.Logic.Nodes.IRaiseExceptionNode>().ToTheRightOf(ifNode);
    raiseExceptionNode.Exception = eSpace.UserExceptions.Named("SecurityException");
    raiseExceptionNode.SetExceptionMessage("\"You don't have permission to write\"");
    ifNode.FalseTarget = raiseExceptionNode;

    var endNode = validateCanEditProject.CreateNode<OutSystems.Model.Logic.Nodes.IEndNode>().Below(ifNode);
    ifNode.TrueTarget = endNode;
}
