// Create a client action function ValidateProjectName that takes a Name input parameter and returns a boolean Result output parameter, checking that the name length is at most 200 characters
// Context: The module has a folder named Project

using OutSystems.Model;
using OutSystems.Model.Enumerations;
using OutSystems.Model.Expressions;
using OutSystems.Model.Factory;
using OutSystems.Model.Types;

eSpace => {
    var validateProjectName = eSpace.CreateClientAction("ValidateProjectName");
    validateProjectName.Folder = eSpace.Folders.Named("Project");
    validateProjectName.Function = true;

    var name = validateProjectName.CreateInputParameter("Name");
    name.DataType = eSpace.TextType;

    var startNode = validateProjectName.CreateNode<OutSystems.Model.Logic.Nodes.IStartNode>();

    var assignNode = validateProjectName.CreateNode<OutSystems.Model.Logic.Nodes.IAssignNode>().ConnectedBelow(startNode);
    assignNode.CreateAssignment("Result", "Length(Name) <= 200");

    var endNode = validateProjectName.CreateNode<OutSystems.Model.Logic.Nodes.IEndNode>().ConnectedBelow(assignNode);

    var result = validateProjectName.CreateOutputParameter("Result");
    result.DataType = eSpace.BooleanType;
    result.SetDefaultValue("True");
}
