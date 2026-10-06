// Create a server action ProjectCreateOrUpdate that takes a Project entity record as input, validates edit permissions by calling ValidateCanEditProject, then calls CreateOrUpdateProject, and outputs the new Id
// Context: The module has a server entity named Project, a server action named ValidateCanEditProject, and a folder named Project

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var projectCreateOrUpdate = eSpace.CreateServerAction("ProjectCreateOrUpdate");
    projectCreateOrUpdate.Folder = eSpace.Folders.Named("Project");

    var project = eSpace.Entities.OfType<OutSystems.Model.Data.IServerEntity>().Named("Project");

    var source = projectCreateOrUpdate.CreateInputParameter("Source");
    source.DataType = project;

    var startNode = projectCreateOrUpdate.CreateNode<OutSystems.Model.Logic.Nodes.IStartNode>();

    var validateCanEditProject = projectCreateOrUpdate.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("ValidateCanEditProject").ConnectedBelow(startNode);
    var validateCanEditProjectAction = eSpace.ServerActions.Named("ValidateCanEditProject");
    validateCanEditProject.Action = validateCanEditProjectAction;
    validateCanEditProject.SetArgumentValue(validateCanEditProjectAction.InputParameters.Named("Id"), "Source.Id");

    var createOrUpdateProject = projectCreateOrUpdate.CreateNode<OutSystems.Model.Logic.Nodes.IExecuteServerActionNode>("CreateOrUpdateProject").ConnectedBelow(validateCanEditProject);
    createOrUpdateProject.Action = project.CreateOrUpdateAction;
    createOrUpdateProject.SetArgumentValue(project.CreateOrUpdateAction.InputParameters.Named("Source"), "Source");

    var assignNode = projectCreateOrUpdate.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>().ConnectedBelow(createOrUpdateProject);
    assignNode.CreateAssignment("Id", "CreateOrUpdateProject.Id");

    var endNode = projectCreateOrUpdate.CreateNode<OutSystems.Model.Logic.Nodes.IEndNode>().ConnectedBelow(assignNode);

    var id = projectCreateOrUpdate.CreateOutputParameter("Id");
    id.DataType = project.IdentifierType;
}
