// Create a server action ProjectDelete that takes a Project Id as input, validates edit permissions by calling ValidateCanEditProject, then deletes the Project record
// Context: The module has a server entity named Project, a server action named ValidateCanEditProject, and a folder named Project

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var projectDelete = eSpace.CreateServerAction("ProjectDelete");
    projectDelete.Folder = eSpace.Folders.Named("Project");

    var project = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Project");

    var id = projectDelete.CreateInputParameter("Id");
    id.DataType = project.IdentifierType;

    var startNode = projectDelete.CreateNode<OutSystems.Model.Logic.Nodes.IStartNode>();

    var validateCanEditProject = projectDelete.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("ValidateCanEditProject").ConnectedBelow(startNode);
    var validateCanEditProjectAction = eSpace.ServerActions.Named("ValidateCanEditProject");
    validateCanEditProject.Action = validateCanEditProjectAction;
    validateCanEditProject.SetArgumentValue(validateCanEditProjectAction.InputParameters.Named("Id"), "Id");

    var deleteEntity = projectDelete.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("DeleteEntity").ConnectedBelow(validateCanEditProject);
    deleteEntity.Action = project.DeleteAction;
    deleteEntity.SetArgumentValue(project.DeleteAction.InputParameters.Named("Id"), "Id");

    var endNode = projectDelete.CreateNode<OutSystems.Model.Logic.Nodes.IEndNode>().ConnectedBelow(deleteEntity);
}
